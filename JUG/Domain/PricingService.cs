using System.Collections.Generic;
using System.Threading.Tasks;

namespace JUG.Domain
{
    public class PricingService : IPricingService
    {
        private readonly IPricePolicyFactory _pricePolicyFactory;

        public PricingService(IPricePolicyFactory pricePolicyFactory)
        {
            _pricePolicyFactory = pricePolicyFactory;
        }

        public async Task<InterventionPricing> GetPricingFor(Intervention intervention, IEnumerable<ServiceAction> serviceActions, ContractLimits contractLimits)
        {
            PricePolicy pricePolicy = await _pricePolicyFactory.CreateFor(intervention);
            
            Money totalPrice = Money.Zero;
            foreach (ServiceAction serviceAction in serviceActions)
            {
                PricingContext context = new PricingContext(serviceAction, contractLimits);
                Pricing pricing = pricePolicy(context);
                contractLimits = pricing.ContractLimits;
                totalPrice += pricing.Value;
            }
            return new InterventionPricing(totalPrice, contractLimits);
        }
    }
}