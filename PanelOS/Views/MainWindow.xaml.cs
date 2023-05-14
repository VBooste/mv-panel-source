using PanelOS.ViewModels;
using PanelOS.Views;
using System.Windows;
using System.Windows.Input;
using PanelOS.Properties;
using System.Linq;
using System.Collections.Generic;
using PanelOS.Models;
using System.Windows.Controls;
using System;
using PanelOS.GameInteraction;

namespace PanelOS
{
    public partial class MainWindow : Window
    {
        private bool BoostingComboboxChangingDisabled;
        private bool CalibrationComboboxChangingDisabled;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LauncherViewModel();

            if(!SettingsAreSet())
            {
                SettingsView settingsView = new SettingsView();
                settingsView.ShowDialog();

                if(settingsView.DialogResult != true)
                    Close();
            }
        }

        private bool SettingsAreSet()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.SteamFolder) ||
                string.IsNullOrWhiteSpace(Settings.Default.CsGoFolder) ||
                string.IsNullOrWhiteSpace(Settings.Default.CsGoWindowX.ToString()) ||
                string.IsNullOrWhiteSpace(Settings.Default.CsGoWindowX.ToString()))
                return false;

            return true;
        }

        private void CalibrationButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherTabControl.SelectedIndex = 1;
        }

        private void BoostButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherTabControl.SelectedIndex = 2;
        }

        private void CommunityButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/mv0642");
        }

        private void FAQButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://csgomv.ru/");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView settingsView = new SettingsView();
            settingsView.ShowDialog();
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }        

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void AccountColorsFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<int> selectedIndices = new List<int>();

            foreach (var item in AccountColorsFilter.SelectedItems)
                selectedIndices.Add(AccountColorsFilter.Items.IndexOf(item));

            if (selectedIndices.Count > 1 && selectedIndices.Contains(0))
                AccountColorsFilter.SelectedItems.RemoveAt(0);

            if (AccountColorsFilter.SelectedItems.Count == 0)
                AccountColorsFilter.SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClickerView clickerWindow = Application.Current.Windows.OfType<ClickerView>().SingleOrDefault();

            if (clickerWindow != null)
                clickerWindow.Close();

            LauncherViewModel context = (LauncherViewModel)DataContext;

            if (context.Accounts.Any(acc => acc.Active))
            {
                string message = "All started accounts will be closed. Quit?";
                MessageBoxResult result = MessageBox.Show(message, "Close", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    context.StopAllAccountsCommand.Execute(null);
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AccountColorPopup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var accountColorPopup = sender as ListBox;

            if (accountColorPopup.SelectedItems.Count == 0)
                accountColorPopup.SelectedIndex = 0;
        }

        private async void BoostingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoostingComboboxChangingDisabled)
            {
                BoostingComboboxChangingDisabled = false;
                return;
            }

            if (e.RemovedItems.Count > 0)
            {
                LauncherViewModel viewModel = DataContext as LauncherViewModel;

                BoostingLobby activeBoostingLobby = viewModel.BoostingLobbies.FirstOrDefault(bl => bl.Active);

                if (activeBoostingLobby == null)
                    return;

                Account oldAccount = e.RemovedItems[0] as Account;
                Account newAccount = e.AddedItems[0] as Account;

                if (activeBoostingLobby.Players.Count(p => p == newAccount.AccountId) > 1)
                {
                    viewModel.LauncherMessageQueue.Enqueue("No duplicated accounts allowed");
                    BoostingComboboxChangingDisabled = true;
                    (sender as ComboBox).SelectedValue = oldAccount.AccountId;
                    return;
                }

                int x = 0; int y = 0;
                int playerIndex = Array.IndexOf(activeBoostingLobby.Players, newAccount.AccountId);

                if (playerIndex == 1 || playerIndex == 5 || playerIndex == 9)
                    x = 425;
                else if (playerIndex == 2 || playerIndex == 6)
                    x = 850;
                else if (playerIndex == 3 || playerIndex == 7)
                    x = 1275;

                if (playerIndex > 7)
                    y = 700;
                else if (playerIndex > 3)
                    y = 350;

                oldAccount.Active = false;
                newAccount.Active = true;

                await System.Threading.Tasks.Task.Delay(1000);
                ServerCommands.StopAccount(oldAccount.Login);

                await System.Threading.Tasks.Task.Delay(1000);

                FileSystemCommands.WriteCsGoWindowTitle(newAccount.SteamUserId,
                    "	game	\"@ ACCOUNT: " + newAccount.SteamUserId + " | LOGIN: " + newAccount.Login + " | BOT\"");

                ServerCommands.StartAccount(newAccount, x, y, "cfg_boost");

                viewModel.DbContext.SaveChanges();
            }
        }

        private async void CalibrationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalibrationComboboxChangingDisabled)
            {
                CalibrationComboboxChangingDisabled = false;
                return;
            }

            if (e.RemovedItems.Count > 0)
            {
                LauncherViewModel viewModel = DataContext as LauncherViewModel;

                CalibrationLobby activeCalibrationLobby = viewModel.CalibrationLobbies.FirstOrDefault(bl => bl.Active);

                if (activeCalibrationLobby == null)
                    return;

                Account oldAccount = e.RemovedItems[0] as Account;
                Account newAccount = e.AddedItems[0] as Account;

                if (activeCalibrationLobby.Players.Count(p => p == newAccount.AccountId) > 1)
                {
                    viewModel.LauncherMessageQueue.Enqueue("No duplicated accounts allowed");
                    CalibrationComboboxChangingDisabled = true;
                    (sender as ComboBox).SelectedValue = oldAccount.AccountId;
                    return;
                }

                int x = 0; int y = 0;
                int playerIndex = Array.IndexOf(activeCalibrationLobby.Players, newAccount.AccountId);
                
                if (playerIndex == 3 || playerIndex == 7)
                    x = 425;
                else if (playerIndex == 1 || playerIndex == 4 || playerIndex == 8)
                    x = 850;
                else if (playerIndex == 5 || playerIndex == 9)
                    x = 1275;

                if (playerIndex < 2)
                    y = 350;
                else if (playerIndex > 5)
                    y = 700;               

                oldAccount.Active = false;
                newAccount.Active = true;

                await System.Threading.Tasks.Task.Delay(1000);
                ServerCommands.StopAccount(oldAccount.Login);

                await System.Threading.Tasks.Task.Delay(1000);

                if (playerIndex == 0)
                {
                    FileSystemCommands.WriteCsGoWindowTitle(newAccount.SteamUserId, 
                        "	game	\"@ ACCOUNT: " + newAccount.SteamUserId + " | LOGIN: " + newAccount.Login + " | LEADER #1\"");

                    ServerCommands.StartAccount(newAccount, x, y, "cfg_leader1");
                }
                if (playerIndex == 1)
                {
                    FileSystemCommands.WriteCsGoWindowTitle(newAccount.SteamUserId, 
                        "	game	\"@ ACCOUNT: " + newAccount.SteamUserId + " | LOGIN: " + newAccount.Login + " | LEADER #2\"");

                    ServerCommands.StartAccount(newAccount, x, y, "cfg_leader2");
                }
                else
                {
                    FileSystemCommands.WriteCsGoWindowTitle(newAccount.SteamUserId, 
                        "	game	\"@ ACCOUNT: " + newAccount.SteamUserId + " | LOGIN: " + newAccount.Login + " | BOT\"");

                    ServerCommands.StartAccount(newAccount, x, y, "cfg_boost");
                }

                viewModel.DbContext.SaveChanges();
            }
        }

        private void ClickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Windows.OfType<ClickerView>().SingleOrDefault() == null)
            {
                ClickerView clickerView = new ClickerView();
                clickerView.Show();
            }
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            LauncherTabControl.Focus();
        }

        private void LauncherTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}