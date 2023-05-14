using System.Windows;
using System.Windows.Input;

namespace PanelOS.Views
{
    public partial class SteamAccNotFoundView : Window
    {
        public SteamAccNotFoundView()
        {
            InitializeComponent();
        }

        private void TryAgainButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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