using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WowSuite.Launcher.Logic;
using WowSuite.Launcher.Properties;
using WowSuite.Utils;
using WowSuite.Utils.Statistic;

namespace WowSuite.Launcher
{
    /// <summary>
    /// Логика взаимодействия для OnlineStat.xaml
    /// </summary>

    public partial class OnlineStat : Window
    {

        private readonly LauncherLogic _launcher;
        private readonly AddressSet _addressSet;
        private readonly IpPortConfig _mySqlConfig;
        private readonly IpPortConfig _worldConfig;
        private readonly WebClientFactory _webClientFactory;
        private readonly WebClient _web;


        public OnlineStat()
        {
            InitializeComponent();
            initIndicator.Visibility = Visibility.Hidden;


            var mySqlConfig = new IpPortConfig
            {
                Ip = Settings.Default.mysql_ip,
                Port = Settings.Default.mysql_port
            };
            var worldConfig = new IpPortConfig
            {
                Ip = Settings.Default.world_ip,
                Port = Settings.Default.world_port
            };
            var addressSet = new AddressSet
            {
                LoadStatOnline = string.Format(Settings.Default.api_url + "?_key={0}&_url=online", Settings.Default.skey_api),
            };

            _addressSet = addressSet;
            _mySqlConfig = mySqlConfig;
            _worldConfig = worldConfig;

            _launcher = new LauncherLogic(addressSet, mySqlConfig, worldConfig);
            _launcher.StatOnline += Launcher_OnStatOnline;

            _webClientFactory = new WebClientFactory();
            _web = _webClientFactory.Create();
        }

        //Загружаем все, что касается статистики персонажа через XML
        private void Launcher_OnStatOnline(object sender, LoadTextEventArgs e)
        {
            switch (e.State)
            {
                case LoadingState.Canceled:
                    break;

                case LoadingState.Started:
                    StartWaitAnimation();
                    break;

                case LoadingState.Completed:
                    StopWaitAnimation();
                    try
                    {
                        if (e.Result.Success)
                        {
                            StatisticSet statSet = StatisticSet.FromXml(e.Result.Data);
                            charList.ItemsSource = statSet.Stat;
                        }
                    }
                    catch (Exception err)
                    {

                        MessageBox.Show("Not possible to generate a list of characters. Please restart the program. If the error persists, again, please contact technical support project");
                    }
                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //Загружаем все, что касается статистики персонажа через XML

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _launcher.Initialize();
            showList.Visibility = Visibility.Hidden;

        }

        void hideListClick()
        {
            try
            {
                charList.Visibility = Visibility.Hidden; //скрыть список персонажей
                showList.Visibility = Visibility.Visible;//показать кнопку "Показать список"
                hideList.Visibility = Visibility.Hidden;//скрыть кнопку "Скрыть список"
            }
            catch
            {
            }
        }

        void showListClick()
        {
            try
            {
                charList.Visibility = Visibility.Visible; //показать список персонажей
                showList.Visibility = Visibility.Hidden; //Скрыть кнопку "Показать список"
                hideList.Visibility = Visibility.Visible;//показать кнопку "Скрыть список"
            }
            catch
            {
            }
        }

        private void hideList_Click(object sender, RoutedEventArgs e)
        {
            hideListClick();
        }

        private void showList_Click(object sender, RoutedEventArgs e)
        {
            showListClick();
        }

        private void closeBut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void charList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(charList, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {

                string charName = (charList.SelectedItem as StatisticItem).Name;
                var charInfo = new CharacterInfo(charName);
                charInfo.Owner = this;
                charInfo.Show();

            }
        }

        private void charList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            exampleLabel.Visibility = Visibility.Visible;
        }

        public void StartWaitAnimation()
        {
            initIndicator.Visibility = Visibility.Visible;
            initIndicator.StartAnimation();
        }

        public void StopWaitAnimation()
        {
            initIndicator.Visibility = Visibility.Hidden;
            initIndicator.StopAnimation();
        }

        private void searchCharBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView view = CollectionViewSource.GetDefaultView(charList.ItemsSource) as CollectionView;
            if (view != null)
            {
                view.Filter = delegate(object x)
                {
                    var test = (x as StatisticItem);
                    //return test.Name.Contains(((TextBox)sender).Text);
                    return test.Name.ToUpper().Contains(((TextBox)sender).Text.ToUpper());
                };
            }
        }


        


    }
}
