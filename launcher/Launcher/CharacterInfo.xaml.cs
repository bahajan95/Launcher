using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WowSuite.Launcher.Logic;
using WowSuite.Launcher.Properties;
using WowSuite.Utils.PlayerInfo;

namespace WowSuite.Launcher
{
    /// <summary>
    /// Логика взаимодействия для CharacterInfo.xaml
    /// </summary>
    public partial class CharacterInfo : Window
    {
        private readonly LauncherLogic _launcher;
        private readonly AddressSet _addressSet;
        private readonly IpPortConfig _mySqlConfig;
        private readonly IpPortConfig _worldConfig;
        private readonly WebClientFactory _webClientFactory;
        private readonly WebClient _web;

        public string CharName { get; private set; }
        public string ImagePath { get; private set; }
        // переменные для каждого слота по картинке
        public string Slot0 { get; private set; }
        public string Slot1 { get; private set; }
        public string Slot2 { get; private set; }
        public string Slot3 { get; private set; }
        public string Slot4 { get; private set; }
        public string Slot5 { get; private set; }
        public string Slot6 { get; private set; }
        public string Slot7 { get; private set; }
        public string Slot8 { get; private set; }
        public string Slot9 { get; private set; }
        public string Slot10 { get; private set; }
        public string Slot11 { get; private set; }
        public string Slot12 { get; private set; }
        public string Slot13 { get; private set; }
        public string Slot14 { get; private set; }
        public string Slot15 { get; private set; }
        public string Slot16 { get; private set; }
        public string Slot17 { get; private set; }
        public string Slot18 { get; private set; }
        // переменные для каждого слота по картинке

        public CharacterInfo(string charName)
        {
            InitializeComponent();
            initIndicator.Visibility = Visibility.Hidden;
            CharName = charName;

            bgCharInfo.DataContext = ImagePath;

            // дата контекст для переменных слотов
            s0.DataContext = Slot0;
            s1.DataContext = Slot0;
            s2.DataContext = Slot0;
            s3.DataContext = Slot0;
            s4.DataContext = Slot0;
            s5.DataContext = Slot0;
            s6.DataContext = Slot0;
            s7.DataContext = Slot0;
            s8.DataContext = Slot0;
            s9.DataContext = Slot0;
            s10.DataContext = Slot0;
            s11.DataContext = Slot0;
            s12.DataContext = Slot0;
            s13.DataContext = Slot0;
            s14.DataContext = Slot0;
            s15.DataContext = Slot0;
            s16.DataContext = Slot0;
            s17.DataContext = Slot0;
            s18.DataContext = Slot0;
            // дата контекст для переменных слотов


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
                //ShowPlayerInfo = Settings.Default.site_link + "launcher/charinfo.php?charName=" + charName,// Display info character



                ShowPlayerInfo = string.Format(Settings.Default.api_url + "?_key={0}&_url=CharInfo/" + charName, Settings.Default.skey_api),
                PlayerImage = string.Format(Settings.Default.api_url + "?_key={0}&_url=CharItem/" + charName, Settings.Default.skey_api),
            };

            _addressSet = addressSet;
            _mySqlConfig = mySqlConfig;
            _worldConfig = worldConfig;

            _launcher = new LauncherLogic(addressSet, mySqlConfig, worldConfig);
            _launcher.ShowPlayerInfo += Launcher_OnShowPlayerInfo;


            _launcher.PlayerImage += Launcher_OnPlayerImage;

            _webClientFactory = new WebClientFactory();
            _web = _webClientFactory.Create();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _launcher.Initialize();

        }



