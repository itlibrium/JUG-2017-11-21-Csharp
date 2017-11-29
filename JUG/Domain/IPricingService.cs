using System.Collections.Generic;
using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;

namespace JUG.Domain
{
    [DomainService]
    public interface IPricingService
    {
        Task<InterventionPricing> GetPricingFor(Intervention intervention, IEnumerable<ServiceAction> serviceActions, ContractLimits contractLimits);
    }
}