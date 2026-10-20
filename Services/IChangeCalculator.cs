namespace UKChangeCalculator.Services;

public interface IChangeCalculator
{
    IReadOnlyDictionary<string, int> CalculateChange(
        int amountPaidInPence,
        int productPriceInPence);
}