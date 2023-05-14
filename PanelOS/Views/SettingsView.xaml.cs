using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PanelOS.GameInteraction;
using PanelOS.Properties;

namespace PanelOS.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();

            SteamFolderTextBox.Text = Settings.Default.SteamFolder;
            CsGoFolderTextBox.Text = Settings.Default.CsGoFolder;

            CsGoWindowXTextBox.Text = Settings.Default.CsGoWindowX.ToString();
            CsGoWindowYTextBox.Text = Settings.Default.CsGoWindowY.ToString();
        }

        private void SteamFolderOverviewButton_Click(object sender, RoutedEventArgs e)
        {
            using (var steamFolderDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(Settings.Default.SteamFolder))
                    steamFolderDialog.SelectedPath = Settings.Default.SteamFolder;

                steamFolderDialog.ShowDialog();

                if (!string.IsNullOrEmpty(steamFolderDialog.SelectedPath))
                {
                    if (File.Exists(steamFolderDialog.SelectedPath + "\\Steam.exe"))
                        SteamFolderTextBox.Text = steamFolderDialog.SelectedPath;
                    else
                    {
                        NotificationView notification = new NotificationView("Steam.exe not found");
                        notification.ShowDialog();
                    }
                }
            }
        }

        private void CsGoFolderOverview_Click(object sender, RoutedEventArgs e)
        {
            using (var csGoFolderDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(Settings.Default.CsGoFolder))
                    csGoFolderDialog.SelectedPath = Settings.Default.CsGoFolder;

                csGoFolderDialog.ShowDialog();

                if (!string.IsNullOrEmpty(csGoFolderDialog.SelectedPath))
                {
                    if (File.Exists(csGoFolderDialog.SelectedPath + "\\csgo.exe"))
                        CsGoFolderTextBox.Text = csGoFolderDialog.SelectedPath;
                    else
                    {
                        NotificationView notification = new NotificationView("csgo.exe not found");
                        notification.ShowDialog();
                    }
                }
            }
        }

        private void ApplySettingsButton_Click(object sender, RoutedEventArgs e)
        { 
            if (SettingsAreCorrect())
            {
                Settings.Default.SteamFolder = SteamFolderTextBox.Text;
                Settings.Default.CsGoFolder = CsGoFolderTextBox.Text;

                Settings.Default.CsGoWindowX = int.Parse(CsGoWindowXTextBox.Text);
                Settings.Default.CsGoWindowY = int.Parse(CsGoWindowYTextBox.Text);

                FileSystemCommands.WriteCsGoWindowSize(Settings.Default.CsGoWindowX, Settings.Default.CsGoWindowY);
                Settings.Default.Save();
                DialogResult = true;
            }
            else
            {
                NotificationView notification = new NotificationView("Wrong settings");
                notification.ShowDialog();
            }
        }

        private bool SettingsAreCorrect()
        {
            if (string.IsNullOrWhiteSpace(SteamFolderTextBox.Text) ||
                string.IsNullOrWhiteSpace(CsGoFolderTextBox.Text) ||
                string.IsNullOrWhiteSpace(CsGoWindowXTextBox.Text) ||
                string.IsNullOrWhiteSpace(CsGoWindowYTextBox.Text))
                return false;

            if (CsGoWindowXTextBox.Text.Any(character => !char.IsDigit(character)) ||
                CsGoWindowYTextBox.Text.Any(character => !char.IsDigit(character)))
                return false;

            if (int.TryParse(CsGoWindowXTextBox.Text, out int windowX) &&
                int.TryParse(CsGoWindowYTextBox.Text, out int windowY))
            {
                if (windowX < 100 || windowY < 100)
                    return false;
            }

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}