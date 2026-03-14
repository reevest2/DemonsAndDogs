using System.Net;
using API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DemonsAndDogs.API.Tests.Startup;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}

public class ApiStartupTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiStartupTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Application_Builds_Without_Exception()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        Assert.NotNull(response);
    }

    [Fact]
    public async Task SwaggerJson_Returns_Ok()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SwaggerUi_Returns_Ok_Or_Redirect()
    {
        var response = await _client.GetAsync("/swagger");
        Assert.True(
            response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.MovedPermanently,
            $"Swagger UI returned unexpected status: {response.StatusCode}");
    }
}
