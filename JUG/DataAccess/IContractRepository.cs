using ITLibrium.Hexagon.Domain.Meta;
using JUG.Model;

namespace JUG.DataAccess
{
    [Repository]
    public interface IContractRepository
    {
        Contract Get(int id);
        void Save(Contract contract);
    }
}