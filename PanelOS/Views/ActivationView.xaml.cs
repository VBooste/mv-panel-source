using System;
using System.Windows;
using System.Windows.Input;
using HWIDGen;
using MaterialDesignThemes.Wpf;

namespace PanelOS.Views
{
    public partial class ActivationView : Window
    {
        private string HWID;        

        public ActivationView()
        {
            InitializeComponent();
            HWIDGenerator hwidGenerator = new HWIDGenerator();
            HWID = hwidGenerator.GetHWID();
            HWIDLabel.Content = HWID;

            SnackbarMessageQueue activationSnackbarQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(1500));
            activationWindowPopup.MessageQueue = activationSnackbarQueue;
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/im?media=&sel=-189495491");
        }

        private void HWIDLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(HWID);
            activationWindowPopup.MessageQueue.Enqueue("Copied!");
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}