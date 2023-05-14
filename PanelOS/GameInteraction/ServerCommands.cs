using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using PanelOS.Models;
using PanelOS.Properties;

namespace PanelOS.GameInteraction
{
    public static class ServerCommands
    {
        #region Commands

        public static void StartAccount(Account account, int windowX, int windowY, string exec)
        {
            if (!NamedPipeExists(account.Login))
            {
                string serverPath = Directory.GetCurrentDirectory() + "\\launcher\\Launcher.exe";
                string steamFolderPath = Settings.Default.SteamFolder + "\\";
                Process server = new Process();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = "/C \"\"" + serverPath + "\" \"" + account.Login + "\" \"" + account.Password + "\" " +
                        account.SteamUserId + " " + windowX + " " + windowY + " " + exec + " \"" + steamFolderPath + "\""
                };

                server.StartInfo = startInfo;
                server.Start();
            }
            else
            {
                Pipe(account.Login, "Start");
            }
        }

        public static void RestartAccount(string login)
        {
            if (NamedPipeExists(login))
                Pipe(login, "Restart");
        }

        public static void StopAccount(string login)
        {
            if (NamedPipeExists(login))
            {
                Pipe(login, "Stop");
                Pipe(login, "Quit");
            }
        }

        public static void StopAllAccounts()
        {
            try
            {
                foreach (Process csGoProcess in Process.GetProcesses().Where(p => p.ProcessName == "csgo"))
                    csGoProcess.Kill();
            }
            catch (Exception) { }

            try
            {
                foreach (Process steamProcess in Process.GetProcesses().Where(p => p.ProcessName.StartsWith("Steam_")))
                    steamProcess.Kill();
            }
            catch (Exception) { }

            try
            {
                foreach (var serverProcess in Process.GetProcesses().Where(p => p.ProcessName == "Launcher"))
                    serverProcess.Kill();
            }
            catch (Exception) { }
        }

        #endregion

        #region Infrastructure

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        private static void Pipe(string name, string command)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", name, PipeDirection.InOut))
            {
                using (StreamWriter streamWriter = new StreamWriter(pipeClient))
                {
                    pipeClient.Connect();
                    streamWriter.Write(command);
                    streamWriter.Dispose();
                    pipeClient.Dispose();
                }
            }
        }

        private static bool NamedPipeExists(string pipeName)
        {
            try
            {
                int timeout = 0;
                string normalizedPath = Path.GetFullPath(string.Format(@"\\.\pipe\{0}", pipeName));
                bool exists = WaitNamedPipe(normalizedPath, timeout);

                if (!exists)
                {
                    int error = Marshal.GetLastWin32Error();

                    if (error == 0)
                        return false;
                    else if (error == 2)
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}