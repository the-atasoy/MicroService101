using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data.Entity;

[Index(nameof(CommandLine), nameof(PlatformId), IsUnique = true)]
public class Command
{
    [Key]
    public Guid Id { get; set; }
    public required string HowTo { get; set; }
    public required string CommandLine { get; set; }
    public Guid PlatformId { get; set; }
    public required Platform Platform { get; set; }
}