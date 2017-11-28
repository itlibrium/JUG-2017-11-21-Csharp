using System.Collections.Generic;
using System.Threading.Tasks;
using JUG.Domain;
using Microsoft.EntityFrameworkCore;

namespace JUG.Infrastructure
{
    public class SparePartsFacade : ISparePartsFacade
    {
        private readonly JugDbContext _dbContext;

        public SparePartsFacade(JugDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyDictionary<int, Money>> GetPrices()
        {
            return await _dbContext.SpareParts.ToDictionaryAsync(sp => sp.Id, sp => Money.FromDecimal(sp.Price));
        }
    }
}