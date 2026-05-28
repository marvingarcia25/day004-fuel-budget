using FuelBudget.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<FuelPriceService>();

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/fuel", (FuelPriceService svc) =>
{
    var snap = svc.GetSnapshot();
    return Results.Ok(new
    {
        prices = snap.Prices.Select(p => new { p.Region, p.Grade, p.PricePerLitre }),
        weekLabel = snap.WeekLabel,
        fetchedAt = snap.FetchedAt
    });
});

app.MapGet("/api/fuel/calculate", (string? region, string? grade, double? efficiency, double? weeklyKm, FuelPriceService svc) =>
{
    if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(grade) || efficiency is null || weeklyKm is null)
        return Results.BadRequest(new { error = "region, grade, efficiency, and weeklyKm are required." });

    try
    {
        var price = svc.GetPrice(region, grade);
        var result = FuelPriceService.Calculate(price, efficiency.Value, weeklyKm.Value);
        return Results.Ok(new
        {
            region,
            grade,
            pricePerLitre = price,
            result.WeeklyLitres,
            result.WeeklyCost,
            result.MonthlyCost,
            result.AnnualCost
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

public partial class Program { }
