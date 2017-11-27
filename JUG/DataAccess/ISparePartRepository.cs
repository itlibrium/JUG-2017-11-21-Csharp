using ITLibrium.Hexagon.Domain.Meta;
using JUG.Model;

namespace JUG.DataAccess
{
    [Repository]
    public interface ISparePartRepository
    {
        SparePart Get(int id);
    }
}