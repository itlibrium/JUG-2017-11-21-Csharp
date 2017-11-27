using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ITLibrium.Bdd.Scenarios;
using ITLibrium.Hexagon.Domain.SimpleInjector;
using ITLibrium.Hexagon.SimpleInjector;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using JUG.DataAccess;
using JUG.Logic;
using JUG.Model;
using SimpleInjector;
using Xunit;

namespace JUG.Tests
{
    public class AcceptanceTests
    {
        [Fact]
        public void LabourCostCalculatedCorrectly()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(300))
                .Test();
        }

        [Fact]
        public void LabourMinCostCalculatedCorrectly()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(1.5))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(200))
                .Test();
        }

        [Fact]
        public void SparePartsCostCalculatedCorrectly()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(0))
                    .And(f => f.PricePerHour(0))
                    .And(f => f.Duration(0))
                    .And(f => f.UsedSparePartPrices(500, 300))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(800))
                .Test();
        }

        [Fact]
        public void WarrantyCostCalculatedCorrectly()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                    .And(f => f.UsedSparePartPrices(500, 300))
                    .And(f => f.IsWarrantyService())
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(0))
                .Test();
        }

        [Fact]
        public void LabourCostIsNotAddedWhenFreeServiceIsAvailable()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                    .And(f => f.UsedSparePartPrices(500, 300))
                    .And(f => f.AvailableFreeServices(2))
                    .And(f => f.FreeServiceTimeLimit(5))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(800))
                .Test();
        }

        [Fact]
        public void FreeServicesUsedIsUpdatedWhenFreeServiceIsAvailable()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                    .And(f => f.UsedSparePartPrices(500, 300))
                    .And(f => f.AvailableFreeServices(2))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.FreeServicesUsedShouldBe(1))
                .Test();
        }

        [Fact]
        public void LabourCostOverLimitIsAddedInFreeService()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                    .And(f => f.UsedSparePartPrices(500, 300))
                    .And(f => f.AvailableFreeServices(2))
                    .And(f => f.FreeServiceTimeLimit(2))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(900))
                .Test();
        }

        [Fact]
        public void SparePartsCostIsNotAddedIfLessThenAvailableLimit()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                    .And(f => f.UsedSparePartPrices(500, 300))
                    .And(f => f.AvailableFreeServices(2))
                    .And(f => f.FreeServiceTimeLimit(5))
                    .And(f => f.AvailableSparePartsCostLimit(1000))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(0))
                    .And(f => f.SparePartsCostLimitUsedShouldBe(800))
                .Test();
        }

        [Fact]
        public void SparePartsCostOverAvailableLimitIsAdded()
        {
            BddScenario
                .Given<Fixture>()
                    .And(f => f.MinPrice(200))
                    .And(f => f.PricePerHour(100))
                    .And(f => f.Duration(3))
                    .And(f => f.UsedSparePartPrices(500, 300))
                    .And(f => f.AvailableFreeServices(2))
                    .And(f => f.FreeServiceTimeLimit(5))
                    .And(f => f.AvailableSparePartsCostLimit(500))
                .When(f => f.ServiceIsFinished())
                .Then(f => f.PriceShouldBe(300))
                    .And(f => f.SparePartsCostLimitUsedShouldBe(0))
                .Test();
        }

        private class Fixture
        {
            private decimal _minPrice;
            private decimal _pricePerHour;
            private double _duration;
            private readonly List<SparePart> _usedSpareParts = new List<SparePart>();
            private bool _isWarranty;
            private int _freeServices;
            private double _freeServicesTimeLimit;
            private decimal _sparePartsCostLimit;

            private bool _isInitialized;

            private int _serviceId;

            private ServiceService _serviceService;
            private JugDbContext _dbContext;

            public void MinPrice(decimal minPrice) => _minPrice = minPrice;
            public void PricePerHour(decimal pricePerHour) => _pricePerHour = pricePerHour;
            public void Duration(double hours) => _duration = hours;
            public void UsedSparePartPrices(params decimal[] prices) => _usedSpareParts.AddRange(prices.Select(p => new SparePart { Price = p }));
            public void IsWarrantyService() => _isWarranty = true;
            public void AvailableFreeServices(int freeServices) => _freeServices = freeServices;
            public void FreeServiceTimeLimit(double hours) => _freeServicesTimeLimit = hours;
            public void AvailableSparePartsCostLimit(decimal sparePartsCostLimit) => _sparePartsCostLimit = sparePartsCostLimit;

            public void ServiceIsFinished()
            {
                Initialize();
                _serviceService.Finish(_serviceId, _duration, _usedSpareParts.Select(p => p.Id));
            }

            public void PriceShouldBe(decimal price) => _dbContext.Services.Find(_serviceId).Price.Should().Be(price);
            public void FreeServicesUsedShouldBe(int count) => 
                _dbContext.Services.Find(_serviceId).Client.Contract.FreeServicesUsed.Should().Be(count);
            public void SparePartsCostLimitUsedShouldBe(decimal value) =>
                _dbContext.Services.Find(_serviceId).Client.Contract.SparePartsCostLimitUsed.Should().Be(value);

            private void Initialize()
            {
                if (_isInitialized)
                    throw new InvalidOperationException();

                Container container = CreateContainer();
                var dbContext = container.GetInstance<JugDbContext>();

                dbContext.SpareParts.AddRange(_usedSpareParts);

                var client = new Client
                {
                    EquipmentModel = new EquipmentModel
                    {
                        PricingCategory = new PricingCategory
                        {
                            MinPrice = _minPrice,
                            PricePerHour = _pricePerHour
                        },
                        FreeServiceTimeLimit = _freeServicesTimeLimit
                    },
                    Contract = _freeServices > 0 || _sparePartsCostLimit > 0
                        ? new Contract
                        {
                            FreeServices = _freeServices,
                            SparePartsCostLimit = _sparePartsCostLimit
                        }
                        : null
                };
                dbContext.Clients.Add(client);

                var service = new Service
                {
                    Client = client,
                    Status = ServiceStatus.Scheduled,
                    IsWarranty = _isWarranty
                };
                dbContext.Services.Add(service);

                dbContext.SaveChanges();
                _serviceId = service.Id;

                _serviceService = container.GetInstance<ServiceService>();
                _dbContext = dbContext;

                _isInitialized = true;
            }

            private static Container CreateContainer()
            {
                var container = new Container();
                container.Register(s => s
                    .UseLifestyle(Lifestyle.Singleton)
                    .SelectAssemblies(Assemblies.WithPrefix("JUG"))
                    .IncludeDomainStatelessLogic()
                    .Include(typeof(JugDbContext)));
                container.Verify();
                return container;
            }
        }
    }
}