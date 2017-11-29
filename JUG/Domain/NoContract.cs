namespace JUG.Domain
{
    public class NoContract : IContract
    {
        public ContractLimits GetContractLimits() => ContractLimits.NoContract;
        public void AddUsage(ContractLimits contractLimits) { }
    }
}