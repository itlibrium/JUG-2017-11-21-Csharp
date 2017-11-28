using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ITLibrium.Bdd.Scenarios;
using ITLibrium.Hexagon.App.Commands;
using ITLibrium.Hexagon.App.SimpleInjector;
using ITLibrium.Hexagon.Domain.SimpleInjector;
using ITLibrium.Hexagon.SimpleInjector;
using ITLibrium.Hexagon.SimpleInjector.Selectors;
using JUG.App;
using JUG.CRUD;
using JUG.Domain;
using JUG.Infrastructure;
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

        private class Fixture
        {
            private decimal _minPrice;
            private decimal _pricePerHour;
            private double _duration;
            private readonly List<SparePart> _usedSpareParts = new List<SparePart>();
            private bool _isWarranty;

            private bool _isInitialized;

            private int _interventionId;

            private ICommandHandler<FinishInterventionCommand> _handler;
            private JugDbContext _dbContext;

            public void MinPrice(decimal minPrice) => _minPrice = minPrice;
            public void PricePerHour(decimal pricePerHour) => _pricePerHour = pricePerHour;
            public void Duration(double hours) => _duration = hours;
            public void UsedSparePartPrices(params decimal[] prices) => _usedSpareParts.AddRange(prices.Select(p => new SparePart { Price = p }));
            public void IsWarrantyService() => _isWarranty = true;

            public void ServiceIsFinished()
            {
                Initialize();
                _handler.HandleAsync(
                    new FinishInterventionCommand(
                        _interventionId,
                        new[]{
                            new ServiceActionDto
                            {
                                Type = _isWarranty ? (int)ServiceActionType.WarrantyRepair : (int)ServiceActionType.Repair,
                                Hours = _duration,
                                SparePartIds = _usedSpareParts.Count == 0 ? new int[0] : _usedSpareParts.Select(p => p.Id).ToArray()
                            }}))
                    .Wait();
            }

            public void PriceShouldBe(decimal price) => _dbContext.Interventions.Find(_interventionId).Price.Should().Be(Money.FromDecimal(price));
            
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
                        }
                    }
                };
                dbContext.Clients.Add(client);

                var intervention = Intervention.CreateFor(client.Id);
                dbContext.Interventions.Add(intervention);

                dbContext.SaveChanges();
                _interventionId = intervention.Id;

                _handler = container.GetInstance<ICommandHandler<FinishInterventionCommand>>();
                _dbContext = dbContext;

                _isInitialized = true;
            }

            private static Container CreateContainer()
            {
                var container = new Container();
                container.Register(s => s
                    .UseLifestyle(Lifestyle.Singleton)
                    .SelectAssemblies(Assemblies.WithPrefix("JUG"))
                    .IncludeAppLogic()
                    .IncludeDomainStatelessLogic()
                    .Include(typeof(JugDbContext)));
                container.Verify();
                return container;
            }
        }
    }
}