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
        public readonly static string DatabasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "EveMarket", "storage.sqlite");
        
        private NHibernate.ISessionFactory _sessionFactory;

        public OrderStorage()
        {
            // Initialize NHibernate
            var cfg = new Configuration();
            cfg.Configure();
            cfg.SetProperty("connection.connection_string", "Data Source=" + DatabasePath + ";Version=3");
            cfg.AddAssembly(typeof(Domain.Order).Assembly);

            // Create database and table if file doesn't exist yet
            if (!File.Exists(DatabasePath))
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
                    session.Save(order);
                }
                tx.Commit();
            }
        }
    }
}
