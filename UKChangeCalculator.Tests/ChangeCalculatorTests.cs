
using NUnit.Framework;
using UKChangeCalculator.Models;
using UKChangeCalculator.Services;

namespace UKChangeCalculator.Tests;

[TestFixture]
public class ChangeCalculatorTests
{
    private ChangeCalculator _calculator = null!;

    [SetUp]
    public void SetUp()
    {
        var denominations = new List<Denomination>
        {
            new() { Name = "£50", ValueInPence = 5000 },
            new() { Name = "£20", ValueInPence = 2000 },
            new() { Name = "£10", ValueInPence = 1000 },
            new() { Name = "£5", ValueInPence = 500 },
            new() { Name = "£2", ValueInPence = 200 },
            new() { Name = "£1", ValueInPence = 100 },
            new() { Name = "50p", ValueInPence = 50 },
            new() { Name = "20p", ValueInPence = 20 },
            new() { Name = "10p", ValueInPence = 10 },
            new() { Name = "5p", ValueInPence = 5 },
            new() { Name = "2p", ValueInPence = 2 },
            new() { Name = "1p", ValueInPence = 1 }
        };

        _calculator = new ChangeCalculator(denominations);
    }

    [Test]
    public void CalculateChange_ValidAmounts_ReturnsCorrectChange()
    {
        var result = _calculator.CalculateChange(1000, 700);

        Assert.That(result["£2"], Is.EqualTo(1));
        Assert.That(result["£1"], Is.EqualTo(1));
    }

    [Test]
    public void CalculateChange_ExactPayment_ReturnsEmptyResult()
    {
        var result = _calculator.CalculateChange(500, 500);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void CalculateChange_ZeroAmounts_ReturnsEmptyResult()
    {
        var result = _calculator.CalculateChange(0, 0);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void CalculateChange_NegativeAmountPaid_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _calculator.CalculateChange(-100, 50));
    }

    [Test]
    public void CalculateChange_NegativeProductPrice_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _calculator.CalculateChange(100, -50));
    }

    [Test]
    public void CalculateChange_InsufficientPayment_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(
            () => _calculator.CalculateChange(500, 700));
    }

    [Test]
    public void CalculateChange_OnePenny_ReturnsOnePenny()
    {
        var result = _calculator.CalculateChange(101, 100);

        Assert.That(result["1p"], Is.EqualTo(1));
    }

    [Test]
    public void CalculateChange_FiftyPoundChange_ReturnsOneNote()
    {
        var result = _calculator.CalculateChange(10000, 5000);

        Assert.That(result["£50"], Is.EqualTo(1));
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public void CalculateChange_MultipleCoins_ReturnsCorrectChange()
    {
        var result = _calculator.CalculateChange(200, 137);

        Assert.That(result["50p"], Is.EqualTo(1));
        Assert.That(result["10p"], Is.EqualTo(1));
        Assert.That(result["2p"], Is.EqualTo(1));
        Assert.That(result["1p"], Is.EqualTo(1));
    }

    [Test]
    public void Constructor_NullDenominations_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ChangeCalculator(null!));
    }

    [Test]
    public void Constructor_EmptyDenominations_ThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () => new ChangeCalculator(new List<Denomination>()));
    }

    [Test]
    public void Constructor_ZeroDenomination_ThrowsException()
    {
        var denominations = new List<Denomination>
        {
            new() { Name = "Zero", ValueInPence = 0 }
        };

        Assert.Throws<ArgumentException>(
            () => new ChangeCalculator(denominations));
    }

    [Test]
    public void Constructor_NegativeDenomination_ThrowsException()
    {
        var denominations = new List<Denomination>
        {
            new() { Name = "Invalid", ValueInPence = -10 }
        };

        Assert.Throws<ArgumentException>(
            () => new ChangeCalculator(denominations));
    }

    [Test]
    public void CalculateChange_UnrepresentableChange_ThrowsException()
    {
        var denominations = new List<Denomination>
        {
            new() { Name = "2p", ValueInPence = 2 }
        };

        var calculator = new ChangeCalculator(denominations);

        Assert.Throws<InvalidOperationException>(
            () => calculator.CalculateChange(5, 0));
    }

    [Test]
    public void ChangeCalculator_ImplementsIChangeCalculator()
    {
        Assert.That(_calculator, Is.InstanceOf<IChangeCalculator>());
    }
}
