using Caliburn.Micro;
using EVEMarketWatch.Core.Data;
using EVEMarketWatch.Core.Database;
using EVEMarketWatch.Core.Database.Query;
using EVEMarketWatch.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ninject;

namespace EVEMarketWatch.DealFinder
{
    public class MainWindowViewModel : Screen
    {
        private IDictionary<int, InventoryType> _invTypes;

        private List<Order> _orders;

        public MainWindowViewModel()
        {
            //start ninject
            var kernel = new StandardKernel();
            //kernel.Load(Assembly.GetExecutingAssembly());

            //start the db
            var db = new ConfigureDatabase();
            var sessionFactory = db.Create(kernel);

            _invTypes = InventoryType.GetAll();

            DisplayName = "Fetching data...";
            _orders = new List<Order>();

            //var orderQuery = new OrderQuery(sessionFactory);
            var orderQuery = kernel.Get<OrderQuery>();

            var fetchDealsThread = new Thread(() =>
            {
                _orders.AddRange(orderQuery.GetOrdersSince(DateTime.Now.Subtract(TimeSpan.FromDays(2))));
                SpotSweetDeals();
            });

            fetchDealsThread.Start();
        }

        public IEnumerable<Deal> Deals
        {
            get;
            set;
        }
        
        private  Deal _selectedItem;
        public Deal SelectedItem 
        {
            get { return _selectedItem; }
            set 
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        private void SpotSweetDeals()
        {
            var orders = _orders
                .Where(o => o.volRemaining > 10)
                .GroupBy(o => o.typeID);

            var idealRatio = 1.1;

            Deals = from g in orders
                        let maxBuy = g.Any(o => o.bid) ? g.Where(o => o.bid).Max(o => o.price) : double.NegativeInfinity
                        let minSell = g.Any(o => !o.bid) ? g.Where(o => !o.bid).Min(o => o.price) : double.PositiveInfinity
                        let margin = maxBuy / minSell
                        let marginAbsolute = maxBuy - minSell
                        let type = ResolveInventoryType(g.Key)
                        let volume = type.VolumeNum
                        let marginPerMetreCubed = marginAbsolute / volume
                        where margin > idealRatio
                        orderby marginPerMetreCubed descending
                        select new Deal
                        {
                            TypeId = g.Key,
                            TypeName = type.TypeName,
                            MaxBuy = maxBuy,
                            MinSell = minSell,
                            Buys = from o in g where o.bid && o.price / minSell > idealRatio orderby o.price descending select o,
                            Sells = from o in g where !o.bid && maxBuy / o.price > idealRatio orderby o.price select o,
                            MaxMargin = Math.Round(margin, 2),
                            MaxMarginAbsolute = Math.Round(marginAbsolute, 2),
                            MarginPerMetreCubed = Math.Round(marginPerMetreCubed, 2)
                        };

            Execute.OnUIThread(() => 
                {
                    NotifyOfPropertyChange(() => Deals);
                    DisplayName = Deals.Count() + " potential deals found.";
                });
        }

        private InventoryType ResolveInventoryType(int typeId)
        {
            if (!_invTypes.ContainsKey(typeId))
                return new InventoryType();

            return _invTypes[typeId];
        }
    }
}
