using System;

namespace EVEMarketWatch.Core.Domain
{
    public class History
    {
        public History()
        {
            Id = new Guid();
        }

        public virtual Guid Id { get; set; }

        public virtual int typeID { get; set; }
        public virtual int regionID { get; set; }
        public virtual DateTime generatedAt { get; set; }

        public virtual DateTime date { get; set; }
        public virtual double orders { get; set; }
        public virtual double low { get; set; }
        public virtual double high { get; set; }
        public virtual double average { get; set; }
        public virtual double quantity { get; set; }
    }
}
