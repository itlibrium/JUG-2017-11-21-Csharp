using System;
using System.Collections.Generic;
using ITLibrium.Hexagon.Domain.Meta;
using JUG.DataAccess;
using JUG.Model;

namespace JUG.Logic
{
    [DomainService]
    public class ServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly IContractRepository _contractRepository;

        //...

        public ServiceService(
            IServiceRepository serviceRepository,
            ISparePartRepository sparePartRepository,
            IContractRepository contractRepository)
        {
            _serviceRepository = serviceRepository;
            _sparePartRepository = sparePartRepository;
            _contractRepository = contractRepository;
        }
        
        //...

        public void Finish(int serviceId, double duration, IEnumerable<int> sparePartIds)
        {
            Service service = _serviceRepository.Get(serviceId);
            if (service.Status != ServiceStatus.Scheduled)
                throw new BusinessException("Nieprawidłowy status usługi");

            service.Duration = duration;
            service.SpareParts = GetSpareParts(sparePartIds);
            service.Price = GetPrice(service);
            service.Status = ServiceStatus.Done;
            
            _serviceRepository.Save(service);
        }
        
        private List<SparePart> GetSpareParts(IEnumerable<int> sparePartIds)
        {
            
            var spareParts = new List<SparePart>();
            foreach (int sparePartId in sparePartIds)
            {
                spareParts.Add(_sparePartRepository.Get(sparePartId));
            }
            return spareParts;
        }
        
        private decimal GetPrice(Service service)
        {
            if (service.IsWarranty)
                return GetWarrantyPrice();

            return GetNormalServicePrice(service);
        }

        private static decimal GetWarrantyPrice() => 0;

        private decimal GetNormalServicePrice(Service service)
        {
            EquipmentModel equipmentModel = service.Client.EquipmentModel;
            PricingCategory pricingCategory = equipmentModel.PricingCategory;
            Contract contract = service.Client.Contract;
            
            decimal sparePartsCost = GetSparePartsCost(service);
            
            if (contract == null)
            {
                return Math.Max(pricingCategory.MinPrice, pricingCategory.PricePerHour * (decimal) service.Duration) + sparePartsCost;
            }

            return GetPriceForServiceUnderContract(service, contract, sparePartsCost);
        }
        
        private static decimal GetSparePartsCost(Service service)
        {
            decimal sparePartsCost = 0;
            foreach (SparePart sparePart in service.SpareParts)
            {
                sparePartsCost += sparePart.Price;
            }
            return sparePartsCost;
        }
        
        private decimal GetPriceForServiceUnderContract(Service service, Contract contract, decimal sparePartsCost)
        {
            EquipmentModel equipmentModel = service.Client.EquipmentModel;
            PricingCategory pricingCategory = equipmentModel.PricingCategory;
            
            sparePartsCost = UpdateSparePartsCostLimit(contract, sparePartsCost);
            decimal price = sparePartsCost;
                    
            if (contract.FreeServicesUsed < contract.FreeServices)
            {
                contract.FreeServicesUsed++;
                price += GetPriceForTimeOverLimit(service);
            }
            else
            {
                price += Math.Max(pricingCategory.MinPrice, pricingCategory.PricePerHour * (decimal) service.Duration);
            }
                    
            _contractRepository.Save(contract);
            return price;
        }
        
        private static decimal UpdateSparePartsCostLimit(Contract contract, decimal sparePartsCost)
        {
            decimal sparePartsCostLimit = contract.SparePartsCostLimit - contract.SparePartsCostLimitUsed;
            if (sparePartsCostLimit >= sparePartsCost)
            {
                contract.SparePartsCostLimitUsed += sparePartsCost;
                sparePartsCost = 0;
            }
            else
            {
                contract.SparePartsCostLimitUsed = 0;
                sparePartsCost -= sparePartsCostLimit;
            }
            return sparePartsCost;
        }

        private static decimal GetPriceForTimeOverLimit(Service service)
        {
            EquipmentModel equipmentModel = service.Client.EquipmentModel;
            PricingCategory pricingCategory = equipmentModel.PricingCategory;
            
            double freeServiceTimeLimit = equipmentModel.FreeServiceTimeLimit;
            if (service.Duration > freeServiceTimeLimit)
            {
                return pricingCategory.PricePerHour * (decimal) (service.Duration - freeServiceTimeLimit);
            }

            return 0;
        }
        
        //...
    }
}