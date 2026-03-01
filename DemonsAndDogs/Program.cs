using Mediator;
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
        //Http client to the API
        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:44390/") }); //TODO: Put in settings somewhere
        
        
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(MediatorAssemblyMarker).Assembly));

        await builder.Build().RunAsync();
    }
}