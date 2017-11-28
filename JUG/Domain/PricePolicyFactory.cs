﻿using System.Collections.Generic;
using System.Threading.Tasks;
using JUG.CRUD;

namespace JUG.Domain
{
    public class PricePolicyFactory : IPricePolicyFactory
    {
        private readonly ICrmFacade _crmFacade;
        private readonly ISparePartsFacade _sparePartsFacade;

        public PricePolicyFactory(ICrmFacade crmFacade, ISparePartsFacade sparePartsFacade)
        {
            _crmFacade = crmFacade;
            _sparePartsFacade = sparePartsFacade;
        }

        public async Task<PricePolicy> CreateFor(Intervention intervention)
        {
            PricingCategory pricingCategory = await _crmFacade.GetPricingCategoryForClient(intervention.ClientId);
            IReadOnlyDictionary<int, Money> sparePartPrices = await _sparePartsFacade.GetPrices();
            
            return PricePolicies.Sum(
                PricePolicies.Labour(pricingCategory.PricePerHour, pricingCategory.MinPrice),
                PricePolicies.SparePartsCost(sparePartPrices));
        }
    }
}