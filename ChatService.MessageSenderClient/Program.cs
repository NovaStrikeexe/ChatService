using ChatService.MessageSenderClient.Services;
using ChatService.MessageSenderClient.Services.Http;
using ChatService.MessageSenderClient.Services.Http.Implementation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ChatService.MessageSenderClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5150/") });
        builder.Services.AddScoped<IMessageService, MessageService>();

        await builder.Build().RunAsync();
    }
}