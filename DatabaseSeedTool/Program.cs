using DatabaseSeedTool.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Radzen;
using DbContext = DataAccess.DbContext;

namespace DatabaseSeedTool;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var connectionString = "Host=localhost;Port=5432;Database=demonsanddogs;Username=postgres;Password=\" \";";
        builder.Services.AddDbContextFactory<DbContext>(options =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            options.UseNpgsql(dataSource);
        });

        builder.Services.AddScoped<DatabaseSeedService>();
        builder.Services.AddScoped<NotificationService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
