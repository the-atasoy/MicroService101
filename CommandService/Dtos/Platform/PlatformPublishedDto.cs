namespace CommandService.Dtos.Platform;

public class PlatformPublishedDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Event { get; set; }
}