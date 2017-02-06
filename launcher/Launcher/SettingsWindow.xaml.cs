using System.IO;
using System.Windows;
using System.Windows.Input;
using BSU.Utils.Data;
using WowSuite.Launcher.Config;

namespace WowSuite.Launcher
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            SetResultText(string.Empty);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void closeButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetResultText(string text)
        {
            result.Text = text;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var authData = new AuthData(textBoxLogin.Text, textBoxPassword.Text);
                DataSerializer<AuthData>.SaveObject(authData, LocalConfiguration.Instance.Files.AuthDataFile);

                SetResultText("Data saved successfully!");
            }
            catch
            {
                MessageBox.Show(this, "Error saving data. Refer to the technical support of the project.",
                    "AutoLogin");
            }
        }

        private void TextBoxes_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (result.Text.Length > 0)
            {
                result.Text = string.Empty;
            }
        }

        private void LoadData()
        {
            if (File.Exists(LocalConfiguration.Instance.Files.AuthDataFile))
            {
                var authData = DataSerializer<AuthData>.LoadObject(LocalConfiguration.Instance.Files.AuthDataFile);
                textBoxLogin.Text = authData.Login;
                textBoxPassword.Text = authData.Password;
            }
        }
    }
}