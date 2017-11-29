using System;
using System.Collections.Generic;
using System.Linq;

namespace JUG.Domain
{
    public static class PricePolicies
    {
        public static PricePolicy Labour(decimal pricePerHour, decimal minPrice, double freeInterventionTimeLimit)
        {
            return Labour(
                Money.FromDecimal(pricePerHour), 
                Money.FromDecimal(minPrice),
                Duration.FromHours(freeInterventionTimeLimit));
        }
        
        public static PricePolicy Labour(Money pricePerHour, Money minPrice, Duration freeInterventionTimeLimit)
        {
            return context =>
            {
                FreeInterventionsLimit freeInterventionsLimit = context.ContractLimits.FreeInterventionsLimit;
                if (freeInterventionsLimit.UsedInCurrentIntervention)
                {
                    Money labourOverLimit = LabourOverLimit(context, pricePerHour, freeInterventionTimeLimit);
                    return new Pricing(context.ContractLimits, labourOverLimit);
                }

                if (freeInterventionsLimit.CanUse)
                {
                    ContractLimits modifiedContractLimits = context.ContractLimits.UseFreeIntervention();
                    Money labourOverLimit = LabourOverLimit(context, pricePerHour, freeInterventionTimeLimit);
                    return new Pricing(modifiedContractLimits, labourOverLimit);
                }
                
                Money labour = Money.Max(pricePerHour * context.ServiceAction.Duration.Hours, minPrice);
                return new Pricing(context.ContractLimits, labour);
            };
        }

        private static Money LabourOverLimit(PricingContext context, Money pricePerHour, Duration freeInterventionTimeLimit)
        {
            return pricePerHour * (context.ServiceAction.Duration - freeInterventionTimeLimit).Hours;
        }

        public static PricePolicy SparePartsCost(IReadOnlyDictionary<int, Money> sparePartPrices)
        {
            return context =>
            {
                Money sparePartsCostLimit = context.ContractLimits.SparePartsCostLimit.Available;
                Money sparePartsCost = context.ServiceAction.SparePartIds
                    .Select(id => sparePartPrices[id])
                    .Aggregate(Money.Zero, Money.Sum);
                Money discount = Money.Min(sparePartsCost, sparePartsCostLimit);
                
                ContractLimits modifiedContractLimits = context.ContractLimits.UseSparePartsCostLimit(discount);
                return new Pricing(
                    modifiedContractLimits, 
                    sparePartsCost - discount);
            };
        }

        public static PricePolicy Free()
        {
            return context => new Pricing(context.ContractLimits, Money.Zero);
        }

        public static PricePolicy Sum(params PricePolicy[] policies)
        {
            return policies.Aggregate(Money.Sum);
        }

        public static PricePolicy Aggregate(this IEnumerable<PricePolicy> policies, Func<Money, Money, Money> valueAggregator)
        {
            return baseContext =>
            {
                Money total = Money.Zero;
                PricingContext context = baseContext;
                foreach (PricePolicy policy in policies)
                {
                    Pricing pricing = policy(context);
                    total = valueAggregator(total, pricing.Value);
                    context = new PricingContext(context.ServiceAction, pricing.ContractLimits);
                }
                return new Pricing(context.ContractLimits, total);
            };
        }

        public static PricePolicy When(this PricePolicy policy, Predicate<ServiceAction> condition)
        {
            return context => condition(context.ServiceAction)
                ? policy(context)
                : new Pricing(context.ContractLimits, Money.Zero);
        }
    }
}