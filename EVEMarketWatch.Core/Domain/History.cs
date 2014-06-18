using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarketWatch.Core.Domain
{
    public class History
    {
        public int typeID { get; set; }
        public int regionID { get; set; }
        public DateTime generatedAt { get; set; }

        public DateTime date { get; set; }
        public double orders { get; set; }
        public double low { get; set; }
        public double high { get; set; }
        public double average { get; set; }
        public double quantity { get; set; }
    }
}
