using System.Text;
using System.Text.Json;
using PlatformService.Dtos.Platform;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient(HttpClient client, IConfiguration configuration) : ICommandDataClient
{
    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            Encoding.UTF8,
            "application/json");
        
        var response = await client.PostAsync(
            $"{configuration["CommandService:BaseUrl"]}{configuration["CommandService:PlatformUrl"]}",
            httpContent);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "--> Sync POST to Command Service was OK!"
            : $"--> Sync POST to Command Service was NOT OK! {response.ReasonPhrase}");
    }
}