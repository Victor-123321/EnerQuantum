using D3code.Data;
using D3code.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static D3code.Components.Pages.AreaDetails;

namespace D3code.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly EnerQuantumDbContext _context;

        public AreasController(EnerQuantumDbContext context)
        {
            _context = context;
        }

        // GET: api/areas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaDetailsDto>>> GetAreas()
        {
            var areas = await _context.Areas
                .Select(a => new AreaDetailsDto
                {
                    AreaId = a.AreaId,
                    Name = a.Name,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    GridType = a.GridType,
                    AverageDemandMWh = _context.EnergyUsage
                        .Where(eu => eu.AreaId == a.AreaId)
                        .Average(eu => (double?)eu.DemandMWh),
                    AverageGenerationMWh = _context.EnergyUsage
                        .Where(eu => eu.AreaId == a.AreaId)
                        .Average(eu => (double?)eu.GenerationMWh),
                    TotalOutages = _context.EnergyUsage
                        .Count(eu => eu.AreaId == a.AreaId && eu.ServiceStatus == "Outage")
                })
                .ToListAsync();

            return Ok(areas);
        }
        [HttpGet("AllAreas")]
        public async Task<ActionResult<AllAreaDataDto>> GetAllAreasData()
        {
            try
            {
                // Fetch areas with camelCase
                var areas = await _context.Areas
                    .Select(a => new AreaDto
                    {
                        area_id = a.AreaId,
                        name = a.Name,
                        latitude = a.Latitude ?? 0,
                        longitude = a.Longitude ?? 0,
                        gridType = a.GridType ?? string.Empty
                    })
                    .ToListAsync();

                // Fetch energy_usage with camelCase
                var energyUsage = await _context.EnergyUsage
                    .Select(eu => new EnergyUsageDto
                    {
                        timestamp = eu.Timestamp,
                        area_id = eu.AreaId,
                        generation_mwh = eu.GenerationMWh ?? 0,
                        demand_mwh = eu.DemandMWh ?? 0,
                        serviceStatus = eu.ServiceStatus ?? string.Empty,
                        losses_pct = eu.LossesPct ?? 0,
                        circuit = eu.Circuit ?? string.Empty,
                        durationHours = eu.DurationHours ?? 0,
                        import_mwh = eu.ImportMWh,
                        export_mwh = eu.ExportMWh,
                        net_exchange_mwh = eu.NetExchangeMWh
                    })
                    .ToListAsync();

                // Fetch climate_events with camelCase
                var climateEvents = await _context.ClimateEvents
                    .Select(ce => new ClimateEventDto
                    {
                        start_timestamp = ce.StartTimestamp,
                        area_id = ce.AreaId,
                        temp_c = ce.TempC ?? 0,
                        temp_min_c = ce.TempMinC ?? 0,
                        temp_max_c = ce.TempMaxC ?? 0,
                        climate_condition = ce.ClimateCondition ?? string.Empty,
                        phenomenon = ce.Phenomenon ?? string.Empty,
                        durationHours = ce.DurationHours ?? 0,
                        precipitation_mm = ce.PrecipitationMm ?? 0,
                        wind_speed_mps = ce.WindSpeedMps ?? 0,
                        pressure_hpa = ce.PressureHpa ?? 0
                    })
                    .ToListAsync();

                var data = new AllAreaDataDto
                {
                    areas = areas,
                    energy_usage = energyUsage,
                    climate_events = climateEvents,
                    infrastructure = new Dictionary<string, object>()
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error: {ex.Message}" });
            }
        }

        // GET: api/areas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDetailsDto>> GetArea(int id)
        {
            var area = await _context.Areas
                .Where(a => a.AreaId == id)
                .Select(a => new AreaDetailsDto
                {
                    AreaId = a.AreaId,
                    Name = a.Name,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    GridType = a.GridType,
                    AverageDemandMWh = _context.EnergyUsage
                        .Where(eu => eu.AreaId == a.AreaId)
                        .Average(eu => (double?)eu.DemandMWh),
                    AverageGenerationMWh = _context.EnergyUsage
                        .Where(eu => eu.AreaId == a.AreaId)
                        .Average(eu => (double?)eu.GenerationMWh),
                    TotalOutages = _context.EnergyUsage
                        .Count(eu => eu.AreaId == a.AreaId && eu.ServiceStatus == "Outage")
                })
                .FirstOrDefaultAsync();

            if (area == null)
            {
                return NotFound(new { Message = "Área no encontrada." });
            }

            return Ok(area);
        }
    }

    public class AreaDetailsDto
    {
        public int AreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string GridType { get; set; } = string.Empty;
        public double? AverageDemandMWh { get; set; }
        public double? AverageGenerationMWh { get; set; }
        public int TotalOutages { get; set; }
    }
}
