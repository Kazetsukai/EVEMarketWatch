using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEMarketWatch.Core.Domain;
using FluentNHibernate.Mapping;

namespace EVEMarketWatch.Core.Database.Mappings
{
    public class HistoryMap : ClassMap<History>
    {
        public HistoryMap()
        {
            Id(x => x.Id);
            Id(x => x.average);
            Id(x => x.date);
            Id(x => x.generatedAt);
            Id(x => x.high);
            Id(x => x.low);
            Id(x => x.orders);
            Id(x => x.quantity);
            Id(x => x.regionID);
            Id(x => x.typeID);
        }
    }
}
