using System;
using System.Xml.Serialization;
using BSU.Utils.Data;

namespace WowSuite.Utils.Statistic
{
    [XmlRoot("Characters")]
    public class StatisticSet
    {
        [XmlArray("Stat")]
        [XmlArrayItem("CharBlock")]
        public StatisticItem[] Stat { get; set; }

        public static StatisticSet FromXml(string xmlText)
        {
            if (xmlText == null)
                throw new ArgumentNullException("xmlText");

            var set = DataSerializer<StatisticSet>.LoadFromXml(xmlText);

            return set;
        }
    }
}