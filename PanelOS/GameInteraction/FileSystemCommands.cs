using System;
using System.IO;
using PanelOS.Properties;
using PanelOS.Views;

namespace PanelOS.GameInteraction
{
    public static class FileSystemCommands
    {
        private static readonly string SteamFolder = Settings.Default.SteamFolder;

        public static void CopySteamExecutable(string userId)
        {
            if (File.Exists(SteamFolder + "\\Steam.exe"))
            {
                if (!File.Exists(SteamFolder + "\\Steam_" + userId + ".exe"))
                    File.Copy(SteamFolder + "\\Steam.exe", SteamFolder + "\\Steam_" + userId + ".exe");
            }
            else
            {
                NotificationView notification = new NotificationView("Steam.exe not found");
                notification.ShowDialog();
            }
        }

        public static void DeleteSteamExecutable(string userId)
        {
            if (File.Exists(SteamFolder + "\\Steam_" + userId + ".exe"))
                File.Delete(SteamFolder + "\\Steam_" + userId + ".exe");
        }

        public static void CopyCsGoDataFolder(string strSource, string strDestination)
        {
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
                DirectoryInfo dirInfo = new DirectoryInfo(strSource);
                FileInfo[] files = dirInfo.GetFiles();

                foreach (FileInfo tempfile in files)
                    tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));

                DirectoryInfo[] directories = dirInfo.GetDirectories();

                foreach (DirectoryInfo tempdir in directories)
                    CopyCsGoDataFolder(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
            }
        }

        public static void WriteCsGoWindowTitle(string userId, string newProperties)
        {
            string fileName = Settings.Default["CsGoFolder"] + "\\csgo_" + userId + "\\gameinfo.txt";
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[2] = newProperties;
            File.WriteAllLines(fileName, arrLine);
        }

        public static void CreateCsGoSubfolders(string userId)
        {
            if (!Directory.Exists(SteamFolder + "\\userdata\\" + userId + "\\"))
                Directory.CreateDirectory(SteamFolder + "\\userdata\\" + userId + "\\");

            if (!Directory.Exists(SteamFolder + "\\userdata\\" + userId + "\\730\\"))
                Directory.CreateDirectory(SteamFolder + "\\userdata\\" + userId + "\\730\\");

            if (!Directory.Exists(SteamFolder + "\\userdata\\" + userId + "\\730\\local\\"))
                Directory.CreateDirectory(SteamFolder + "\\userdata\\" + userId + "\\730\\local\\");

            if (!Directory.Exists(SteamFolder + "\\userdata\\" + userId + "\\730\\local\\cfg\\"))
                Directory.CreateDirectory(SteamFolder + "\\userdata\\" + userId + "\\730\\local\\cfg\\");

            CopyConfigIfNotExists(userId, "data\\userdata\\config.cfg", "\\730\\local\\cfg\\config.cfg");
            CopyConfigIfNotExists(userId, "data\\userdata\\video.txt", "\\730\\local\\cfg\\video.txt");
            CopyConfigIfNotExists(userId, "data\\userdata\\videodefaults.txt", "\\730\\local\\cfg\\videodefaults.txt");
        }

        private static void CopyConfigIfNotExists(string userId, string sourceFile, string destinationFile)
        {
            if (File.Exists(SteamFolder + "\\userdata\\" + userId + destinationFile))
            {
                if (!FileIsReadOnly(SteamFolder + "\\userdata\\" + userId + destinationFile))
                {
                    File.Copy(sourceFile, SteamFolder + "\\userdata\\" + userId + destinationFile, true);
                }
                else
                {
                    SetFileReadAccess(SteamFolder + "\\userdata\\" + userId + destinationFile, false);
                    File.Copy(sourceFile, SteamFolder + "\\userdata\\" + userId + destinationFile, true);
                }

                SetFileReadAccess(SteamFolder + "\\userdata\\" + userId + destinationFile, true);
            }
            else
            {
                File.Copy(sourceFile, SteamFolder + @"\userdata\" + userId + destinationFile, true);
                SetFileReadAccess(SteamFolder + "\\userdata\\" + userId + destinationFile, true);
            }
        }

        public static void WriteCsGoWindowSize(int windowWidth, int windowHeight)
        {
            string cfgPath = "data\\cfg\\";
            string destinationPath = Settings.Default.CsGoFolder + "\\csgo\\cfg\\";

            if (Directory.Exists(cfgPath) && Directory.Exists(destinationPath))
            {
                foreach (var fileInfo in new DirectoryInfo(cfgPath).GetFiles())
                {
                    if (!File.Exists(destinationPath + fileInfo.Name))
                        File.Copy(cfgPath + fileInfo.Name, destinationPath + fileInfo.Name);
                }

                if (File.Exists(destinationPath + "cfg_global.cfg"))
                {                    
                    string[] lines = File.ReadAllLines(destinationPath + "cfg_global.cfg");
                    lines[0] = "mat_setvideomode " + windowWidth + " " + windowHeight + " 1";
                    File.WriteAllLines(destinationPath + "cfg_global.cfg", lines);
                }
            }
        }

        public static void DeleteCsGoSubfolders(string userId)
        {
            if (Directory.Exists(Settings.Default["CsGoFolder"] + "\\csgo_" + userId))
                DeleteDirectory(Settings.Default["CsGoFolder"] + "\\csgo_" + userId);
        }

        private static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
                DeleteDirectory(directory);

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        private static bool FileIsReadOnly(string fileName)
        {
            return new FileInfo(fileName).IsReadOnly;
        }

        private static void SetFileReadAccess(string fileName, bool isReadonly)
        {
            FileInfo fileInfo = new FileInfo(fileName)
            {
                IsReadOnly = isReadonly
            };
        }
    }
}