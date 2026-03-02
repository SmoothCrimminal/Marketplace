using Marketplace.Domain.Exceptions;
using Marketplace.Domain.Interfaces;

namespace Marketplace.Domain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; }
        public CurrencyDetails Currency { get; }

        protected Money(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentNullException(nameof(currencyCode), "Currency code has to be defined");

            var currency = currencyLookup.FindCurrency(currencyCode);
            if (!currency.InUse)
                throw new ArgumentException($"Currency {currencyCode} is not in use");

            if (decimal.Round(amount, currency.DecimalPlaces) != amount)
                throw new ArgumentOutOfRangeException(nameof(amount), $"Amount cannot have more than {currency.DecimalPlaces} decimal places");

            Amount = amount;
            Currency = currency;
        }

        protected Money(decimal amount, CurrencyDetails currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money FromDecimal(decimal amount, string currency, ICurrencyLookup currencyLookup) => new Money(amount, currency, currencyLookup);
        public static Money FromString(string amount, string currency, ICurrencyLookup currencyLookup) => new Money(decimal.Parse(amount), currency, currencyLookup);

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new CurrencyMismatchException("Cannot add amounts in different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new CurrencyMismatchException("Cannot subtract amounts in different currencies");

            return new Money(Amount - other.Amount, Currency);
        }

        public static Money operator +(Money a, Money b) => a.Add(b);
        public static Money operator -(Money a, Money b) => a.Subtract(b);
    }
}
