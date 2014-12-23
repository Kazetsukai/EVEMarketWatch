using EVEMarketWatch.Core.Domain;

namespace EVEMarketWatch.Core.Utility
{
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
            me.expiryDate = order.expiryDate;
            me.duration = order.duration;
            me.stationID = order.stationID;
            me.solarSystemID = order.solarSystemID;
            me.generatedAt = order.generatedAt;
            me.regionID = order.regionID;
            me.typeID = order.typeID;

            // Todo: some consistency checking (typeID etc shouldn't change)
        }

        public static bool IsBuy(this Order order)
        {
            return order.bid;
        }

        public static bool IsSell(this Order order)
        {
            return !order.bid;
        }
    }
}
