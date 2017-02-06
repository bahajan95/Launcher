using System;
using System.Xml.Serialization;
using BSU.Utils.Data;

namespace WowSuite.Utils.PlayerInfo
{
    [XmlRoot("Characters")]
    public class PlayerInfoSet
    {
        [XmlArray("Stat")]
        [XmlArrayItem("CharBlock")]
        public PlayerInfoItem[] Stat { get; set; }

        public static PlayerInfoSet FromXml(string xmlText)
        {
            if (xmlText == null)
                throw new ArgumentNullException("xmlText");

            var set = DataSerializer<PlayerInfoSet>.LoadFromXml(xmlText);

            return set;
        }
    }
}