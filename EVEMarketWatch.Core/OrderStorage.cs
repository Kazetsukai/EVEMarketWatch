﻿using NHibernate;
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
using System.Collections.Concurrent;
using System.Data;

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

            using (var session = _sessionFactory.OpenSession())
            using (IDbCommand command = session.Connection.CreateCommand())
            {
                command.CommandText = "PRAGMA journal_mode=WAL";
                command.ExecuteNonQuery();
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
    }
}
