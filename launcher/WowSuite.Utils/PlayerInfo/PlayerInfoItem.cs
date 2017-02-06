using System.Xml.Serialization;

namespace WowSuite.Utils.PlayerInfo
{
    /// <summary>
    /// Чтение XML статистики по персонажам онлайн
    /// </summary>
    public class PlayerInfoItem
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

        /// <summary>Здоровье</summary>
        [XmlElement("Health")]
        public string Health { get; set; }

        /// <summary>Сила</summary>
        [XmlElement("Power")]
        public string Power { get; set; }

        /// <summary>Общее время</summary>
        [XmlElement("TotalTime")]
        public string TotalTime { get; set; }

        /// <summary>Ранг</summary>
        [XmlElement("Rank")]
        public string Rank { get; set; }

        /// <summary>Хонор</summary>
        [XmlElement("Honor")]
        public string Honor { get; set; }

        /// <summary>Арена</summary>
        [XmlElement("Arena")]
        public string Arena { get; set; }

        /// <summary>Убийств</summary>
        [XmlElement("Kills")]
        public string Kills { get; set; }

        /// <summary>Заданий</summary>
        [XmlElement("Quest")]
        public string Quest { get; set; }

        /// <summary>Достижения</summary>
        [XmlElement("Achiev")]
        public string Achiev { get; set; }
    }
}