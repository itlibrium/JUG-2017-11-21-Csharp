using System.Collections.Generic;
using System.Linq;

namespace JUG.Domain
{
    public class Intervention
    {
        public int Id { get; private set; }
        public int ClientId { get; private set; }

        private List<ServiceAction> ServiceActions { get; set; }
        public Money Price { get; private set; }

        //...

        public static Intervention CreateFor(int clientId) => new Intervention(clientId);

        private Intervention(int clientId)
        {
            ClientId = clientId;
        }

        private Intervention() { }

        public void Finish(IEnumerable<ServiceAction> serviceActions, InterventionPricing interventionPricing)
        {
            if (ServiceActions != null)
                throw new BusinessException("Nie można zakończyć interwencji więcej niż raz");

            ServiceActions = serviceActions.ToList();
            Price = interventionPricing.TotalPrice;
        }

        //...
    }
}

    