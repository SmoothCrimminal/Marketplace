using Marketplace.Domain.Exceptions;
using Marketplace.Domain.Interfaces;
using Marketplace.Domain.ValueObjects;
using Moq;

namespace Marketplace.Tests
{
    public class MoneyTest
    {
        private readonly Mock<ICurrencyLookup> _fakeCurrencyLookup;

        public MoneyTest()
        {
            _fakeCurrencyLookup = new Mock<ICurrencyLookup>();
            _fakeCurrencyLookup.Setup(x => x.FindCurrency(It.IsAny<string>()))
            .Returns((string currencyCode) =>
            {
                return currencyCode switch
                {
                    "USD" => new CurrencyDetails
                    {
                        CurrencyCode = "USD",
                        DecimalPlaces = 2,
                        InUse = true
                    },
                    "JPY" => new CurrencyDetails
                    {
                        CurrencyCode = "JPY",
                        DecimalPlaces = 0,
                        InUse = true
                    },
                    "EUR" => new CurrencyDetails
                    {
                        CurrencyCode = "EUR",
                        DecimalPlaces = 2,
                        InUse = true
                    },
                    "DEM" => new CurrencyDetails
                    {
                        CurrencyCode = "DEM",
                        DecimalPlaces = 2,
                        InUse = false
                    },
                    _ => CurrencyDetails.None
                };
            });
        }

        [Fact]
        public void MoneyObjectsSameAmountShouldBeEqual()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", _fakeCurrencyLookup.Object);
            var secondAmount = Money.FromDecimal(5, "EUR", _fakeCurrencyLookup.Object);

            Assert.Equal(firstAmount, secondAmount);
        }

        [Fact]
        public void MoneyObjectsSameAmountDifferentCurrencyShouldNotBeEqual()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", _fakeCurrencyLookup.Object);
            var secondAmount = Money.FromDecimal(5, "USD", _fakeCurrencyLookup.Object);

            Assert.NotEqual(firstAmount, secondAmount);
        }

        [Fact]
        public void SumOfGivesFullAmount()
        {
            var coin1 = Money.FromDecimal(1, "EUR", _fakeCurrencyLookup.Object);
            var coin2 = Money.FromDecimal(2, "EUR", _fakeCurrencyLookup.Object);
            var coin3 = Money.FromDecimal(2, "EUR", _fakeCurrencyLookup.Object);

            var banknote = Money.FromDecimal(5, "EUR", _fakeCurrencyLookup.Object);

            Assert.Equal(banknote, coin1 + coin2 + coin3);
        }

        [Fact]
        public void UnknownCurrencyShouldNotBeAllowed()
        {
            Assert.Throws<ArgumentException>(() =>
                Money.FromDecimal(100, "CO?", _fakeCurrencyLookup.Object));
        }


        [Fact]
        public void UnusedCurrencyShouldNotBeAllowed()
        {
            Assert.Throws<ArgumentException>(() =>
                Money.FromDecimal(100, "DEM", _fakeCurrencyLookup.Object));
        }

        [Fact]
        public void ThrowWhenTooManyDecimalPlaces()
        {
            Assert.Throws<ArgumentException>(() =>
                Money.FromDecimal(100.123m, "DEM", _fakeCurrencyLookup.Object));
        }

        [Fact]
        public void ThrowAddingDifferentCurrencies()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", _fakeCurrencyLookup.Object);
            var secondAmount = Money.FromDecimal(5, "USD", _fakeCurrencyLookup.Object);

            Assert.Throws<CurrencyMismatchException>(() => firstAmount + secondAmount);
        }

        [Fact]
        public void ThrowSubtractingDifferentCurrencies()
        {
            var firstAmount = Money.FromDecimal(5, "EUR", _fakeCurrencyLookup.Object);
            var secondAmount = Money.FromDecimal(5, "USD", _fakeCurrencyLookup.Object);

            Assert.Throws<CurrencyMismatchException>(() => firstAmount - secondAmount);
        }
    }
}
