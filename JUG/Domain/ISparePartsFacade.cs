using System.Collections.Generic;
using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;

namespace JUG.Domain
{
    [DomainService]
    public interface ISparePartsFacade
    {
        Task<IReadOnlyDictionary<int, Money>> GetPrices();
    }
}