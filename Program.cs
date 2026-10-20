using System.Globalization;
using System.Text.Json;
using UKChangeCalculator.Models;
using UKChangeCalculator.Services;

namespace UKChangeCalculator;

internal class Program
{
    private static void Main()
    {
        try
        {
            var denominations = LoadDenominations();

            IChangeCalculator calculator =
                new ChangeCalculator(denominations);

            decimal amountPaid = ReadAmount("Enter amount paid (£): ");
            decimal productPrice = ReadAmount("Enter product price (£): ");

            int paidInPence = decimal.ToInt32(amountPaid * 100m);
            int priceInPence = decimal.ToInt32(productPrice * 100m);

            var change = calculator.CalculateChange(
                paidInPence,
                priceInPence);

            Console.WriteLine();
            Console.WriteLine(
                $"Your change is: £{(paidInPence - priceInPence) / 100m:F2}");
            Console.WriteLine();

            if (change.Count == 0)
            {
                Console.WriteLine("No change to return.");
                return;
            }

            foreach (var item in change)
            {
                Console.WriteLine($"{item.Value} x {item.Key}");
            }
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is JsonException ||
            ex is InvalidOperationException ||
            ex is ArgumentException)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static List<Denomination> LoadDenominations()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Configuration",
            "denominations.json");

        string json = File.ReadAllText(path);

        var configuration =
            JsonSerializer.Deserialize<DenominationConfiguration>(json);

        return configuration?.Denominations
            ?? throw new InvalidOperationException(
                "Denomination configuration is empty.");
    }

    private static decimal ReadAmount(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            string? input = Console.ReadLine();

            if (decimal.TryParse(
                    input,
                    NumberStyles.Currency,
                    CultureInfo.GetCultureInfo("en-GB"),
                    out decimal amount)
                && amount >= 0
                && decimal.Round(amount, 2) == amount)
            {
                return amount;
            }

            Console.WriteLine(
                "Enter a valid non-negative amount with up to two decimal places.");
        }
    }
}

internal sealed class DenominationConfiguration
{
    public List<Denomination> Denominations { get; set; } = [];
}