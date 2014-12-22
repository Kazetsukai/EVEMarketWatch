using System;
using System.Collections.Generic;
using System.Linq;
using EVEMarketWatch.Core.Domain;
using NHibernate;
using NHibernate.Linq;

namespace EVEMarketWatch.Core.Database.Query
{
    public class OrderQuery
    {
        private readonly ISessionFactory _sessionFactory;

        public OrderQuery(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
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

        public IEnumerable<Order> GetExpiredOrders()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                return session.Query<Order>().Where(o => o.expiryDate < DateTime.UtcNow).ToList(); //assumption that dates are in utc
            }
        }
    }
}
