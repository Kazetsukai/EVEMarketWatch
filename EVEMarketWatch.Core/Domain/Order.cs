using System;

namespace EVEMarketWatch.Core.Domain
{
    public class Order
    {
// ReSharper disable InconsistentNaming
        public virtual int typeID { get; set; }

        public virtual int regionID { get; set; }
        public virtual DateTime generatedAt { get; set; }

        public virtual double price { get; set; }
        public virtual double volRemaining { get; set; }
        public virtual double range { get; set; }
        public virtual double orderID { get; set; }
        public virtual double volEntered { get; set; }
        public virtual double minVolume { get; set; }

        /// <summary>
        /// if true, it's a buy order
        /// if false, it's a sell order
        /// </summary>
        public virtual bool bid { get; set; }
        public virtual DateTime issueDate { get; set; }
        public virtual DateTime expiryDate { get; set; }
        public virtual double duration { get; set; }
        public virtual double stationID { get; set; }
        public virtual double solarSystemID { get; set; }
// ReSharper restore InconsistentNaming
    }
}
