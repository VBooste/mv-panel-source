using MaterialDesignThemes.Wpf;
using PanelOS.Helpers;
using PanelOS.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PanelOS.Views
{
    public partial class AddCalibrationLobbyView : Window
    {
        private readonly LauncherDbContext DbContext;
        private readonly SnackbarMessageQueue MessageQueue;

        public CalibrationLobby CalibrationLobby { get; private set; }
        public IEnumerable<Account> Accounts { get; }

        public AddCalibrationLobbyView(CalibrationLobby calibrationLobby, SnackbarMessageQueue messageQueue)
        {
            InitializeComponent();
            
            MessageQueue = messageQueue;
            CalibrationLobby = calibrationLobby;
            DataContext = CalibrationLobby;

            DbContext = new LauncherDbContext();
            DbContext.Accounts.Load();
            Accounts = DbContext.Accounts.Local.ToBindingList();
        }

        private void AddCalibrationLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(CalibrationLobby.Name) || CalibrationLobby.Players.Any(p => p == null) ||
                DbContext.CalibrationLobbies.Any(bl => bl.Name == CalibrationLobby.Name)))
            {
                if (CalibrationLobby.Players.Distinct().Count() != 10)
                {
                    MessageQueue.Enqueue("No duplicated accounts allowed");
                    return;
                }

                DialogResult = true;
            }
            else
                MessageQueue.Enqueue("Fill in lobby name and select accounts, please");
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
