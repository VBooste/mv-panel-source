using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PanelOS.Helpers
{
    class ColorFilter : INotifyPropertyChanged
    {
        private string bookmarkColor { get; set; }
        private string bookmarKind { get; set; }
        private bool selected { get; set; }

        public string BookmarkColor
        {
            get { return bookmarkColor; }
            set
            {
                bookmarkColor = value;
                OnPropertyChanged("Color");
            }
        }

        public string BookmarkKind
        {
            get { return bookmarKind; }
            set
            {
                bookmarKind = value;
                OnPropertyChanged("Kind");
            }
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
