using D3code.Data;
using D3code.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace D3code.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyDemandController : ControllerBase
    {
        private readonly EnerQuantumDbContext _context;

        public EnergyDemandController(EnerQuantumDbContext context)
        {
            _context = context;
        }

        [HttpGet("HospitalLeon")]
        public async Task<ActionResult<object>> GetAllAreaData()
        {
            try
            {
                var areas = await _context.Areas
                    .Select(a => new { a.AreaId, a.Name, a.Latitude, a.Longitude, a.GridType })
                    .ToListAsync();

                if (!areas.Any())
                {
                    return NotFound(new { Message = "No se encontraron áreas." });
                }

                var data = new
                {
                    areas = areas,
                    energy_usage = await (from eu in _context.EnergyUsage
                                          join ce in _context.ClimateEvents
                                          on new { eu.Timestamp, eu.AreaId } equals new { Timestamp = ce.StartTimestamp, ce.AreaId }
                                          select new
                                          {
                                              eu.Timestamp,
                                              eu.AreaId,
                                              GenerationMwh = eu.GenerationMWh,
                                              DemandMwh = eu.DemandMWh,
                                              eu.ServiceStatus,
                                              eu.LossesPct,
                                              eu.Circuit,
                                              eu.DurationHours,
                                              eu.ImportMWh,
                                              eu.ExportMWh,
                                              eu.NetExchangeMWh,
                                              ce.TempC,
                                              ce.TempMinC,
                                              ce.TempMaxC,
                                              ce.ClimateCondition,
                                              ce.Phenomenon,
                                              ce.PrecipitationMm,
                                              ce.WindSpeedMps,
                                              ce.PressureHpa
                                          }).ToListAsync(),
                    climate_events = await _context.ClimateEvents.ToListAsync(),
                    infrastructure = new { } // Add infrastructure data if available
                };

                if (!data.energy_usage.Any())
                {
                    return NotFound(new { Message = "No se encontraron datos de energía o clima." });
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error al obtener datos: {ex.Message}" });
            }
        }

        [HttpPost("GenerateHospitalData")]
        public async Task<ActionResult<string>> GenerateHospitalData()
        {
            try
            {
                // Clear existing data
                await _context.EnergyUsage
                    .Where(eu => eu.Area.Name == "Hospital León, Guanajuato")
                    .ExecuteDeleteAsync();
                await _context.ClimateEvents
                    .Where(ce => ce.Area.Name == "Hospital León, Guanajuato")
                    .ExecuteDeleteAsync();
                await _context.Areas
                    .Where(a => a.Name == "Hospital León, Guanajuato")
                    .ExecuteDeleteAsync();
                await _context.SaveChangesAsync();

                // Insert Area
                var area = new Area
                {
                    AreaId = 5,
                    Name = "Piso 4, Hospital León, Guanajuato",
                    Latitude = 21.1168,
                    Longitude = -101.6866,
                    GridType = "microgrid:hospital"
                };
                _context.Areas.Add(area);
                await _context.SaveChangesAsync();

                // Generate 90 days (2160 hours) from 2024-01-01
                var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc); 
                var energyData = new List<EnergyUsage>();
                var climateData = new List<ClimateEvent>();
                var circuits = new[] { "HOSP-ER-001", "HOSP-ICU-002", "HOSP-GEN-003", "HOSP-ADM-004" };
                var random = new Random(42); // Consistent seed for reproducibility

                // Outage triggers: 50 outages, weighted by weather or demand
                var outageTimestamps = new List<DateTime>();
                for (int i = 0; i < 50; i++)
                {
                    var daysOffset = random.Next(0, 90);
                    var hoursOffset = random.Next(0, 24);
                    outageTimestamps.Add(startDate.AddDays(daysOffset).AddHours(hoursOffset));
                }

                for (int hour = 0; hour < 2160; hour++)
                {
                    var timestamp = startDate.AddHours(hour);
                    var hourOfDay = timestamp.Hour;

                    // ClimateEvents: Simulate weather
                    var tempC = hourOfDay >= 12 && hourOfDay <= 18
                        ? random.NextDouble() * (25 - 15) + 15 // Warmer afternoons
                        : random.NextDouble() * (20 - 10) + 10; // Cooler nights
                    var tempMinC = tempC - (random.NextDouble() * 3 + 2);
                    var tempMaxC = tempC + (random.NextDouble() * 3 + 2);
                    var precipitationMm = random.NextDouble() < 0.2 ? random.NextDouble() * 5 : 0.0; // 20% chance of rain
                    var windSpeedMps = precipitationMm > 0 ? random.NextDouble() * (5 - 3) + 3 : random.NextDouble() * (3 - 1) + 1;
                    var pressureHpa = precipitationMm > 0 ? random.NextDouble() * (1015 - 1000) + 1000 : random.NextDouble() * (1025 - 1010) + 1010;
                    var condition = precipitationMm > 0 ? "Rain" : "Sunny";
                    var phenomenon = precipitationMm > 0 ? "Rain" : "None";

                    climateData.Add(new ClimateEvent
                    {
                        StartTimestamp = timestamp,
                        AreaId = area.AreaId,
                        TempC = tempC,
                        TempMinC = tempMinC,
                        TempMaxC = tempMaxC,
                        ClimateCondition = condition,
                        Phenomenon = phenomenon,
                        DurationHours = 24.0, // Daily weather
                        PrecipitationMm = precipitationMm,
                        WindSpeedMps = windSpeedMps,
                        PressureHpa = pressureHpa,
                        Area = area
                    });

                    // EnergyUsage: Simulate energy metrics
                    var generation = hourOfDay >= 8 && hourOfDay <= 18
                        ? random.NextDouble() * (100 - 20) + 20 // Solar peak
                        : random.NextDouble() * 20; // Low at night
                    var demand = hourOfDay >= 8 && hourOfDay <= 18
                        ? random.NextDouble() * (150 - 100) + 100 // Daytime peak
                        : random.NextDouble() * (120 - 80) + 80; // Night base
                    if (tempC > 20) demand += 10; // Heat increases AC load

                    // Outage logic: Triggered by high demand or bad weather
                    var isOutage = outageTimestamps.Any(ot => ot.Date == timestamp.Date && ot.Hour == timestamp.Hour)
                        && (precipitationMm > 2 || demand > 130);
                    var serviceStatus = isOutage ? "Outage" : "Normal";
                    var circuit = isOutage ? circuits[random.Next(0, circuits.Length)] : "HOSP-GEN-003";
                    var durationHours = isOutage ? random.NextDouble() * (2.5 - 0.5) + 0.5 : 0.0;
                    var lossesPct = isOutage ? 20.0 : 17.5 + random.NextDouble() * (1 - (-1));

                    var importMwh = isOutage ? demand : Math.Max(0, demand - generation + random.NextDouble() * 10 - 5);
                    var exportMwh = isOutage ? 0.0 : Math.Max(0, generation - demand + random.NextDouble() * 10 - 5);
                    var netExchange = importMwh - exportMwh;
                    generation = isOutage ? 0.0 : generation; // No generation during outage

                    energyData.Add(new EnergyUsage
                    {
                        Timestamp = timestamp,
                        AreaId = area.AreaId,
                        GenerationMWh = generation,
                        DemandMWh = demand,
                        ServiceStatus = serviceStatus,
                        LossesPct = lossesPct,
                        Circuit = circuit,
                        DurationHours = durationHours,
                        ImportMWh = importMwh,
                        ExportMWh = exportMwh,
                        NetExchangeMWh = netExchange,
                        Area = area
                    });
                }

                // Bulk insert to optimize performance
                _context.EnergyUsage.AddRange(energyData);
                _context.ClimateEvents.AddRange(climateData);
                await _context.SaveChangesAsync();

                return Ok($"Generated and saved: 1 Area, {energyData.Count} EnergyUsage rows, {climateData.Count} ClimateEvents rows");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating data: {ex.Message}");
            }
        }

        public class HospitalDataDto
        {
            public DateTime Timestamp { get; set; }
            public int AreaId { get; set; }
            public string AreaName { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string GridType { get; set; }
            public double? GenerationMWh { get; set; }
            public double? DemandMWh { get; set; }
            public string ServiceStatus { get; set; }
            public double? LossesPct { get; set; }
            public string Circuit { get; set; }
            public double? DurationHoursEnergy { get; set; }
            public double? ImportMWh { get; set; }
            public double? ExportMWh { get; set; }
            public double? NetExchangeMWh { get; set; }
            public double? TempC { get; set; }
            public double? TempMinC { get; set; }
            public double? TempMaxC { get; set; }
            public string ClimateCondition { get; set; }
            public string Phenomenon { get; set; }
            public double? DurationHoursClimate { get; set; }
            public double? PrecipitationMm { get; set; }
            public double? WindSpeedMps { get; set; }
            public double? PressureHpa { get; set; }
        }
    }
}
