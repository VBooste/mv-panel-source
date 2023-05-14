using MaterialDesignThemes.Wpf;
using PanelOS.Models;
using System.Windows;
using System.Windows.Input;

namespace PanelOS.Views
{
    public partial class AddSteamAccountView : Window
    {
        public Account Account { get; private set; }
        private readonly SnackbarMessageQueue MessageQueue;

        public AddSteamAccountView(Account account, SnackbarMessageQueue messageQueue)
        {
            InitializeComponent();
            Account = account;
            MessageQueue = messageQueue;
            DataContext = Account;
        }

        private void AddSteamAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(Account.Login) || string.IsNullOrWhiteSpace(PasswordBox.Password)))
            {
                Account.Login = Account.Login.Trim();
                Account.Password = PasswordBox.Password.Trim();
                DialogResult = true;
            }
            else
            {
                MessageQueue.Enqueue("Fill in account info, please");
            }
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