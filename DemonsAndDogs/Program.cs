using API.Client;
using API.Client.Abstraction;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

namespace DemonsAndDogs;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<TooltipService>();
        builder.Services.AddScoped<ContextMenuService>();
        builder.Services.AddScoped<ApiClient>();
        builder.Services.AddScoped<IApiClient>(sp => sp.GetRequiredService<ApiClient>());
        builder.Services.AddScoped<ISessionClient, SessionClient>();
        builder.Services.AddScoped<ICampaignClient, CampaignClient>();
        builder.Services.AddScoped<ICharacterClient, CharacterClient>();
        //Http client to the API
        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:44390/") }); //TODO: Put in settings somewhere

        await builder.Build().RunAsync();
    }
}
