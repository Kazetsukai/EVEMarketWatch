using System;
using System.Collections.Generic;
using System.Linq;
using EVEMarketWatch.Core.Domain;
using EVEMarketWatch.Core.Utility;
using NHibernate;
using NHibernate.Linq;

namespace EVEMarketWatch.Core.Database.Repository
{
    public class OrderRepository : RepositoryBase<Order>
    {
        private readonly ISession _session;

        public OrderRepository(ISession session) : base (session)
        {
            _session = session;
        }

        public void Prune(DateTime dateTime)
        {
            using (var tx = _session.BeginTransaction())
            {
                
            }
        }

        public void AddOrders(List<Order> orders)
        {
            if (!orders.Any())
                return;

            using (var tx = _session.BeginTransaction())
            {
                foreach (var order in orders)
                {
                    var existingOrder = _session.Get<Order>(order.orderID);

                    if (existingOrder != null)
                    {
                        existingOrder.UpdateFrom(order);
                        _session.Update(existingOrder);
                    }
                    else
                        _session.Save(order);
                }
                tx.Commit();
            }
        }
    }
}
