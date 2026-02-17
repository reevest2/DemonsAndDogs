using API.Configuration;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using AppConstants;
using Npgsql;
using DbContext = DataAccess.DbContext;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var cs = builder.Configuration.GetConnectionString(AppConstants.Configuration.Default);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(cs);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        builder.Services.AddDbContext<DbContext>(options =>
            options.UseNpgsql(dataSource));
        builder.Services.ConfigureRepositories();
        builder.Services.ConfigureServices();

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