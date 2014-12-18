using EVEMarketWatch.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarketWatch.Utils
{
    public static class DataInterchangeExtensions
    {
        public static List<Order> ConvertToOrders(this DataInterchange data)
        {
            var parsedOrders = new List<Order>();

            if (data.resultType == ResultType.history)
                return parsedOrders;

            var rowset = data.rowsets.First();

            foreach (var row in rowset.rows)
            {
                var order = new Order();

                for (var i = 0; i < row.Count; i++)
                {
                    var prop = typeof(Order).GetProperty(data.columns[i], BindingFlags.Public | BindingFlags.Instance);

                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(order, row[i], null);
                    }
                }

                order.typeID = rowset.typeID;
                order.regionID = rowset.regionID;
                order.generatedAt = rowset.generatedAt;

                parsedOrders.Add(order);
            }

            return parsedOrders;
        }

        public static List<History> ConvertToHistories(this DataInterchange data)
        {
            var parsedHistories = new List<History>();

            if (data.resultType == ResultType.orders)
                return parsedHistories;

            var rowset = data.rowsets.First();

            foreach (var row in rowset.rows)
            {
                var history = new History();

                for (var i = 0; i < row.Count; i++)
                {
                    var prop = typeof(History).GetProperty(data.columns[i], BindingFlags.Public | BindingFlags.Instance);

                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(history, row[i], null);
                    }
                }

                history.typeID = rowset.typeID;
                history.regionID = rowset.regionID;
                history.generatedAt = rowset.generatedAt;

                parsedHistories.Add(history);
            }

            return parsedHistories;
        }
    }
}
