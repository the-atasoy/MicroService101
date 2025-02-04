namespace PlatformService.Dtos.Platform;

public class PlatformReadDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Publisher { get; set; }
    public required string Cost { get; set; }
}