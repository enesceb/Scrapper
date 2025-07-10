using System.ComponentModel.DataAnnotations;

namespace MeshcapadeDataScraper.Models;

public class MeasurementData
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; }
    
    [Required]
    public decimal Height { get; set; }
    
    [Required]
    public decimal Weight { get; set; }
    
    [Required]
    public decimal Chest { get; set; }
    
    [Required]
    public decimal Waist { get; set; }
    
    [Required]
    public decimal Hip { get; set; }
    
    [Required]
    public decimal Inseam { get; set; }
    
    public string? Notes { get; set; }
} 