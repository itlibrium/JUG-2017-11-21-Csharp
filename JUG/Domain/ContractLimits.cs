namespace JUG.Domain
{
    public struct ContractLimits
    {
        public FreeInterventionsLimit FreeInterventionsLimit { get; }
        public SparePartsCostLimit SparePartsCostLimit { get; }
        
        public static ContractLimits NoContract => 
            new ContractLimits(FreeInterventionsLimit.CreateInitial(0), SparePartsCostLimit.CreateInitial(Money.Zero));

        public static ContractLimits CreateInitial(FreeInterventionsLimit freeInterventionsLimit, SparePartsCostLimit sparePartsCostLimit) =>
            new ContractLimits(freeInterventionsLimit, sparePartsCostLimit);

        private ContractLimits(FreeInterventionsLimit freeInterventionsLimit, SparePartsCostLimit sparePartsCostLimit)
        {
            FreeInterventionsLimit = freeInterventionsLimit;
            SparePartsCostLimit = sparePartsCostLimit;
        }
        
        public ContractLimits UseFreeIntervention() => new ContractLimits(FreeInterventionsLimit.Use(), SparePartsCostLimit);
        public ContractLimits UseSparePartsCostLimit(Money value) => new ContractLimits(FreeInterventionsLimit, SparePartsCostLimit.Use(value));
    }
}