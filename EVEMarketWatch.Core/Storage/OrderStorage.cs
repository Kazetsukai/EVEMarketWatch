using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EVEMarketWatch.Core.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;

namespace EVEMarketWatch.Core.Storage
{
    public class OrderStorage
    {
        public readonly static string DatabasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "EveMarket");
        public readonly static string DatabaseFilename = Path.Combine(DatabasePath, "storage.sqlite");

        private readonly ISessionFactory _sessionFactory;

        public OrderStorage()
        {
            // Initialize NHibernate
            var cfg = new Configuration();
            //cfg.Configure();
            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = @"Server=.\SQLExpress; Database=evemarket; Integrated Security=SSPI;";
                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2012Dialect>();
            });

            cfg.AddAssembly(typeof(Order).Assembly);

            new SchemaUpdate(cfg).Execute(true, true);

            // Get ourselves an NHibernate Session
            _sessionFactory = cfg.BuildSessionFactory();
        }

        public void Prune(DateTime dateTime)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                
            }
        }

        public IEnumerable<T> GetOrders<T>(Func<IQueryable<Order>, IQueryable<T>> doQuery)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                return doQuery(session.Query<Order>()).ToList();
            }
        }

        public void AddOrders(List<Order> orders)
        {
            if (!orders.Any())
                return;

            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                foreach (var order in orders)
                {
                    var existingOrder = session.Get<Order>(order.orderID);

                    if (existingOrder != null)
                    {
                        existingOrder.UpdateFrom(order);
                        session.Update(existingOrder);
                    }
                    else
                        session.Save(order);
                }
                tx.Commit();
            }
        }

        public IEnumerable<Order> GetOrdersSince(DateTime dateTime)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                return session.Query<Order>().Where(o => o.generatedAt > dateTime).ToList();
            }
        }

        public IEnumerable<Order> GetOrdersOfTypeId(int typeId)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                return session.Query<Order>().Where(o => o.typeID == typeId).ToList();
            }
        }
    }
}
