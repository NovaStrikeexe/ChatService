using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ChatService.MessageSenderClient;
using ChatService.MessageSenderClient.Services;
using ClientApp.Services;

namespace ClientApp;

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