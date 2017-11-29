namespace JUG.Domain
{
    public interface IContract
    {
        ContractLimits GetContractLimits();
        void AddUsage(ContractLimits interventionPricingContractLimits);
    }
}