using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVEMarketWatch.DealFinder
{
    public class Deal
    {
        public int TypeId { get; set; }
        public double MaxBuy { get; set; }
        public double MinSell { get; set; }
        public double MaxMargin { get; set; }
        public IEnumerable<Core.Domain.Order> Buys { get; set; }
        public IEnumerable<Core.Domain.Order> Sells { get; set; }
    }
}
