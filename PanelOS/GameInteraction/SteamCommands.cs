using System.IO;
using System.Windows;
using System.Diagnostics;
using PanelOS.Properties;
using Gameloop.Vdf;
using PanelOS.Views;

namespace PanelOS.GameInteraction
{
    public static class SteamCommands
    {
        public static dynamic GetLoginusers()
        {
            string loginusersPath = Settings.Default.SteamFolder + "\\config\\loginusers.vdf";

            if (!File.Exists(loginusersPath))
            {
                NotificationView notification = new NotificationView(loginusersPath + " not found");
                notification.ShowDialog();
                return null;
            }

            FileStream fileStream = new FileStream(loginusersPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            string fileContent = streamReader.ReadToEnd();
            dynamic vdf = VdfConvert.Deserialize(fileContent);

            streamReader.Close();
            fileStream.Close();

            return vdf.Value;
        }

        public static void StartSteam(string password)
        {
            string steamPath = Settings.Default.SteamFolder + "\\Steam.exe";

            if (!File.Exists(steamPath))
            {
                NotificationView notification = new NotificationView(steamPath + " not found");
                notification.ShowDialog();
                return;
            }

            Process steamProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C \"" + Settings.Default.SteamFolder + "\\Steam.exe" + "\""
            };

            Clipboard.SetText(password);
            steamProcess.StartInfo = startInfo;
            steamProcess.Start();
        }
    }
}