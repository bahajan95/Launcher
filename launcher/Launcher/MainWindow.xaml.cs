using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using BSU.Utils.Data;
using Constants;
using WowLauncher.Utils;
using WowSuite.Launcher.Config;
using WowSuite.Launcher.Controls;
using WowSuite.Launcher.Logic;
using WowSuite.Launcher.Logic.Updating;
using WowSuite.Launcher.Properties;
using WowSuite.Utils.News;
using MessageBox = System.Windows.MessageBox;

namespace WowSuite.Launcher
{
    public partial class MainWindow : Window
    {
        private readonly TranslateTransform _translate;
        private readonly TranslateTransform _translate1;
        private readonly TranslateTransform _translate2;
        private readonly TranslateTransform _translate3;
        private bool _mouseOnWindow;
        private Point _lastMousePos;

        private readonly WebClient _web;
        private readonly WebClientFactory _webClientFactory;
        private readonly WowUpdater _updater;
        private readonly LauncherLogic _launcher;

        private readonly AddressSet _addressSet;
        private readonly IpPortConfig _mySqlConfig;
        private readonly IpPortConfig _worldConfig;

        public MainWindow()
        {
            InitializeComponent();

            TransformGroup group = (TransformGroup)bg.RenderTransform;
            _translate = (TranslateTransform)group.Children[3];

            TransformGroup group1 = (TransformGroup)hand.RenderTransform;
            _translate1 = (TranslateTransform)group1.Children[3];

            TransformGroup group2 = (TransformGroup)handAxe.RenderTransform;
            _translate2 = (TranslateTransform)group2.Children[3];

            TransformGroup group3 = (TransformGroup)blik.RenderTransform;
            _translate3 = (TranslateTransform)group3.Children[3];

            SetVisibilityToHotNewsBlock(false);
            progressBar.Visibility = Visibility.Hidden;
            bannersLoader.Visibility = Visibility.Hidden;
            bannersFrame.Visibility = Visibility.Hidden;
            hotNewsTextBox.Text = "Initialization...";

            initIndicator.Visibility = Visibility.Hidden;

            _webClientFactory = new WebClientFactory();
            _web = _webClientFactory.Create();

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
                OnlinePlayers = string.Format(Settings.Default.api_url + "?_key={0}&_url=count_online", Settings.Default.skey_api),
                ServerPid = UrlHelper.Combine(Settings.Default.new_files, Wow.FileName.PID_FILE_NAME),
                ServerPatchFile = Settings.Default.patchlist,
                ServerFilesRoot = Settings.Default.new_files,
                NewsAboutUpdate = string.Format(Settings.Default.api_url + "?_key={0}&_url=news", Settings.Default.skey_api),
                HotNews = string.Format(Settings.Default.api_url + "?_key={0}&_url=hot_news", Settings.Default.skey_api),
                LoadBannerNews = string.Format(Settings.Default.api_url + "?_key={0}&_url=news", Settings.Default.skey_api),
             
            };

            _addressSet = addressSet;
            _mySqlConfig = mySqlConfig;
            _worldConfig = worldConfig;

            string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string localPidFile = Path.Combine(rootDirectory, Wow.FileName.PID_FILE_NAME);

            _updater = new WowUpdater(rootDirectory, addressSet.ServerPid, localPidFile, addressSet.ServerPatchFile, addressSet.ServerFilesRoot);
            _updater.UpdateProgressChanged += Updater_UpdateProgressChanged;
            _updater.UpdateStateChanged += Updater_UpdateStateChanged;

            _launcher = new LauncherLogic(addressSet, mySqlConfig, worldConfig);
            _launcher.DataLoadingStateChanged += Launcher_DataLoadingStateChanged;
            _launcher.NewsLoadStateChanged += Launcher_NewsLoadStateChanged;
            _launcher.NewsLoadBanner += Launcher_NewsLoadBanner;

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            servername.Content = Settings.Default.server_name;
            _launcher.Initialize();

