namespace JUG.Domain
{
    public class Contract : IContract
    {
        public int Id { get; private set; }
        public int ClientId { get; private set; }
        
        public int FreeInterventionsLimit { get; private set; }
        public Money SparePartsCostLimit { get; private set; }
        
        public int FreeInterventionsLimitUsed { get; private set; }
        public Money SparePartsCostLimitUsed { get; private set; }

        public Contract(
            int clientId,
            int freeInterventionsLimit, 
            int freeInterventionsLimitUsed, 
            Money sparePartsCostLimit, 
            Money sparePartsCostLimitUsed)
        {
            ClientId = clientId;
            FreeInterventionsLimit = freeInterventionsLimit;
            SparePartsCostLimit = sparePartsCostLimit;
            FreeInterventionsLimitUsed = freeInterventionsLimitUsed;
            SparePartsCostLimitUsed = sparePartsCostLimitUsed;
        }

        private Contract() { }

        public ContractLimits GetContractLimits() => ContractLimits.CreateInitial(
            Domain.FreeInterventionsLimit.CreateInitial(FreeInterventionsLimit - FreeInterventionsLimitUsed), 
            Domain.SparePartsCostLimit.CreateInitial(SparePartsCostLimit - SparePartsCostLimitUsed));
        
        public void AddUsage(ContractLimits contractLimits)
        {
            if (contractLimits.FreeInterventionsLimit.Used > FreeInterventionsLimit - FreeInterventionsLimitUsed)
                throw new BusinessException("Brak darmowych interwencji do wykorzystania w ramach umowy serwisowej");

            if (contractLimits.SparePartsCostLimit.Used > SparePartsCostLimit - SparePartsCostLimitUsed)
                throw new BusinessException("Koszt części zamiennych przekracza dopuszczalny limit w ramach umowy serwisowej");

            FreeInterventionsLimitUsed += contractLimits.FreeInterventionsLimit.Used;
            SparePartsCostLimitUsed += contractLimits.SparePartsCostLimit.Used;
        }
    }
}