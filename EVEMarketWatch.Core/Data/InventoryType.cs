using EVEMarketWatch.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarketWatch.Core.Data
{
    public class InventoryType
    {
        public string TypeID { get; set; }
        public string TypeName { get; set; }
        public string Mass { get; set; }
        public string Volume { get; set; }

        public double MassNum
        {
            get
            {
                double m = 0;
                if (double.TryParse(Mass, out m))
                    return m;

                return double.NaN;
            }
        }

        public double VolumeNum
        {
            get
            {
                double m = 0;
                if (double.TryParse(Volume, out m))
                    return m;

                return double.NaN;
            }
        }

        public static IDictionary<int, InventoryType> GetAll()
        {
            using (var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMarketWatch.Core.Data.invTypes.csv")))
            {
                var dict = new Dictionary<int, InventoryType>();
                foreach (var i in CSVParser.ParseInto<InventoryType>(streamReader.ReadToEnd()))
                {
                    if (!String.IsNullOrWhiteSpace(i.TypeID))
                        dict.Add(int.Parse(i.TypeID), i);
                }

                return dict;
            }
        }
    }
}