            try
            {
                //Проверяем асинхронно требуется ли обновление.
                bool needUpdate = await _updater.CheckUpdateAsync();
                if (needUpdate)
                {
                    _updater.Update();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            UpdateOnlinePlayersCounter(_addressSet.OnlinePlayers, _worldConfig.Ip, _worldConfig.Port);
        }

        private void Updater_UpdateProgressChanged(object sender, ProgressEventArgs e)
        {
            Debug.WriteLine("Update progress: {0}", e.Progress);
            progressBar.Value = e.Progress;
        }

        private void Updater_UpdateStateChanged(object sender, UpdateStateEventArgs e)
        {
            switch (e.NewState)
            {
                //Апдейтер ничего не делает. Так же переключается в этот режим, чтобы сбросить текущее состояние. Например, после 
                //Completed сразу же срабатывает None.
                case UpdateState.None:
                    break;

                case UpdateState.Checking: //Проверяет наличие обновлений
                    playButton.IsEnabled = false;
                    break;

                case UpdateState.NotNeeded: //Обновление не требуется
                    playButton.IsEnabled = true;
                    progressBar.Visibility = Visibility.Hidden;
                    playButton.Content = "PLAY";
                    break;

                case UpdateState.Started: //Обновление запущено
                    playButton.IsEnabled = false;
                    progressBar.Visibility = Visibility.Visible;
                    playButton.Content = "UPDATE";
                    break;

                case UpdateState.Completed: //Обновление успешно завершено
                    playButton.IsEnabled = true;
                    progressBar.Visibility = Visibility.Hidden;
                    playButton.Content = "PLAY";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Launcher_DataLoadingStateChanged(object sender, LauncherStateEventArgs e)
        {
            switch (e.State)
            {
                case LauncherState.ConnectionFailed:
                    hotNewsTextBox.Text = "Not connection to server";
                    bannersLoader.Visibility = Visibility.Hidden;
                    bannersFrame.Visibility = Visibility.Hidden;
                    break;

                case LauncherState.InitializationStarted:
                    StartWaitAnimation();
                    break;

                case LauncherState.InitializationCompleted:
                    StopWaitAnimation();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Вывод новостей
        private void Launcher_NewsLoadBanner(object sender, LoadTextEventArgs e)
        {
            switch (e.State)
            {
                case LoadingState.Canceled:
                    break;

                case LoadingState.Started:
                    break;

                case LoadingState.Completed:
                    bannersLoader.Visibility = Visibility.Visible;
                    bannersFrame.Visibility = Visibility.Visible;
                    try
                    {
                        if (e.Result.Success)
                        {
                            ExpressNewsSet newsSet = ExpressNewsSet.FromXml(e.Result.Data);
                            bannersLoader.NewsItems = newsSet.ExpressNews;
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("It is not possible to connect to the news server. Please restart the program. If the error persists, again, please contact technical support project");
                        
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Вывод новостей

        private void Launcher_NewsLoadStateChanged(object sender, LoadTextEventArgs e)
        {
            switch (e.State)
            {
                case LoadingState.Canceled:
                    break;

                case LoadingState.Started:
                    hotNewsTextBox.Text = "Getting data...";
                    break;

                case LoadingState.Completed:
                    if (e.Result.Success)
                    {
                        hotNewsTextBox.Text = e.Result.Data;
                        SetVisibilityToHotNewsBlock(true);
                    }
                    else
                    {
                        hotNewsTextBox.Text = string.Empty;
                        SetVisibilityToHotNewsBlock(false);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetVisibilityToHotNewsBlock(bool visible)
        {
            if (visible)
            {
                hotNewsTextBox.Visibility = Visibility.Visible;
                hotNewsLabel.Visibility = Visibility.Visible;
                hotNewsBlock.Visibility = Visibility.Visible;
            }
            else
            {
                hotNewsTextBox.Visibility = Visibility.Hidden;
                hotNewsLabel.Visibility = Visibility.Hidden;
                hotNewsBlock.Visibility = Visibility.Hidden;
            }
        }

        private async void UpdateOnlinePlayersCounter(string onlinePlayersAddress, string worldIp, int worldPort)
        {
            bool connected = await _launcher.CheckConnectionToServerAsync(worldIp, worldPort);
            if (connected)
            {
                on_off.Content = "Online";
                on_off.Foreground = Brushes.Lime;

                WebClient web = _webClientFactory.Create();
                onlinePlayer.Content = await web.DownloadStringTaskAsync(onlinePlayersAddress);
            }
            else
            {
                on_off.Content = "Offline";
                on_off.Foreground = Brushes.Red;
                onlinePlayer.Content = "0";
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.Default.reg_link);
        }

        private void ForumButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.Default.forum_link);
        }

        private void LkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.Default.acc_link);
        }

        private void BnetButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.Default.site_link);
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Wow.FileName.WOW_GAME_EXE_NAME))
            {
                playButton.Content = "STARTING";
                playButton.IsEnabled = false;

                var process = new Process();
                process.StartInfo.FileName = Wow.FileName.WOW_GAME_EXE_NAME;
                //process.StartInfo.UseShellExecute = false;
                process.Start();
                AuthData authData = await Task<AuthData>.Factory.StartNew(() =>
                    {
                        Thread.Sleep(8000);
                        return DataSerializer<AuthData>.LoadObject(LocalConfiguration.Instance.Files.AuthDataFile);
                    });

                SendKeys.SendWait(authData.Login);
                SendKeys.SendWait("{Tab}");
                SendKeys.SendWait(authData.Password);
                SendKeys.SendWait("{enter}");
                Close();
            }
            else
                MessageBox.Show(this,
                    string.Format("File {0} not found. Place the application in the folder containing the client",
                        Wow.FileName.WOW_GAME_EXE_NAME));
        }

        private void CacheDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string cache = LocalConfiguration.Instance.Folders.GetPath(WowLauncherFolder.Cache);
            if (Directory.Exists(cache))
            {
                Directory.Delete(cache, true);
                MessageBox.Show(this, string.Format("Filder {0} successfully removed", AppFolder.CACHE_FOLDER_NAME), "Success");
            }
            else
            {
                MessageBox.Show(this, string.Format("Folder {0} not found", AppFolder.CACHE_FOLDER_NAME), "Error");
            }
        }

        private void RealmListButton_Click(object sender, RoutedEventArgs e)
        {
            string localeFolder = Path.Combine(Wow.FolderName.Client.DATA_FOLDER_NAME, Settings.Default.lang_wow);
            string realmListFile = Path.Combine(localeFolder, "realmlist.wtf");

            if (Directory.Exists(localeFolder))
            {
                using (var writer = new StreamWriter(realmListFile))
                {
                    writer.WriteLine(Settings.Default.realmlist);
                    MessageBox.Show(this, "Server added to realmlist", "Realmlist");
                }
            }
            else
            {
                MessageBox.Show(this,
                    "Error write file realmlist.\nPlease contact technical support for the project.",
                    "Error write Realmlist");
            }
        }

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOnlinePlayersCounter(_addressSet.OnlinePlayers, _worldConfig.Ip, _worldConfig.Port);
            var statistic = new OnlineStat();
            statistic.Owner = this;
            statistic.Show();
        }

        #region Методы обновления UI (потокобезопасные)

        private void SetProgressBarMaxValue(int maxValue)
        {
            if (Dispatcher.CheckAccess()) //Обращаемся ли мы к UI из не UI потока
            {
                progressBar.Maximum = maxValue;
            }
            else
            {
                Dispatcher.Invoke((Action<int>)SetProgressBarMaxValue, maxValue);
            }
        }

        private void SetProgressBarValue(int newValue)
        {
            if (Dispatcher.CheckAccess()) //Обращаемся ли мы к UI из не UI потока
            {
                progressBar.Value = newValue;
            }
            else
            {
                Dispatcher.Invoke((Action<int>)SetProgressBarValue, newValue);
            }
        }

        private void IncrementProgressBarValue()
        {
            if (Dispatcher.CheckAccess()) //Обращаемся ли мы к UI из не UI потока
            {
                progressBar.Value++;
            }
            else
            {
                Dispatcher.Invoke((Action)IncrementProgressBarValue);
            }
        }

        private void UpdateResultText(string text)
        {
            if (Dispatcher.CheckAccess()) //Обращаемся ли мы к UI из не UI потока
            {
                newsAvailible.Text = "Download file: " + text;
            }
            else
            {
                Dispatcher.Invoke((Action<string>)UpdateResultText, text);
            }
        }

        private void ChangePlayButtonEnableState(bool state)
        {
            if (Dispatcher.CheckAccess()) //Обращаемся ли мы к UI из не UI потока
            {
                playButton.IsEnabled = state;
            }
            else
            {
                Dispatcher.Invoke((Action<bool>)ChangePlayButtonEnableState, state);
            }
        }

        #endregion

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

        private void SettinsButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            settings.Owner = this;
            settings.Show();
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseOnWindow)
            {
                Point mouse = e.GetPosition(this);
                _translate.X = ActualHeight - (mouse.X / 120) - ActualHeight;
                _translate.Y = ActualWidth - (mouse.Y / 120) - ActualWidth;


                _translate2.X = ActualHeight - (mouse.X / 400) - ActualHeight;
                _translate2.Y = ActualWidth - (mouse.Y / 400) - ActualWidth;

                _translate1.X = ActualHeight + (mouse.X / 700) - ActualHeight;
                _translate1.Y = ActualWidth + (mouse.Y / 700) - ActualWidth;

                _translate3.X = ActualHeight + (mouse.X / 200) - ActualHeight;
                _translate3.Y = ActualWidth + (mouse.Y / 200) - ActualWidth;

                _lastMousePos = mouse;

            }
            _mouseOnWindow = true;
        }
    }
}