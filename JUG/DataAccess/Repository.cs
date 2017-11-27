using JUG.Model;

namespace JUG.DataAccess
{
    public class Repository : IServiceRepository, ISparePartRepository, IContractRepository
    {
        private readonly JugDbContext _dbContext;

        public Repository(JugDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        Service IServiceRepository.Get(int id)
        {
            return _dbContext.Services.Find(id);
        }

        void IServiceRepository.Save(Service service)
        {
            _dbContext.Services.Update(service);
            _dbContext.SaveChanges();
        }

        SparePart ISparePartRepository.Get(int id)
        {
            return _dbContext.SpareParts.Find(id);
        }

        Contract IContractRepository.Get(int id)
        {
            return _dbContext.Contracts.Find(id);
        }

        void IContractRepository.Save(Contract contract)
        {
            _dbContext.Contracts.Update(contract);
            _dbContext.SaveChanges();
        }
    }
}