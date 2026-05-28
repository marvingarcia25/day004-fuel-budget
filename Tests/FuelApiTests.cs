using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

public class FuelApiTests
{
    [Fact]
    public async Task GetFuel_Returns200WithPrices()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/fuel");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("prices", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Calculate_ValidParams_Returns200()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/fuel/calculate?region=Auckland&grade=91&efficiency=12&weeklyKm=250");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("weeklyCost", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Calculate_ZeroEfficiency_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/fuel/calculate?region=Auckland&grade=91&efficiency=0&weeklyKm=250");
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Calculate_NegativeKm_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/fuel/calculate?region=Auckland&grade=91&efficiency=12&weeklyKm=-1");
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task Calculate_MissingParams_Returns400()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/fuel/calculate");
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
