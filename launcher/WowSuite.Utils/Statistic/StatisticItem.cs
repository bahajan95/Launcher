using System.Xml.Serialization;

namespace WowSuite.Utils.Statistic
{
    /// <summary>
    /// Чтение XML статистики по персонажам онлайн
    /// </summary>
    public class StatisticItem
    {
        /// <summary>Имя чара</summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>Уровень</summary>
        [XmlElement("Level")]
        public string Level { get; set; }

        /// <summary>Ссылка на изображение рассы</summary>
        //[XmlElement("Race")]
        public string Race { get; set; }

        /// <summary>Класс</summary>
        [XmlElement("Class")]
        public string Class { get; set; }

        /// <summary>Фракция</summary>
        [XmlElement("Side")]
        public string Side { get; set; }

        /// <summary>Локация</summary>
        [XmlElement("Zone")]
        public string Zone { get; set; }

        /// <summary>Локация</summary>
        [XmlElement("TotalTime")]
        public string TotalTime { get; set; }
    }
}