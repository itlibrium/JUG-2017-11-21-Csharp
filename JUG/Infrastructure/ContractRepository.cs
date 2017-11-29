using System.Threading.Tasks;
using JUG.Domain;
using Microsoft.EntityFrameworkCore;

namespace JUG.Infrastructure
{
    public class ContractRepository : IContractRepository
    {
        private readonly JugDbContext _dbContext;

        public ContractRepository(JugDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IContract> GetForClient(int clientId)
        {
            IContract contract = await _dbContext.Contracts.SingleOrDefaultAsync(c => c.ClientId == clientId);
            return contract ?? new NoContract();
        }

        public Task Save(IContract contract)
        {
            if(contract is NoContract)
                return Task.CompletedTask;

            _dbContext.Contracts.Update((Contract) contract);
            return Task.CompletedTask;
        }
    }
}