using System.Threading.Tasks;
using JUG.Domain;

namespace JUG.Infrastructure
{
    public class InterventionRepository : IInterventionRepository
    {
        private readonly JugDbContext _dbContext;

        public InterventionRepository(JugDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Intervention> Get(int id)
        {
            return _dbContext.Interventions.FindAsync(id);
        }

        public Task Save(Intervention intervention)
        {
            _dbContext.Interventions.Update(intervention);
            return Task.CompletedTask;
        }
    }
}