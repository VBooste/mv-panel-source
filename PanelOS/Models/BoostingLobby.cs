using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace PanelOS.Models
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class BoostingLobby : INotifyPropertyChanged
    {
        public int BoostingLobbyId { get; set; }
        private string name { get; set; }
        private bool active { get; set; }

        private int? player1 { get; set; }
        private int? player2 { get; set; }
        private int? player3 { get; set; }
        private int? player4 { get; set; }
        private int? player5 { get; set; }
        private int? player6 { get; set; }
        private int? player7 { get; set; }
        private int? player8 { get; set; }
        private int? player9 { get; set; }

        public int?[] Players => new int?[9] 
        {
            Player1, Player2, Player3, Player4, 
            Player5, Player6, Player7, Player8, Player9 
        };            

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
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

        public int? Player1
        {
            get { return player1; }
            set
            {
                player1 = value;
                OnPropertyChanged("Player1");
            }
        }

        public int? Player2
        {
            get { return player2; }
            set
            {
                player2 = value;
                OnPropertyChanged("Player2");
            }
        }

        public int? Player3
        {
            get { return player3; }
            set
            {
                player3 = value;
                OnPropertyChanged("Player3");
            }
        }

        public int? Player4
        {
            get { return player4; }
            set
            {
                player4 = value;
                OnPropertyChanged("Player4");
            }
        }

        public int? Player5
        {
            get { return player5; }
            set
            {
                player5 = value;
                OnPropertyChanged("Player5");
            }
        }

        public int? Player6
        {
            get { return player6; }
            set
            {
                player6 = value;
                OnPropertyChanged("Player6");
            }
        }

        public int? Player7
        {
            get { return player7; }
            set
            {
                player7 = value;
                OnPropertyChanged("Player7");
            }
        }

        public int? Player8
        {
            get { return player8; }
            set
            {
                player8 = value;
                OnPropertyChanged("Player8");
            }
        }

        public int? Player9
        {
            get { return player9; }
            set
            {
                player9 = value;
                OnPropertyChanged("Player9");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
