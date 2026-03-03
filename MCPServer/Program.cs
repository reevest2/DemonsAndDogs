using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(Assembly.GetExecutingAssembly())
    .WithPromptsFromAssembly(Assembly.GetExecutingAssembly());

await builder.Build().RunAsync();