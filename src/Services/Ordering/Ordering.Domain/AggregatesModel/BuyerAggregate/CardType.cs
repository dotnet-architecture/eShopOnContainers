using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate
{
    /// <remarks> 
    /// Card type class should be marked as abstract with protected constructor to encapsulate known enum types
    /// this is currently not possible as OrderingContextSeed uses this constructor to load cardTypes from csv file
    /// </remarks>
    public class CardType
        : Enumeration
    {
        public static CardType Amex = new AmexCardType();
        public static CardType Visa = new VisaCardType();
        public static CardType MasterCard = new MasterCardType();

        public CardType(int id, string name)
            : base(id, name)
        {
        }

        private class AmexCardType : CardType
        {
            public AmexCardType() : base(1, "Amex")
            { }
        }

        private class VisaCardType : CardType
        {
            public VisaCardType() : base(2, "Visa")
            { }
        }

        private class MasterCardType : CardType
        {
            public MasterCardType() : base(3, "MasterCard")
            { }
        }
    }
}
