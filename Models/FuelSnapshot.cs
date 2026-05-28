namespace FuelBudget.Models;

public record FuelSnapshot(
    IReadOnlyList<FuelPrice> Prices,
    string WeekLabel,
    DateTime FetchedAt
);
