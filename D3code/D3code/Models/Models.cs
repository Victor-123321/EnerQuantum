using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace D3code.Models
{
    public class Area
    {
        [Key]
        public int AreaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [StringLength(50)]
        public string GridType { get; set; }

        // Navigation property for related EnergyUsage records
        public ICollection<EnergyUsage> EnergyUsage { get; set; } = new List<EnergyUsage>();
    }

    public class EnergyUsage
    {
        [Column(TypeName = "timestamptz")]
        public DateTime Timestamp { get; set; }

        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }

        public double? GenerationMWh { get; set; }
        public double? DemandMWh { get; set; }

        [StringLength(50)]
        public string ServiceStatus { get; set; }

        public double? LossesPct { get; set; }

        [StringLength(20)]
        public string Circuit { get; set; }

        public double? DurationHours { get; set; }
        public double? ImportMWh { get; set; }
        public double? ExportMWh { get; set; }
        public double? NetExchangeMWh { get; set; }

    }

    public class ClimateEvent
    {
        [Column(TypeName = "timestamptz")]
        public DateTime StartTimestamp { get; set; }

        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }

        public double? TempC { get; set; }
        public double? TempMinC { get; set; }
        public double? TempMaxC { get; set; }

        [StringLength(50)]
        public string ClimateCondition { get; set; }

        [StringLength(50)]
        public string Phenomenon { get; set; }

        public double? DurationHours { get; set; }
        public double? PrecipitationMm { get; set; }
        public double? WindSpeedMps { get; set; }
        public double? PressureHpa { get; set; }
    }
}