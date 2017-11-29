using System.Collections.Generic;
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
            EquipmentModel equipmentModel = await _crmFacade.GetEquipmentModelForClient(intervention.ClientId);
            
            return PricePolicies.Sum(
                PricePolicies
                    .Labour(pricingCategory.PricePerHour, pricingCategory.MinPrice, equipmentModel.FreeInterventionTimeLimit)
                    .When(IsNotWarranty),
                PricePolicies
                    .SparePartsCost(sparePartPrices)
                    .When(IsNotWarranty),
                PricePolicies
                    .Free()
                    .When(IsWarranty));
        }

        private static bool IsNotWarranty(ServiceAction serviceAction) => !IsWarranty(serviceAction);

        private static bool IsWarranty(ServiceAction serviceAction) =>
            serviceAction.Type == ServiceActionType.WarrantyRepair ||
            serviceAction.Type == ServiceActionType.WarrantyReview;
    }
}