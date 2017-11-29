namespace JUG.Domain
{
    public struct Pricing
    {
        public ContractLimits ContractLimits { get; }
        public Money Value { get; }

        public Pricing(ContractLimits contractLimits, Money value)
        {
            ContractLimits = contractLimits;
            Value = value;
        }
    }
}