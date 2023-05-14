using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace PanelOS.Views
{
    public struct WindowRect
    {
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
    }

    public partial class ClickerView : Window
    {
        private string CsGoPath;
        private List<Process> CsGoProcesses;
        private BackgroundWorker AutoAcceptWorker;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        public ClickerView()
        {
            InitializeComponent();

            CsGoPath = Properties.Settings.Default.CsGoFolder + "\\";
            CsGoProcesses = new List<Process>();

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            RepeatTimesTextBox.Text = Properties.Settings.Default.RepeatTimes.ToString();
            LoadingDelayTextBox.Text = Properties.Settings.Default.LoadingDelayTime.ToString();
            ReconnectDelayTextBox.Text = Properties.Settings.Default.ReconnectDelayTime.ToString();

            WriteToLogsTextBox("Clicker loaded", System.Windows.Media.Brushes.Green);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Top;
        }

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref WindowRect rectangle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static void sendKey(IntPtr hwnd, int keyCode, bool extended)
        {
            uint scanCode = MapVirtualKey((uint)keyCode, 0);
            uint lParam;

            lParam = 0x00000001 | (scanCode << 16);

            if (extended)
                lParam |= 0x01000000;

            PostMessage(hwnd, WM_KEYDOWN, (IntPtr)keyCode, (IntPtr)lParam);
            PostMessage(hwnd, WM_KEYUP, (IntPtr)keyCode, (IntPtr)lParam);
        }

        public Bitmap CaptureFromScreen(Rectangle bounds)
        {
            if (bounds == Rectangle.Empty)
                bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Bitmap captruredArea = new Bitmap(bounds.Width, bounds.Height);
            Graphics graphics = Graphics.FromImage(captruredArea);

            graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            graphics.Dispose();

            return captruredArea;
        }

        public Color GetPixelColor(System.Drawing.Point p)
        {
            Rectangle rectangle = new Rectangle(p, new System.Drawing.Size(1, 1));
            Bitmap screenshot = CaptureFromScreen(rectangle);

            Color color = screenshot.GetPixel(0, 0);
            screenshot.Dispose();

            return color;
        }

        public void LeftMouseClick(int xpos, int ypos)
        {
            Thread.Sleep(200);
            SetCursorPos(xpos, ypos);

            Thread.Sleep(200);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);

            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void WriteToLogsTextBox(string text, System.Windows.Media.Brush brushColor)
        {
            TextRange textRange = new TextRange(LogsTextBox.Document.ContentEnd, LogsTextBox.Document.ContentEnd);
            textRange.Text = DateTime.Now.ToString("HH:mm:ss") + ": " + text + "\n";
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, brushColor);
            LogsTextBox.ScrollToEnd();
        }

        private string ReadMatchIdFromLogFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string logs = streamReader.ReadToEnd();
                    return Regex.Match(logs, "Received Steam datagram ticket for server steamid:" +
                        "([0-9]+) vport ([0-9]+). match_id=([0-9]+)").Groups[3].Value;
                }
            }
        }

        private void ClearLogFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    streamWriter.Write("");
            }
        }

        private void AutoAcceptWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(3500);

            ProcessFailedAccepts();
            ClickGoIfNotClicked("LEADER #1");
            ClickGoIfNotClicked("LEADER #2");

            if (AutoAcceptWorker.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }

            string matchId1 = "0";
            string matchId2 = "1";

            string leader1_MatchId = ReadMatchIdFromLogFile(CsGoPath + "csgo\\log\\1.log");
            string leader2_MatchId = ReadMatchIdFromLogFile(CsGoPath + "csgo\\log\\2.log");

            if (leader1_MatchId != "")
                matchId1 = leader1_MatchId;

            if (leader2_MatchId != "")
                matchId2 = leader2_MatchId;

            LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("ID 1 : " + leader1_MatchId, System.Windows.Media.Brushes.White));
            LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("ID 2 : " + leader2_MatchId, System.Windows.Media.Brushes.White));

            if (leader1_MatchId == leader2_MatchId && leader1_MatchId != "" && leader2_MatchId != "")
                LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Equals", System.Windows.Media.Brushes.Green));
            else
                LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Not equals", System.Windows.Media.Brushes.Red));

            ClearLogFile(CsGoPath + "csgo\\log\\1.log");
            ClearLogFile(CsGoPath + "csgo\\log\\2.log");

            LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Logs cleared", System.Windows.Media.Brushes.White));

            if (matchId1 == matchId2)
            {
                LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Starting accept clicks", System.Windows.Media.Brushes.AliceBlue));
                Thread.Sleep(3500);

                int acceptX = 0, acceptY = 0;

                foreach (Process csgoProcess in CsGoProcesses)
                {
                    if (AutoAcceptWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    IntPtr csgoWindowDescriptor = csgoProcess.MainWindowHandle;
                    WindowRect csgoWindowPosition = new WindowRect();
                    GetWindowRect(csgoWindowDescriptor, ref csgoWindowPosition);

                    if (csgoProcess.MainWindowTitle.EndsWith(" | BOT"))
                        acceptX = csgoWindowPosition.TopLeftX + 180;
                    else if (csgoProcess.MainWindowTitle.Contains(" | LEADER #"))
                        acceptX = csgoWindowPosition.TopLeftX + 390;

                    acceptY = csgoWindowPosition.TopLeftY + 200;

                    Color acceptColor = GetPixelColor(new System.Drawing.Point(acceptX, acceptY));

                    if (acceptColor.R > 70 && acceptColor.R < 80 &&
                        acceptColor.G > 170 && acceptColor.G < 180 &&
                        acceptColor.B > 75 && acceptColor.B < 85)
                    {
                        LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Accept at " + acceptX + "," + acceptY, System.Windows.Media.Brushes.Green));
                        LeftMouseClick(acceptX, acceptY);
                        Thread.Sleep(500);
                    };
                }

                LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Clicker stopped", System.Windows.Media.Brushes.Orange));

                if (AutoAcceptWorker != null)
                {
                    AutoAcceptWorker.CancelAsync();
                    e.Cancel = true;
                }
            }
        }

        private void ProcessFailedAccepts()
        {
            Thread.Sleep(500);

            int failedToAcceptX = 0, failedToAcceptY = 0;

            foreach (Process csgoProcess in CsGoProcesses)
            {
                if (AutoAcceptWorker.CancellationPending == true)
                    return;

                IntPtr csgoWindowDescriptor = csgoProcess.MainWindowHandle;
                WindowRect csgoWindowPosition = new WindowRect();
                GetWindowRect(csgoWindowDescriptor, ref csgoWindowPosition);

                if (csgoProcess.MainWindowTitle.EndsWith(" | BOT"))
                    failedToAcceptX = csgoWindowPosition.TopLeftX + 285;
                else if (csgoProcess.MainWindowTitle.Contains(" | LEADER #"))
                    failedToAcceptX = csgoWindowPosition.TopLeftX + 500;

                failedToAcceptY = csgoWindowPosition.TopLeftY + 195;

                Color color = GetPixelColor(new System.Drawing.Point(failedToAcceptX - 35, failedToAcceptY - 3));

                if (color.R > 40 && color.R < 50 &&
                    color.G > 40 && color.G < 50 &&
                    color.B > 40 && color.B < 50)
                {
                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox(
                        "Confirm at " + failedToAcceptX + "," + failedToAcceptY, System.Windows.Media.Brushes.Yellow));

                    LeftMouseClick(failedToAcceptX, failedToAcceptY);
                    Thread.Sleep(500);
                };
            }
        }

        private void ClickGoIfNotClicked(string player)
        {
            Thread.Sleep(500);

            if (AutoAcceptWorker.CancellationPending == true)
                return;

            Process csGoLeaderProcess = CsGoProcesses.FirstOrDefault(p => p.MainWindowTitle.EndsWith(" | " + player));

            if (csGoLeaderProcess != null)
            {
                IntPtr playerWindowDescriptor = csGoLeaderProcess.MainWindowHandle;
                WindowRect playerWindowPosition = new WindowRect();
                GetWindowRect(playerWindowDescriptor, ref playerWindowPosition);

                Color leaderAcceptColor = GetPixelColor(new System.Drawing.Point(
                    playerWindowPosition.TopLeftX + 390, playerWindowPosition.TopLeftY + 200));

                if (!(leaderAcceptColor.R > 70 && leaderAcceptColor.R < 80 &&
                    leaderAcceptColor.G > 170 && leaderAcceptColor.G < 180 &&
                    leaderAcceptColor.B > 75 && leaderAcceptColor.B < 85))
                {
                    Color cancelSearchColor = GetPixelColor(new System.Drawing.Point(
                        playerWindowPosition.BottomRightX - 135, playerWindowPosition.BottomRightY - 15));

                    if (!(cancelSearchColor.R > 40 && cancelSearchColor.R < 60 &&
                        cancelSearchColor.G < 5 && cancelSearchColor.B < 5))
                    {
                        LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox(
                            "Click [GO] at " + (playerWindowPosition.BottomRightX - 135) + "," +
                            (playerWindowPosition.BottomRightY - 15), System.Windows.Media.Brushes.Yellow));

                        LeftMouseClick(playerWindowPosition.BottomRightX - 135, playerWindowPosition.BottomRightY - 15);
                        Thread.Sleep(500);
                    };
                }
            }
        }

        private void AutoAcceptWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AutoAcceptWorker = null;
                StartAutoAcceptButton.IsEnabled = true;
                StopAutoAcceptButton.IsEnabled = false;
                return;
            }

            AutoAcceptWorker.RunWorkerAsync();
        }

        private void StartAutoAcceptButton_Click(object sender, RoutedEventArgs e)
        {
            StartAutoAcceptButton.IsEnabled = false;
            StopAutoAcceptButton.IsEnabled = true;

            if (Directory.Exists(CsGoPath + "csgo"))
            {
                if (!Directory.Exists(CsGoPath + "csgo\\log"))
                    Directory.CreateDirectory(CsGoPath + "csgo\\log");

                if (!File.Exists(CsGoPath + "csgo\\log\\1.log"))
                    File.Create(CsGoPath + "csgo\\log\\1.log");
                else
                    ClearLogFile(CsGoPath + "csgo\\log\\1.log");

                if (!File.Exists(CsGoPath + "csgo\\log\\2.log"))
                    File.Create(CsGoPath + "csgo\\log\\2.log");
                else
                    ClearLogFile(CsGoPath + "csgo\\log\\2.log");

                AutoAcceptWorker = new BackgroundWorker();
                AutoAcceptWorker.WorkerSupportsCancellation = true;
                AutoAcceptWorker.DoWork += new DoWorkEventHandler(AutoAcceptWorker_DoWork);
                AutoAcceptWorker.RunWorkerCompleted += AutoAcceptWorker_RunWorkerCompleted;

                CsGoProcesses = Process.GetProcesses().Where(p => p.ProcessName == "csgo" && p.MainWindowTitle.Contains("ACCOUNT")).ToList();

                if (CsGoProcesses.Count == 0)
                {
                    WriteToLogsTextBox("No CS:GO launched games found", System.Windows.Media.Brushes.Red);
                    StartAutoAcceptButton.IsEnabled = true;
                    StopAutoAcceptButton.IsEnabled = false;
                    return;
                }

                if (!CsGoProcesses.Any(p => p.MainWindowTitle.Contains(" | LEADER")))
                {
                    WriteToLogsTextBox("No leaders found", System.Windows.Media.Brushes.Red);
                    StartAutoAcceptButton.IsEnabled = true;
                    StopAutoAcceptButton.IsEnabled = false;
                    return;
                }

                ClickGoIfNotClicked("LEADER #1");
                ClickGoIfNotClicked("LEADER #2");

                AutoAcceptWorker.RunWorkerAsync();
            }
        }

        private void StopAutoAcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutoAcceptWorker != null)
                AutoAcceptWorker.CancelAsync();

            StartAutoAcceptButton.IsEnabled = true;
            StopAutoAcceptButton.IsEnabled = false;
        }

        private bool ConnectionParametersAreCorrect()
        {
            if (string.IsNullOrWhiteSpace(RepeatTimesTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoadingDelayTextBox.Text) ||
                string.IsNullOrWhiteSpace(ReconnectDelayTextBox.Text))
                return false;

            if (RepeatTimesTextBox.Text.Any(character => !char.IsDigit(character)) ||
                LoadingDelayTextBox.Text.Any(character => !char.IsDigit(character)) ||
                ReconnectDelayTextBox.Text.Any(character => !char.IsDigit(character)))
                return false;

            if (int.TryParse(RepeatTimesTextBox.Text, out int repeatTimes) &&
                int.TryParse(LoadingDelayTextBox.Text, out int loadingDelayTime) &&
                int.TryParse(ReconnectDelayTextBox.Text, out int reconnectDelayTime))
            {
                if (repeatTimes < 1 || loadingDelayTime < 1 || reconnectDelayTime < 1)
                    return false;
            }

            return true;
        }

        private void StartAutoConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ConnectionParametersAreCorrect())
            {
                WriteToLogsTextBox("Incorrect repeat and delay parameters", System.Windows.Media.Brushes.Red);
                return;
            }

            int repeatTimes = int.Parse(RepeatTimesTextBox.Text);
            int loadingDelayTime = int.Parse(LoadingDelayTextBox.Text) * 1000;
            int reconnectDelayTime = int.Parse(ReconnectDelayTextBox.Text) * 1000;

            Properties.Settings.Default.RepeatTimes = repeatTimes;
            Properties.Settings.Default.LoadingDelayTime = loadingDelayTime / 1000;
            Properties.Settings.Default.ReconnectDelayTime = reconnectDelayTime / 1000;
            Properties.Settings.Default.Save();

            CsGoProcesses = Process.GetProcesses().Where(p => p.ProcessName == "csgo").ToList();

            if (CsGoProcesses.Count == 0)
            {
                WriteToLogsTextBox("No CS:GO launched games found", System.Windows.Media.Brushes.Red);
                return;
            }

            StartAutoConnectionButton.IsEnabled = false;
            StopAutoConnectionButton.IsEnabled = true;

            DisconnectTop.IsEnabled = false;
            DisconnectBottom.IsEnabled = false;

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            bool disconnectTop = DisconnectTop.IsChecked.Value;

            Task startAccountsTask = new Task(async () =>
            {
                for (int i = 0; i < repeatTimes; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Starting round " + (i + 1), System.Windows.Media.Brushes.Aqua));

                    // Disconnect

                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Starting disconnects", System.Windows.Media.Brushes.Green));
                    await Task.Delay(500);

                    foreach (Process csGoProcess in CsGoProcesses)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        IntPtr csGoWindowDescriptor = csGoProcess.MainWindowHandle;
                        WindowRect csGoWindowPosition = new WindowRect();
                        GetWindowRect(csGoWindowDescriptor, ref csGoWindowPosition);

                        if (disconnectTop)
                        {
                            if (csGoWindowPosition.TopLeftY < 300 || csGoWindowPosition.TopLeftY > 650)
                            {
                                sendKey(csGoProcess.MainWindowHandle, (int)System.Windows.Forms.Keys.F3, false);
                                Thread.Sleep(50);
                                sendKey(csGoProcess.MainWindowHandle, (int)System.Windows.Forms.Keys.F11, false);
                                Thread.Sleep(50);
                            }
                        }
                        else
                        {
                            if (csGoWindowPosition.TopLeftY >= 300)
                            {
                                sendKey(csGoProcess.MainWindowHandle, (int)System.Windows.Forms.Keys.F3, false);
                                Thread.Sleep(50);
                                sendKey(csGoProcess.MainWindowHandle, (int)System.Windows.Forms.Keys.F11, false);
                                Thread.Sleep(50);
                            }
                        }
                    }

                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Disconnects completed", System.Windows.Media.Brushes.Green));

                    // Reconnect

                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Waiting before reconnect for " + 
                        reconnectDelayTime / 1000 + " sec", System.Windows.Media.Brushes.White));

                    await Task.Delay(reconnectDelayTime);

                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Starting reconnects", System.Windows.Media.Brushes.Green));

                    foreach (Process csGoProcess in CsGoProcesses)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        IntPtr csGoWindowDescriptor = csGoProcess.MainWindowHandle;
                        WindowRect csGoWindowPosition = new WindowRect();
                        GetWindowRect(csGoWindowDescriptor, ref csGoWindowPosition);

                        if (disconnectTop)
                        {
                            if (csGoWindowPosition.TopLeftY < 300 || csGoWindowPosition.TopLeftY > 650)
                            {
                                // Check if matchmaking failed

                                System.Drawing.Point matchmakingFailedPosition = new System.Drawing.Point(
                                    csGoWindowPosition.TopLeftX + 200, csGoWindowPosition.TopLeftY + 200);

                                Color matchmakingFailedColor = GetPixelColor(matchmakingFailedPosition);

                                if (matchmakingFailedColor.R > 40 && matchmakingFailedColor.R < 50 &&
                                    matchmakingFailedColor.G > 40 && matchmakingFailedColor.G < 50 &&
                                    matchmakingFailedColor.B > 40 && matchmakingFailedColor.B < 50)
                                {
                                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Confirming at " +
                                        (csGoWindowPosition.TopLeftX + 250) + "," + (csGoWindowPosition.TopLeftY + 195), System.Windows.Media.Brushes.Yellow));

                                    LeftMouseClick(csGoWindowPosition.TopLeftX + 250, csGoWindowPosition.TopLeftY + 195);
                                    await Task.Delay(1000);
                                }

                                System.Drawing.Point reconnectPosition = new System.Drawing.Point();
                                reconnectPosition.X = csGoWindowPosition.BottomRightX - 110;
                                reconnectPosition.Y = csGoWindowPosition.TopLeftY + 38;

                                Color reconnectColor = GetPixelColor(reconnectPosition);

                                if (reconnectColor.R > 70 && reconnectColor.R < 80 &&
                                    reconnectColor.G > 170 && reconnectColor.G < 180 &&
                                    reconnectColor.B > 75 && reconnectColor.B < 85)
                                {
                                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Click [RECONNECT] at " +
                                        reconnectPosition.X + "," + reconnectPosition.Y, System.Windows.Media.Brushes.Yellow));

                                    LeftMouseClick(reconnectPosition.X, reconnectPosition.Y);
                                    await Task.Delay(1000);
                                }
                            }
                        }
                        else
                        {
                            if (csGoWindowPosition.TopLeftY >= 300)
                            {
                                // Check if matchmaking failed

                                System.Drawing.Point matchmakingFailedPosition = new System.Drawing.Point(
                                    csGoWindowPosition.TopLeftX + 200, csGoWindowPosition.TopLeftY + 200);

                                Color matchmakingFailedColor = GetPixelColor(matchmakingFailedPosition);

                                if (matchmakingFailedColor.R > 40 && matchmakingFailedColor.R < 50 &&
                                    matchmakingFailedColor.G > 40 && matchmakingFailedColor.G < 50 &&
                                    matchmakingFailedColor.B > 40 && matchmakingFailedColor.B < 50)
                                {
                                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Confirming at " +
                                        (csGoWindowPosition.TopLeftX + 250) + "," + (csGoWindowPosition.TopLeftY + 195), System.Windows.Media.Brushes.Yellow));

                                    LeftMouseClick(csGoWindowPosition.TopLeftX + 250, csGoWindowPosition.TopLeftY + 195);
                                    await Task.Delay(1000);
                                }

                                System.Drawing.Point reconnectPosition = new System.Drawing.Point();
                                reconnectPosition.X = csGoWindowPosition.BottomRightX - 110;
                                reconnectPosition.Y = csGoWindowPosition.TopLeftY + 38;

                                Color reconnectColor = GetPixelColor(reconnectPosition);

                                if (reconnectColor.R > 70 && reconnectColor.R < 80 &&
                                    reconnectColor.G > 170 && reconnectColor.G < 180 &&
                                    reconnectColor.B > 75 && reconnectColor.B < 85)
                                {
                                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Click [RECONNECT] at " +
                                        reconnectPosition.X + "," + reconnectPosition.Y, System.Windows.Media.Brushes.Yellow));

                                    LeftMouseClick(reconnectPosition.X, reconnectPosition.Y);
                                    await Task.Delay(1000);
                                }
                            }

                        }
                    }

                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Reconnects completed", System.Windows.Media.Brushes.Green));
                    LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Waiting for " + loadingDelayTime/1000 + " sec", System.Windows.Media.Brushes.White));
                    await Task.Delay(loadingDelayTime);
                }

                LogsTextBox.Dispatcher.Invoke(() => WriteToLogsTextBox("Autoconnects completed", System.Windows.Media.Brushes.Green));

                Dispatcher.Invoke(() => {
                    StartAutoConnectionButton.IsEnabled = true;
                    StopAutoConnectionButton.IsEnabled = false;

                    DisconnectTop.IsEnabled = true;
                    DisconnectBottom.IsEnabled = true;
                });

            }, cancellationToken);

            startAccountsTask.Start();
        }

        private void StopAutoConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            StartAutoConnectionButton.IsEnabled = true;
            StopAutoConnectionButton.IsEnabled = false;

            DisconnectTop.IsEnabled = true;
            DisconnectBottom.IsEnabled = true;

            WriteToLogsTextBox("Autoconnects stopped", System.Windows.Media.Brushes.Green);
            cancellationTokenSource.Cancel();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (AutoAcceptWorker != null)
                AutoAcceptWorker.CancelAsync();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}