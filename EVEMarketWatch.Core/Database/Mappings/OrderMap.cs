using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEMarketWatch.Core.Domain;
using FluentNHibernate.Mapping;

namespace EVEMarketWatch.Core.Database.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.orderID);
            Map(x => x.typeID);
            Map(x => x.regionID);
            Map(x => x.generatedAt);
            Map(x => x.price);
            Map(x => x.volRemaining);
            Map(x => x.range);
            Map(x => x.volEntered);
            Map(x => x.minVolume);
            Map(x => x.bid);
            Map(x => x.issueDate);
            Map(x => x.expiryDate);
            Map(x => x.duration);
            Map(x => x.stationID);
            Map(x => x.solarSystemID);
        }
    }
}
