using System.Windows;
using System.Windows.Input;

namespace PanelOS.Views
{
    public partial class NotificationView : Window
    {
        public NotificationView(string message)
        {
            InitializeComponent();
            NotificationLabel.Content = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}