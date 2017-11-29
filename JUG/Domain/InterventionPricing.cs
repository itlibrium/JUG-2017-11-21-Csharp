namespace JUG.Domain
{
    public struct InterventionPricing
    {
        public Money TotalPrice { get; }
        public ContractLimits ContractLimits { get; }

        public InterventionPricing(Money totalPrice, ContractLimits contractLimits)
        {
            TotalPrice = totalPrice;
            ContractLimits = contractLimits;
        }
    }
}