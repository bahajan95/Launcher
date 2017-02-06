using System;
using System.Xml.Serialization;
using BSU.Utils.Data;

namespace WowSuite.Utils.News
{
    [XmlRoot("NewsRoot")]
    public class ExpressNewsSet
    {
        [XmlArray("ExpressNews")]
        [XmlArrayItem("NewsItem")]
        public ExpressNewsItem[] ExpressNews { get; set; }

        public static ExpressNewsSet FromXml(string xmlText)
        {
            if (xmlText == null)
                throw new ArgumentNullException("xmlText");

            var set = DataSerializer<ExpressNewsSet>.LoadFromXml(xmlText);

            return set;
        }
    }
}