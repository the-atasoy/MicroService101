using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data.Entity;

[Index(nameof(Name), IsUnique = true)]
public class Platform
{
    [Key]
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public required string Name { get; set; }
    public ICollection<Command> Commands { get; set; } = new List<Command>();
}