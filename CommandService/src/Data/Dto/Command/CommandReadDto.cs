namespace CommandService.Data.Dto.Command;

public class CommandReadDto
{
    public Guid Id { get; set; }
    public required string HowTo { get; set; }
    public required string CommandLine { get; set; }
    public Guid PlatformId { get; set; }
}