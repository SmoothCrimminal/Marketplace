using Marketplace.Domain.ValueObjects;

namespace Marketplace.Domain.Interfaces
{
    public interface ICurrencyLookup
    {
        CurrencyDetails FindCurrency(string currencyCode);
    }
}
