using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;
using JUG.CRUD;

namespace JUG.Domain
{
    [DomainService]
    public interface ICrmFacade
    {
        Task<PricingCategory> GetPricingCategoryForClient(int clientId);
        Task<EquipmentModel> GetEquipmentModelForClient(int clientId);
    }
}