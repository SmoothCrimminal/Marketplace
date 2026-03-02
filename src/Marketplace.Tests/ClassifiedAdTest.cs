using Marketplace.Domain.Entities;
using Marketplace.Domain.Enums;
using Marketplace.Domain.Exceptions;
using Marketplace.Domain.Interfaces;
using Marketplace.Domain.ValueObjects;
using Moq;

namespace Marketplace.Tests
{
    public class ClassifiedAdTest
    {
        private readonly Mock<ICurrencyLookup> _fakeCurrencyLookup;
        private readonly ClassifiedAd _classifiedAd;

        public ClassifiedAdTest()
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

            _classifiedAd = new ClassifiedAd(new ClassifiedAdId(Guid.NewGuid()), new UserId(Guid.NewGuid()));
        }

        [Fact]
        public void CanPublishAValidAd()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my things"));
            _classifiedAd.UpdatePrice(Price.FromDecimal(100.10m, "EUR", _fakeCurrencyLookup.Object));
            _classifiedAd.RequestToPublish();

            Assert.Equal(ClassifiedAdState.PendingReview, _classifiedAd.State);
        }

        [Fact]
        public void CanPnotublishWithoutTitle()
        {
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my things"));
            _classifiedAd.UpdatePrice(Price.FromDecimal(100.10m, "EUR", _fakeCurrencyLookup.Object));

            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void CanPnotublishWithoutText()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdatePrice(Price.FromDecimal(100.10m, "EUR", _fakeCurrencyLookup.Object));

            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void CannotPublishWithoutPrice()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my things"));

            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void CannotPublishWithZeroPrice()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my things"));
            _classifiedAd.UpdatePrice(Price.FromDecimal(0.0m, "EUR", _fakeCurrencyLookup.Object));

            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }
    }
}
