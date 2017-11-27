using ITLibrium.Hexagon.Domain.Meta;
using JUG.Model;

namespace JUG.DataAccess
{
    [Repository]
    public interface IServiceRepository
    {
        Service Get(int id);
        void Save(Service service);
    }
}