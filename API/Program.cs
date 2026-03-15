using API.Configuration;
using API.Middleware;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using AppConstants;
using Npgsql;
using DbContext = DataAccess.DbContext;

namespace API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(AppConstants.Cors.PolicyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // Add services to the container.

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<DbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        }
        else
        {
            var cs = builder.Configuration.GetConnectionString(AppConstants.Configuration.Default);
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(cs);
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();
            builder.Services.AddDbContext<DbContext>(options =>
                options.UseNpgsql(dataSource));
        }
        builder.Services.ConfigureRepositories();
        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder.Build();
        app.UseCors(AppConstants.Cors.PolicyName);

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHttpsRedirection();
        }
        app.UseExceptionHandler();
        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DbContext>();
            await DbSeeder.SeedAsync(db);
        }

        await app.RunAsync();
    }
}
