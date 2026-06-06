# day4_FuelBudget — Fuel Budget Calculator

Find out what your driving actually costs in fuel.

An ASP.NET Razor Pages calculator. Pick your region and fuel grade, enter your car's efficiency and weekly distance, and it works out the weekly, monthly, and annual cost.

## What it does

- Fuel prices by region + grade, with a national-average fallback when a region isn't listed
- Cost from efficiency (L/100km) and weekly km
- Weekly / monthly / annual breakdown (and litres burned per week)

## Stack

- ASP.NET Core (Razor Pages, .NET 8)
- Seeded in-memory `FuelPriceService`
- xUnit tests + GitHub Actions deploy workflow

## Running it

```
dotnet run
```

## Tests

```
dotnet test
```

---

Day 4 of building a small thing every day.
