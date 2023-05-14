using HWIDGen;
using PanelOS.Views;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace PanelOS
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                NotificationView notification = new NotificationView("Program is already running");
                notification.Show();
                await Task.Delay(2000);
                System.Environment.Exit(0);
            }

            string password = await RequestPassword();
            password = password.Trim('\"', '\\');

            if (string.IsNullOrEmpty(password) || password == "false" || password == "No database selected")
            {
                ActivationView activationView = new ActivationView();
                activationView.ShowDialog();
                System.Environment.Exit(0);
            }
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

                if (File.Exists("Userdata.cab"))
                {
                    if (connectionStringsSection.SectionInformation.IsProtected)
                        connectionStringsSection.SectionInformation.UnprotectSection();

                    connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString =
                        "Data Source=.\\Userdata.cab;Password=" + password;

                    if (!connectionStringsSection.SectionInformation.IsProtected)
                        connectionStringsSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");

                    config.Save();
                    ConfigurationManager.RefreshSection("connectionStrings");

                    MainWindow = new MainWindow();
                    MainWindow.Show();
                }
                else
                {
                    if (!connectionStringsSection.SectionInformation.IsProtected)
                        connectionStringsSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");

                    config.Save();
                    ConfigurationManager.RefreshSection("connectionStrings");

                    NotificationView notification = new NotificationView("Your program isn't activated yet");
                    notification.ShowDialog();
                    System.Environment.Exit(0);
                }
            }
        }

        private async Task<string> RequestPassword()
        {
            HWIDGenerator hwidGenerator = new HWIDGenerator();
            string hwid = hwidGenerator.GetHWID();

            Dictionary<string, string> postData = new Dictionary<string, string>
            {
                { "userhwid", hwid }
            };

            HttpClient HTTPClient = new HttpClient();
            FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await HTTPClient.PostAsync("https://csgomv.ru/check/pro.php", content);
            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}