using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITLibrium.Hexagon.App.Commands;
using JUG.Domain;

namespace JUG.App
{
    public class FinishInterventionHandler : ICommandHandler<FinishInterventionCommand>
    {
        private readonly IInterventionRepository _interventionRepository;
        
        private readonly IPricePolicyFactory _pricePolicyFactory;

        public FinishInterventionHandler(IInterventionRepository interventionRepository, IPricePolicyFactory pricePolicyFactory)
        {
            _interventionRepository = interventionRepository;
            _pricePolicyFactory = pricePolicyFactory;
        }

        public async Task HandleAsync(FinishInterventionCommand command)
        {
            Intervention intervention = await _interventionRepository.Get(command.InterventionId);
            PricePolicy pricePolicy = await _pricePolicyFactory.CreateFor(intervention);
            IEnumerable<ServiceAction> serviceActions = command.ServiceActions
                .Select(a => new ServiceAction(
                    (ServiceActionType)a.Type, 
                    Duration.FromHours(a.Hours), 
                    a.SparePartIds));

            intervention.Finish(serviceActions, pricePolicy);
            await _interventionRepository.Save(intervention);
        }
    }
}