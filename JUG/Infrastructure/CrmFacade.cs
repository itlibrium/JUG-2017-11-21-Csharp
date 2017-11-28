using System.Threading.Tasks;
using JUG.CRUD;
using JUG.Domain;

namespace JUG.Infrastructure
{
    public class CrmFacade : ICrmFacade
    {
        private readonly JugDbContext _dbContext;

        public CrmFacade(JugDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PricingCategory> GetPricingCategoryForClient(int clientId)
        {
            Client client = await _dbContext.Clients.FindAsync(clientId);
            return client.EquipmentModel.PricingCategory;
        }
        
        public async Task<EquipmentModel> GetEquipmentModelForClient(int clientId)
        {
            Client client = await _dbContext.Clients.FindAsync(clientId);
            return client.EquipmentModel;
        }
    }
}