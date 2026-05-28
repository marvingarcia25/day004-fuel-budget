using FuelBudget.Services;
using Xunit;

public class FuelPriceServiceTests
{
    [Fact]
    public void GetSnapshot_ReturnsNonEmptyPrices()
    {
        var svc = new FuelPriceService();
        var snap = svc.GetSnapshot();
        Assert.NotEmpty(snap.Prices);
    }

    [Fact]
    public void GetPrice_KnownRegionAndGrade_ReturnsPrice()
    {
        var svc = new FuelPriceService();
        var price = svc.GetPrice("Auckland", "91");
        Assert.True(price > 0);
    }

    [Fact]
    public void GetPrice_UnknownRegion_ReturnsFallback()
    {
        var svc = new FuelPriceService();
        var price = svc.GetPrice("Invercargill", "91");
        Assert.True(price > 0);
    }

    [Fact]
    public void Calculate_StandardInputs_ReturnsCorrectCosts()
    {
        // 12 L/100km, 250 km/week, $2.65/L
        // weeklyL = 250/100 * 12 = 30 L
        // weeklyCost = 30 * 2.65 = $79.50
        var result = FuelPriceService.Calculate(2.65, 12.0, 250.0);
        Assert.Equal(30.0, result.WeeklyLitres);
        Assert.Equal(79.50, result.WeeklyCost);
        Assert.Equal(Math.Round(79.50 * 52 / 12, 2), result.MonthlyCost);
        Assert.Equal(Math.Round(79.50 * 52, 2), result.AnnualCost);
    }

    [Fact]
    public void Calculate_ZeroKm_ReturnsZeroCosts()
    {
        var result = FuelPriceService.Calculate(2.65, 12.0, 0.0);
        Assert.Equal(0.0, result.WeeklyCost);
    }

    [Fact]
    public void Calculate_NegativeEfficiency_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FuelPriceService.Calculate(2.65, -1.0, 250.0));
    }

    [Fact]
    public void Calculate_ZeroEfficiency_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FuelPriceService.Calculate(2.65, 0.0, 250.0));
    }

    [Fact]
    public void Calculate_NegativeKm_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => FuelPriceService.Calculate(2.65, 12.0, -10.0));
    }

    [Theory]
    [InlineData("Auckland", "91")]
    [InlineData("Wellington", "Diesel")]
    [InlineData("Christchurch", "95")]
    public void GetPrice_AllSeedRegionsAndGrades_ReturnPositive(string region, string grade)
    {
        var svc = new FuelPriceService();
        Assert.True(svc.GetPrice(region, grade) > 0);
    }
}
