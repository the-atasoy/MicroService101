using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models;

public class Platform
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Publisher { get; set; }
    public required string Cost { get; set; }
}