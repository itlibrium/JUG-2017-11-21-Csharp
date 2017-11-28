using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;

namespace JUG.Domain
{
    [Repository]
    public interface IInterventionRepository
    {
        Task<Intervention> Get(int id);
        Task Save(Intervention intervention);
    }
}