        //Загружаем все, что касается информации персонажа через XML
        private void Launcher_OnShowPlayerInfo(object sender, LoadTextEventArgs e)
        {
            switch (e.State)
            {
                case LoadingState.Canceled:
                    break;

                case LoadingState.Started:
                    //StartWaitAnimation();
                    break;

                case LoadingState.Completed:
                    //StopWaitAnimation();
                    try
                    {
                        if (e.Result.Success)
                        {

                            PlayerInfoSet playerSet = PlayerInfoSet.FromXml(e.Result.Data); // player info
                            charInfoBox.ItemsSource = playerSet.Stat;
                            // получаем имг сорц как урл
                            ImagePath = playerSet.Stat.First(x => x.Name == CharName).Race;
                            BitmapImage bimage = new BitmapImage();
                            bimage.BeginInit();
                            bimage.UriSource = new Uri(ImagePath, UriKind.Relative);
                            bimage.EndInit();
                            bgCharInfo.Source = bimage;
                        }
                    }
                    catch (Exception err)
                    {

                        MessageBox.Show(
                            "Not possible to generate a character info. Please restart the program. If the error persists, again, please contact technical support project");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //Загружаем все, что касается информации персонажа через XML

        //Загружаем все, что касается информации об вещах через XML
        private void Launcher_OnPlayerImage(object sender, LoadTextEventArgs e)
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
                            PlayerItemSet imageInfo = PlayerItemSet.FromXml(e.Result.Data); // item image


                            // получаем имг сорц как урл
                            var player = imageInfo.Stat[0];
                            Slot0 = player.HeadChar;
                            Slot1 = player.Neck;
                            Slot2 = player.Shoulders;
                            Slot3 = player.BodyChar;
                            Slot4 = player.ChestChar;
                            Slot5 = player.Waist;
                            Slot6 = player.Legs;
                            Slot7 = player.Feet;
                            Slot8 = player.Wrists;
                            Slot9 = player.Hands;
                            Slot10 = player.Finger1;
                            Slot11 = player.Finger2;
                            Slot12 = player.Trinket1;
                            Slot13 = player.Trinket2;
                            Slot14 = player.Back;
                            Slot15 = player.MainHand;
                            Slot16 = player.OffHand;
                            Slot17 = player.Ranget;
                            Slot18 = player.Tabard;

                            BitmapImage img0 = new BitmapImage();
                            img0.BeginInit();
                            img0.UriSource = new Uri(Slot0, UriKind.Absolute);
                            img0.EndInit();
                            s0.Source = img0;

                            BitmapImage img1 = new BitmapImage();
                            img1.BeginInit();
                            img1.UriSource = new Uri(Slot1, UriKind.Absolute);
                            img1.EndInit();
                            s1.Source = img1;

                            BitmapImage img2 = new BitmapImage();
                            img2.BeginInit();
                            img2.UriSource = new Uri(Slot2, UriKind.Absolute);
                            img2.EndInit();
                            s2.Source = img2;

                            BitmapImage img3 = new BitmapImage();
                            img3.BeginInit();
                            img3.UriSource = new Uri(Slot3, UriKind.Absolute);
                            img3.EndInit();
                            s3.Source = img3;

                            BitmapImage img4 = new BitmapImage();
                            img4.BeginInit();
                            img4.UriSource = new Uri(Slot4, UriKind.Absolute);
                            img4.EndInit();
                            s4.Source = img4;

                            BitmapImage img5 = new BitmapImage();
                            img5.BeginInit();
                            img5.UriSource = new Uri(Slot5, UriKind.Absolute);
                            img5.EndInit();
                            s5.Source = img5;

                            BitmapImage img6 = new BitmapImage();
                            img6.BeginInit();
                            img6.UriSource = new Uri(Slot6, UriKind.Absolute);
                            img6.EndInit();
                            s6.Source = img6;

                            BitmapImage img7 = new BitmapImage();
                            img7.BeginInit();
                            img7.UriSource = new Uri(Slot7, UriKind.Absolute);
                            img7.EndInit();
                            s7.Source = img7;

                            BitmapImage img8 = new BitmapImage();
                            img8.BeginInit();
                            img8.UriSource = new Uri(Slot8, UriKind.Absolute);
                            img8.EndInit();
                            s8.Source = img8;

                            BitmapImage img9 = new BitmapImage();
                            img9.BeginInit();
                            img9.UriSource = new Uri(Slot9, UriKind.Absolute);
                            img9.EndInit();
                            s9.Source = img9;

                            BitmapImage img10 = new BitmapImage();
                            img10.BeginInit();
                            img10.UriSource = new Uri(Slot10, UriKind.Absolute);
                            img10.EndInit();
                            s10.Source = img10;

                            BitmapImage img11 = new BitmapImage();
                            img11.BeginInit();
                            img11.UriSource = new Uri(Slot11, UriKind.Absolute);
                            img11.EndInit();
                            s11.Source = img11;

                            BitmapImage img12 = new BitmapImage();
                            img12.BeginInit();
                            img12.UriSource = new Uri(Slot12, UriKind.Absolute);
                            img12.EndInit();
                            s12.Source = img12;

                            BitmapImage img13 = new BitmapImage();
                            img13.BeginInit();
                            img13.UriSource = new Uri(Slot13, UriKind.Absolute);
                            img13.EndInit();
                            s13.Source = img13;

                            BitmapImage img14 = new BitmapImage();
                            img14.BeginInit();
                            img14.UriSource = new Uri(Slot14, UriKind.Absolute);
                            img14.EndInit();
                            s14.Source = img14;

                            BitmapImage img15 = new BitmapImage();
                            img15.BeginInit();
                            img15.UriSource = new Uri(Slot15, UriKind.Absolute);
                            img15.EndInit();
                            s15.Source = img15;

                            BitmapImage img16 = new BitmapImage();
                            img16.BeginInit();
                            img16.UriSource = new Uri(Slot16, UriKind.Absolute);
                            img16.EndInit();
                            s16.Source = img16;

                            BitmapImage img17 = new BitmapImage();
                            img17.BeginInit();
                            img17.UriSource = new Uri(Slot17, UriKind.Absolute);
                            img17.EndInit();
                            s17.Source = img17;

                            BitmapImage img18 = new BitmapImage();
                            img18.BeginInit();
                            img18.UriSource = new Uri(Slot18, UriKind.Absolute);
                            img18.EndInit();
                            s18.Source = img18;

                        }
                    }
                    catch (Exception err)
                    {

                        MessageBox.Show(
                            "It is not possible to connect to the news server. Please restart the program. If the error persists, again, please contact technical support project");
                    
                    }
                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        //Загружаем все, что касается информации об вещах через XML


        private void closeBut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
    }
}
