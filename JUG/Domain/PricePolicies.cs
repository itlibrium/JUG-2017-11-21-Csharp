using System;
using System.Collections.Generic;
using System.Linq;

namespace JUG.Domain
{
    public static class PricePolicies
    {
        public static PricePolicy Labour(decimal pricePerHour, decimal minPrice)
        {
            return Labour(Money.FromDecimal(pricePerHour), Money.FromDecimal(minPrice));
        }

        public static PricePolicy Labour(Money pricePerHour, Money minPrice)
        {
            return serviceAction =>
            {
                Money price = pricePerHour * serviceAction.Duration.Hours;
                return price < minPrice ? minPrice : price;
            };
        }

        public static PricePolicy SparePartsCost(IReadOnlyDictionary<int, Money> sparePartPrices)
        {
            return serviceAction => serviceAction.SparePartIds
                .Select(id => sparePartPrices[id])
                .Aggregate(Money.Zero, (total, price) => total + price);
        }

        public static PricePolicy Sum(params PricePolicy[] policies)
        {
            return serviceAction => policies.Aggregate(Money.Zero, (total, policy) => total + policy(serviceAction));
        }

        public static PricePolicy When(this PricePolicy policy, Predicate<ServiceAction> condition)
        {
            return serviceAction => condition(serviceAction)
                ? policy(serviceAction)
                : Money.Zero;
        }
    }
}