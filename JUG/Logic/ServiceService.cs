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
        
        //...
        
        public ServiceService(
            IServiceRepository serviceRepository,
            ISparePartRepository sparePartRepository)
        {
            _serviceRepository = serviceRepository;
            _sparePartRepository = sparePartRepository;
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
            
            decimal sparePartsCost = 0;
            foreach (SparePart sparePart in service.SpareParts)
            {
                sparePartsCost += sparePart.Price;
            }

            PricingCategory pricingCategory = service.Client.EquipmentModel.PricingCategory;
            service.Price = Math.Max(pricingCategory.MinPrice, pricingCategory.PricePerHour * (decimal) service.Duration) + sparePartsCost;
            
            service.Status = ServiceStatus.Done;
            _serviceRepository.Save(service);
        }
        
        //...
    }
}