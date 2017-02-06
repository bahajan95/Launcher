using System.Xml.Serialization;

namespace WowSuite.Utils.News
{
    /// <summary>
    /// Экспресс новость
    /// </summary>
    public class ExpressNewsItem
    {
        /// <summary>Заголовок</summary>
        [XmlElement("NewsTitle")]
        public string Title { get; set; }

        /// <summary>Тело сообщения</summary>
        [XmlElement("Text")]
        public string Text { get; set; }

        /// <summary>Ссылка на изображение</summary>
        public string ImageLink { get; set; }

        /// <summary>Ссылка новость</summary>
        public string NewsLink { get; set; }
    }
}
