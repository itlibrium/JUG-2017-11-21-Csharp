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
        private readonly IContractRepository _contractRepository;
        private readonly IPricingService _pricingService;

        public FinishInterventionHandler(IInterventionRepository interventionRepository, IContractRepository contractRepository, IPricingService pricingService)
        {
            _interventionRepository = interventionRepository;
            _contractRepository = contractRepository;
            _pricingService = pricingService;
        }

        public async Task HandleAsync(FinishInterventionCommand command)
        {
            Intervention intervention = await _interventionRepository.Get(command.InterventionId);
            IReadOnlyList<ServiceAction> serviceActions = command.ServiceActions
                .Select(a => new ServiceAction(
                    (ServiceActionType)a.Type, 
                    Duration.FromHours(a.Hours), 
                    a.SparePartIds))
                .ToList();
            
            IContract contract = await _contractRepository.GetForClient(intervention.ClientId);
            ContractLimits contractLimits = contract.GetContractLimits();

            InterventionPricing interventionPricing =
                await _pricingService.GetPricingFor(intervention, serviceActions, contractLimits);

            intervention.Finish(serviceActions, interventionPricing);
            await _interventionRepository.Save(intervention);
            
            contract.AddUsage(interventionPricing.ContractLimits);
            await _contractRepository.Save(contract);
        }
    }
}