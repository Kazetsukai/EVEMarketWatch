using System;
using System.Collections.Generic;

namespace EVEMarketWatch
{
    public class DataInterchange
    {
        public List<UploadKey> uploadKeys { get; set; }
        public Generator generator { get; set; }
        public DateTime currentTime { get; set; }
        public ResultType resultType { get; set; }
        public string version { get; set; }

        public List<Rowset> rowsets { get; set; }
        public List<string> columns { get; set; }
    }

    public class UploadKey
    {
        public string name { get; set; }
        public string key { get; set; }
    }

    public class Generator
    {
        public string version { get; set; }
        public string name { get; set; }
    }

    public class Rowset
    {
        public int typeID { get; set; }
        public List<List<object>> rows { get; set; }
        public int regionID { get; set; }
        public DateTime generatedAt { get; set; }
    }

    public enum ResultType
    {
        orders,
        history
    }
}
