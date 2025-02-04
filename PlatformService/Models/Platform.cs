using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Models;

[Index(nameof(Name), IsUnique = true)]
public class Platform
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Publisher { get; set; }
    public required string Cost { get; set; }
}