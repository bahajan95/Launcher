using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WowLauncher.Utils.Graphic;

namespace WowSuite.Launcher
{
    internal class ContentProvider
    {
        const string UriPack = "pack://application:,,,";

        /// <summary>Uri путь к ресурсам текущей сборки</summary>
        static readonly string ResourcesCurrentAssembly = "/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/Resources/";

        static readonly ContentProvider _instance;

        static ContentProvider()
        {
            _instance = new ContentProvider();
        }

        private ContentProvider()
        {
            On = (BitmapImage)ImageHelper.ToBitmapSource(Properties.Resources.on);
            Off = (BitmapImage)ImageHelper.ToBitmapSource(Properties.Resources.off);

            const double WH = 40d; //ширина и высота кадра
            var blizzardWait = new BitmapImage(new Uri(UriPack + ResourcesCurrentAssembly + "spinner-40-battlenet.png"));

            int brushCount = (int)(blizzardWait.Width / WH);
            var brushParts = new ImageBrush[brushCount];
            for (int i = 0; i < brushCount; i++)
            {
                brushParts[i] = new ImageBrush(blizzardWait)
                {
                    ViewportUnits = BrushMappingMode.Absolute,
                    Stretch = Stretch.None,
                    TileMode = TileMode.None,
                    Viewport = new Rect(new Point(-i * WH, 0), new Size(blizzardWait.Width, WH))
                };
            }

            BlizzardAnimatedBrushes = brushParts;
            BlizzardWait = blizzardWait;
        }

        /// <summary>
        /// Единственный экземпляр класса
        /// </summary>
        public static ContentProvider Instance
        {
            get { return _instance; }
        }

        public ImageBrush[] BlizzardAnimatedBrushes { get; private set; }

        public BitmapImage BlizzardWait { get; private set; }

        public BitmapImage On { get; private set; }

        public BitmapImage Off { get; private set; }
    }
}
