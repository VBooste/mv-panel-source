using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace PanelOS.Models
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class CalibrationLobby : INotifyPropertyChanged
    {
        public int CalibrationLobbyId { get; set; }
        private string name { get; set; }
        private bool active { get; set; }

        private int? leader1 { get; set; }
        private int? leader2 { get; set; }

        private int? winner1 { get; set; }
        private int? winner2 { get; set; }
        private int? winner3 { get; set; }
        private int? winner4 { get; set; }

        private int? loser1 { get; set; }
        private int? loser2 { get; set; }
        private int? loser3 { get; set; }
        private int? loser4 { get; set; }

        public int?[] Players => new int?[10]
        {
            Leader1, Leader2,
            Winner1, Winner2, Winner3, Winner4,
            Loser1, Loser2, Loser3, Loser4
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

        public int? Leader1
        {
            get { return leader1; }
            set
            {
                leader1 = value;
                OnPropertyChanged("Leader1");
            }
        }

        public int? Leader2
        {
            get { return leader2; }
            set
            {
                leader2 = value;
                OnPropertyChanged("Leader2");
            }
        }

        public int? Winner1
        {
            get { return winner1; }
            set
            {
                winner1 = value;
                OnPropertyChanged("winner1");
            }
        }

        public int? Winner2
        {
            get { return winner2; }
            set
            {
                winner2 = value;
                OnPropertyChanged("Winner2");
            }
        }

        public int? Winner3
        {
            get { return winner3; }
            set
            {
                winner3 = value;
                OnPropertyChanged("Winner3");
            }
        }

        public int? Winner4
        {
            get { return winner4; }
            set
            {
                winner4 = value;
                OnPropertyChanged("Winner4");
            }
        }

        public int? Loser1
        {
            get { return loser1; }
            set
            {
                loser1 = value;
                OnPropertyChanged("Loser1");
            }
        }

        public int? Loser2
        {
            get { return loser2; }
            set
            {
                loser2 = value;
                OnPropertyChanged("Loser2");
            }
        }

        public int? Loser3
        {
            get { return loser3; }
            set
            {
                loser3 = value;
                OnPropertyChanged("Loser3");
            }
        }

        public int? Loser4
        {
            get { return loser4; }
            set
            {
                loser4 = value;
                OnPropertyChanged("Loser4");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}