using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WowSuite.Utils.Statistic;

namespace WowSuite.Launcher.Controls
{
    public partial class StatisticRotator : UserControl
    {
        public static readonly DependencyProperty NewsItemsProperty = DependencyProperty.Register(
            "Chars", typeof(StatisticItem[]), typeof(StatisticRotator),
            new PropertyMetadata(default(StatisticItem[]),
                new PropertyChangedCallback(NewsItemsPropertyChanged)));

        public static readonly DependencyProperty CurrentNewsItemProperty = DependencyProperty.Register(
            "CurrentCharItem", typeof(StatisticItem), typeof(StatisticRotator),
            new PropertyMetadata(default(StatisticItem)));

        private readonly Storyboard _storyboard;
        private int _currentIndex;

        public StatisticRotator()
        {
            InitializeComponent();
            _storyboard = FindResource("ChangeChars") as Storyboard;
            Debug.Assert(_storyboard != null, "_storyboard не должна быть null");
            Storyboard.SetTarget(_storyboard, mainGridStat);
        }

        /// <summary>
        /// Массив персонажей
        /// </summary>
        public StatisticItem[] Chars
        {
            get { return (StatisticItem[])GetValue(NewsItemsProperty); }
            set { SetValue(NewsItemsProperty, value); }
        }

        /// <summary>
        /// Отображаемые персонажи
        /// </summary>
        public StatisticItem CurrentCharItem
        {
            get { return (StatisticItem)GetValue(CurrentNewsItemProperty); }
            set { SetValue(CurrentNewsItemProperty, value); }
        }

        public BitmapImage Banner { get; set; }

        private static void NewsItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (StatisticRotator) d;
            var news = e.NewValue as StatisticItem[];
            if (news != null && news.Length > 0)
            {
                control.CurrentCharItem = news[0];
            }
        }


        //private void StatisticControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (CurrentCharItem != null && !string.IsNullOrEmpty(CurrentCharItem.NewsLink))
        //    {
        //        Process.Start(CurrentCharItem.NewsLink);
        //    }
        //}

        //Обработчик клика по кнопке "Прошлая новость"
        private void PreviousCharBlock(object sender, RoutedEventArgs e)
        {
            if (Chars != null)
            {
                if (--_currentIndex < 0)
                {
                    _currentIndex = Chars.Length - 1;
                }

                CurrentCharItem = Chars[_currentIndex];
            }
        }

        //Обработчик клика по кнопке "Следующая новость"
        private void NextCharBlock(object sender, RoutedEventArgs e)
        {
            if (Chars != null)
            {
                if (++_currentIndex == Chars.Length)
                {
                    _currentIndex = 0;
                }

                CurrentCharItem = Chars[_currentIndex];
            }
        } 
    }
}