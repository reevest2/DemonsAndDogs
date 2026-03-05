using DataAccess;
using Microsoft.EntityFrameworkCore;
using AppConstants;
using Models;
using Npgsql;
using ResourceFramework.Server.Extensions;
using DbContext = DataAccess.DbContext;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register all resource framework services, repositories, and auto-generated controllers
        builder.Services.AddResourceFramework(registry =>
        {
            registry.AddResource<TestResource>(ResourceTableNames.TestResources);
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var cs = builder.Configuration.GetConnectionString(Configuration.Default);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(cs);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        builder.Services.AddDbContext<DbContext>(options =>
            options.UseNpgsql(dataSource));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
