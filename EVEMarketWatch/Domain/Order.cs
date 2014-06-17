using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarketWatch.Domain
{
    public class Order
    {
        public int typeID { get; set; }
        public int regionID { get; set; }
        public DateTime generatedAt { get; set; }

        public double price { get; set; }
        public double volRemaining { get; set; }
        public double range { get; set; }
        public double orderID { get; set; }
        public double volEntered { get; set; }
        public double minVolume { get; set; }
        public bool bid { get; set; }
        public DateTime issueDate { get; set; }
        public double duration { get; set; }
        public double stationID { get; set; }
        //public double? solarSystemID { get; set; }
        public double solarSystemID { get; set; }
    }
}
