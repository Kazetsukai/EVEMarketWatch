using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEMarketWatch.Core.Domain;

namespace EVEMarketWatch.Core
{
    public class OrderStorage
    {
        public readonly static string DatabasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "EveMarket");
        public readonly static string DatabaseFilename = Path.Combine(DatabasePath, "storage.sqlite");

        private NHibernate.ISessionFactory _sessionFactory;

        public OrderStorage()
        {
            // Initialize NHibernate
            var cfg = new Configuration();
            cfg.Configure();
            cfg.SetProperty("connection.connection_string", "Data Source=" + DatabaseFilename + ";Version=3");
            cfg.AddAssembly(typeof(Domain.Order).Assembly);

            if (!Directory.Exists(DatabasePath))
                Directory.CreateDirectory(DatabasePath);

            // Create database and table if file doesn't exist yet
            if (!File.Exists(DatabaseFilename))
            {
                var schema = new SchemaExport(cfg);
                schema.Create(false, true);
            }

            // Get ourselves an NHibernate Session
            _sessionFactory = cfg.BuildSessionFactory();
        }

        public IEnumerable<T> GetOrders<T>(Func<IQueryable<Order>, IQueryable<T>> doQuery)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                return doQuery(session.Query<Order>()).ToList();
            }
        }

        public IEnumerable<IEnumerable<Order>> GetOrdersPaged(int pageSize)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var count = session.CreateCriteria<Order>()
                    .SetProjection(NHibernate.Criterion.Projections.RowCount())
                    .FutureValue<int>().Value;

                for (int i = 0; i < count; i += pageSize)
                {
                    var orders = session.CreateCriteria<Order>()
                        .SetFirstResult(i)
                        .SetMaxResults(pageSize)
                        .Future<Order>();

                    yield return orders;
                }
            }
        }

        public IEnumerable<Order> RecentOrders
        {
            get 
            {
                return GetOrders(i => i
                    .OrderByDescending(o => o.generatedAt)
                    .Take(100000));
            }
        }

        public void AddOrders(List<Order> orders)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                foreach (var order in orders)
                {
                    session.SaveOrUpdate(order);
                }
                tx.Commit();
            }
        }
    }
}
