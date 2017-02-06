using System;
using System.Xml.Serialization;
using BSU.Utils.Data;

namespace WowSuite.Utils.PlayerInfo
{
    [XmlRoot("Characters")]
    public class PlayerItemSet
    {
        [XmlArray("Stat")]
        [XmlArrayItem("CharBlock")]
        public PlayerItem[] Stat { get; set; }

        public static PlayerItemSet FromXml(string xmlText)
        {
            if (xmlText == null)
                throw new ArgumentNullException("xmlText");

            var set = DataSerializer<PlayerItemSet>.LoadFromXml(xmlText);

            return set;
        }
    }
}