using System.Collections.Generic;
using ITLibrium.Hexagon.App.Commands;

namespace JUG.App
{
    public class FinishInterventionCommand : ICommand
    {
        public int InterventionId { get; }
        public IReadOnlyList<ServiceActionDto> ServiceActions { get; }

        public FinishInterventionCommand(int interventionId, IReadOnlyList<ServiceActionDto> serviceActions)
        {
            InterventionId = interventionId;
            ServiceActions = serviceActions;
        }
    }
}