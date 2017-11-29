using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;

namespace JUG.Domain
{
    [Repository]
    public interface IContractRepository
    {
        Task<IContract> GetForClient(int clientId);
        Task Save(IContract contract);
    }
}