using System.Threading.Tasks;
using ITLibrium.Hexagon.Domain.Meta;

namespace JUG.Domain
{
    [Factory]
    public interface IPricePolicyFactory
    {
        Task<PricePolicy> CreateFor(Intervention intervention);
    }
}