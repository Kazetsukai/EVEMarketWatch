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
}
