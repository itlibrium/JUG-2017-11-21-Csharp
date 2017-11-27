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
            
            var spareParts = new List<SparePart>();
            foreach (int sparePartId in sparePartIds)
            {
                spareParts.Add(_sparePartRepository.Get(sparePartId));
            }
            service.SpareParts = spareParts;

            if (service.IsWarranty)
            {
                service.Price = 0;
            }
            else
            {
                decimal sparePartsCost = 0;
                foreach (SparePart sparePart in service.SpareParts)
                {
                    sparePartsCost += sparePart.Price;
                }

                Contract contract = service.Client.Contract;
                if (contract == null)
                {
                    PricingCategory pricingCategory = service.Client.EquipmentModel.PricingCategory;
                    service.Price = Math.Max(pricingCategory.MinPrice, pricingCategory.PricePerHour * (decimal)service.Duration) + sparePartsCost;
                }
                else
                {
                    decimal price = 0;
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
                    price += sparePartsCost;

                    if (contract.FreeServicesUsed < contract.FreeServices)
                    {
                        contract.FreeServicesUsed++;
                        
                        double freeServiceTimeLimit = service.Client.EquipmentModel.FreeServiceTimeLimit;
                        if (service.Duration > freeServiceTimeLimit)
                        {
                            price += service.Client.EquipmentModel.PricingCategory.PricePerHour * (decimal)(service.Duration - freeServiceTimeLimit);
                        }
                    }
                    else
                    {
                        PricingCategory pricingCategory = service.Client.EquipmentModel.PricingCategory;
                        price += Math.Max(pricingCategory.MinPrice, pricingCategory.PricePerHour * (decimal)service.Duration);
                    }

                    _contractRepository.Save(contract);
                    service.Price = price;
                }
            }
            
            service.Status = ServiceStatus.Done;
            _serviceRepository.Save(service);
        }
        
        //...
    }
}