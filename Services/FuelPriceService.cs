namespace FuelBudget.Services;

using FuelBudget.Models;

public class FuelPriceService
{
    private FuelSnapshot _snapshot;
    private readonly object _lock = new();

    public FuelPriceService()
    {
        _snapshot = BuildSeedSnapshot();
    }

    public FuelSnapshot GetSnapshot()
    {
        lock (_lock) { return _snapshot; }
    }

    /// <summary>
    /// Returns the price per litre for the given region and grade.
    /// Falls back to the national average for the grade if the region is not found.
    /// </summary>
    public double GetPrice(string region, string grade)
    {
        lock (_lock)
        {
            var match = _snapshot.Prices.FirstOrDefault(p =>
                p.Region.Equals(region, StringComparison.OrdinalIgnoreCase) &&
                p.Grade.Equals(grade, StringComparison.OrdinalIgnoreCase));

            if (match != null) return match.PricePerLitre;

            var nationalFallback = _snapshot.Prices.FirstOrDefault(p =>
                p.Region.Equals("National", StringComparison.OrdinalIgnoreCase) &&
                p.Grade.Equals(grade, StringComparison.OrdinalIgnoreCase));

            return nationalFallback?.PricePerLitre ?? 2.65;
        }
    }

    /// <summary>
    /// Calculates weekly, monthly, and annual fuel costs from driving inputs.
    /// </summary>
    public static CostResult Calculate(double pricePerLitre, double efficiencyL100km, double weeklyKm)
    {
        if (efficiencyL100km <= 0) throw new ArgumentException("Efficiency must be positive.");
        if (weeklyKm < 0) throw new ArgumentException("Weekly km cannot be negative.");

        var weeklyLitres = weeklyKm / 100.0 * efficiencyL100km;
        var weeklyCost = weeklyLitres * pricePerLitre;

        return new CostResult(
            WeeklyLitres: Math.Round(weeklyLitres, 1),
            WeeklyCost: Math.Round(weeklyCost, 2),
            MonthlyCost: Math.Round(weeklyCost * 52 / 12, 2),
            AnnualCost: Math.Round(weeklyCost * 52, 2)
        );
    }

    private static FuelSnapshot BuildSeedSnapshot() => new(
        Prices: new List<FuelPrice>
        {
            // National averages (NZD per litre, May 2026 approximations)
            new("National",     "91",     2.65),
            new("National",     "95",     2.80),
            new("National",     "Diesel", 2.05),
            // Regional prices
            new("Auckland",     "91",     2.68),
            new("Auckland",     "95",     2.83),
            new("Auckland",     "Diesel", 2.08),
            new("Wellington",   "91",     2.64),
            new("Wellington",   "95",     2.79),
            new("Wellington",   "Diesel", 2.04),
            new("Christchurch", "91",     2.61),
            new("Christchurch", "95",     2.76),
            new("Christchurch", "Diesel", 2.01),
            new("Hamilton",     "91",     2.66),
            new("Hamilton",     "95",     2.81),
            new("Hamilton",     "Diesel", 2.06),
            new("Dunedin",      "91",     2.63),
            new("Dunedin",      "95",     2.78),
            new("Dunedin",      "Diesel", 2.03),
        },
        WeekLabel: "Week ending 26 May 2026",
        FetchedAt: DateTime.UtcNow
    );
}

public record CostResult(double WeeklyLitres, double WeeklyCost, double MonthlyCost, double AnnualCost);
