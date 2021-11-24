using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Texode.Wpf.Tracker.Models
{
    public class DayResult : INotifyPropertyChanged
    {
        private string _day;
        private int _rank;
        private string _user;
        private string _status;
        private int _steps;

        public string Day
        {
            get { return _day; }
            set
            {
                _day = value;
                OnPropertyChanged("Day");
            }
        }

        public int Rank
        {
            get { return _rank; }
            set
            {
                _rank = value;
                OnPropertyChanged("Rank");
            }
        }

        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public int Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged("Steps");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged is not null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
