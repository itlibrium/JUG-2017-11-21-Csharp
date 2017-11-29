namespace JUG.Domain
{
    public struct PricingContext
    {
        public ServiceAction ServiceAction { get; }
        public ContractLimits ContractLimits { get; }

        public PricingContext(ServiceAction serviceAction, ContractLimits contractLimits)
        {
            ServiceAction = serviceAction;
            ContractLimits = contractLimits;
        }
    }
}