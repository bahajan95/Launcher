using System.Xml.Serialization;

namespace WowSuite.Utils.PlayerInfo
{
    /// <summary>
    /// Чтение XML статистики по персонажам онлайн
    /// </summary>
    public class PlayerItem
    {
        /// <summary>Имя чара</summary>

        
        public string HeadChar { get; set; }
        public string Neck { get; set; }
        public string Shoulders { get; set; }
        public string BodyChar { get; set; }
        public string ChestChar { get; set; }
        public string Waist { get; set; }
        public string Legs { get; set; }
        public string Feet { get; set; }
        public string Wrists { get; set; }
        public string Hands { get; set; }
        public string Finger1 { get; set; }
        public string Finger2 { get; set; }
        public string Trinket1 { get; set; }
        public string Trinket2 { get; set; }
        public string Back { get; set; }
        public string MainHand { get; set; }
        public string OffHand { get; set; }
        public string Ranget { get; set; }
        public string Tabard { get; set; }
    }
}