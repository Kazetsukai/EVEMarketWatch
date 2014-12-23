using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EVEMarketWatch.Core.Utility
{
    public static class CSVParser
    {
        public static IEnumerable<T> ParseInto<T>(string csvData) where T : new()
        {
            string[][] lines = csvData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).Select(l => l.Split(new char[] { ',' }, StringSplitOptions.None)).ToArray();

            if (lines.Length == 0)
                yield break;

            var columnHeadings = lines[0].Select(s => s.ToLower()).ToArray();

            var relevantProperties = new Dictionary<string, PropertyInfo>();

            foreach (var property in typeof(T).GetProperties().Where(p => columnHeadings.Contains(p.Name.ToLower())))
            {
                relevantProperties.Add(property.Name.ToLower(), property);
            }

            foreach (var line in lines.Skip(1))
            {
                var obj = new T();

                for (int i = 0; i < line.Length; i++)
                {
                    if (i >= columnHeadings.Length)
                        break;
                    if (relevantProperties.ContainsKey(columnHeadings[i]))
                        relevantProperties[columnHeadings[i]].SetValue(obj, line[i]);
                }

                yield return obj;
            }
        }
    }
}
