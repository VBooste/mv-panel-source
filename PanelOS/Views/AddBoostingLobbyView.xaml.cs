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
    public partial class AddBoostingLobbyView : Window
    {
        private readonly LauncherDbContext DbContext;
        private readonly SnackbarMessageQueue MessageQueue;

        public BoostingLobby BoostingLobby { get; private set; }
        public IEnumerable<Account> Accounts { get; }

        public AddBoostingLobbyView(BoostingLobby boostingLobby, SnackbarMessageQueue messageQueue)
        {
            InitializeComponent();
            Top -= 200;

            MessageQueue = messageQueue;
            BoostingLobby = boostingLobby;            
            DataContext = BoostingLobby;            

            DbContext = new LauncherDbContext();
            DbContext.Accounts.Load();
            Accounts = DbContext.Accounts.Local.ToBindingList();
        }

        private void AddBoostingLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(BoostingLobby.Name) || BoostingLobby.Players.Any(p => p == null) ||
                DbContext.BoostingLobbies.Any(bl => bl.Name == BoostingLobby.Name)))
            {
                if (BoostingLobby.Players.Distinct().Count() != 9)
                {
                    MessageQueue.Enqueue("No duplicated accounts allowed");
                    return;
                }

                DialogResult = true;
            }
            else
                MessageQueue.Enqueue("Fill in unique lobby name and select accounts, please");
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
