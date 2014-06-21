using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarketWatch.Core.Domain
{
    public class Order
    {
        public virtual int typeID { get; set; }
        public virtual int regionID { get; set; }
        public virtual DateTime generatedAt { get; set; }

        public virtual double price { get; set; }
        public virtual double volRemaining { get; set; }
        public virtual double range { get; set; }
        public virtual double orderID { get; set; }
        public virtual double volEntered { get; set; }
        public virtual double minVolume { get; set; }
        public virtual bool bid { get; set; }
        public virtual DateTime issueDate { get; set; }
        public virtual double duration { get; set; }
        public virtual double stationID { get; set; }
        public virtual double solarSystemID { get; set; }
    }

    public static class OrderHelper
    {
        public static void UpdateFrom(this Order me, Order order)
        {
            me.price = order.price;
            me.volRemaining = order.volRemaining;
            me.range = order.range;
            me.volEntered = order.volEntered;
            me.minVolume = order.minVolume;
            me.bid = order.bid;
            me.issueDate = order.issueDate;
            me.duration = order.duration;
            me.stationID = order.stationID;
            me.solarSystemID = order.solarSystemID;
            me.generatedAt = order.generatedAt;
            me.regionID = order.regionID;
            me.typeID = order.typeID;

            // Todo: some consistency checking (typeID etc shouldn't change)
        }
    }
}
