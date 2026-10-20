using UKChangeCalculator.Models;

namespace UKChangeCalculator.Services;

public sealed class ChangeCalculator : IChangeCalculator
{
    private readonly IReadOnlyList<Denomination> _denominations;

    public ChangeCalculator(IEnumerable<Denomination> denominations)
    {
        ArgumentNullException.ThrowIfNull(denominations);

        _denominations = denominations
            .OrderByDescending(d => d.ValueInPence)
            .ToList();

        if (_denominations.Count == 0 ||
            _denominations.Any(d => d.ValueInPence <= 0))
        {
            throw new ArgumentException(
                "Denominations must contain positive values.",
                nameof(denominations));
        }
    }

    public IReadOnlyDictionary<string, int> CalculateChange(
        int amountPaidInPence,
        int productPriceInPence)
    {
        if (amountPaidInPence < 0 || productPriceInPence < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amountPaidInPence),
                "Amounts cannot be negative.");
        }

        if (amountPaidInPence < productPriceInPence)
        {
            throw new InvalidOperationException(
                "Amount paid is less than the product price.");
        }

        int remaining = amountPaidInPence - productPriceInPence;

        var result = new Dictionary<string, int>();

        foreach (var denomination in _denominations)
        {
            int count = remaining / denomination.ValueInPence;

            if (count > 0)
            {
                result.Add(denomination.Name, count);
                remaining %= denomination.ValueInPence;
            }
        }

        if (remaining != 0)
        {
            throw new InvalidOperationException(
                "The configured denominations cannot represent this change.");
        }

        return result;
    }
}