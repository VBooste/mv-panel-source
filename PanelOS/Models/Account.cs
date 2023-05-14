using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace PanelOS.Models
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class Account : INotifyPropertyChanged
    {
        public int AccountId { get; set; }
        private string steamProfileId { get; set; }
        private string steamUserId { get; set; }
        private string login { get; set; }
        private string password { get; set; }
        private string color { get; set; }
        private bool active { get; set; }        
        private bool isChecked { get; set; }

        public Account()
        {
            Color = "Gray";
        }

        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public string Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

        public string SteamProfileId
        {
            get { return steamProfileId; }
            set
            {
                steamProfileId = value;
                OnPropertyChanged("SteamProfileId");
            }
        }

        public string SteamUserId
        {
            get { return steamUserId; }
            set
            {
                steamUserId = value;
                OnPropertyChanged("SteamUserId");
            }
        }

        [NotMapped]
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                OnPropertyChanged("Active");
            }
        }

        [NotMapped]
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}