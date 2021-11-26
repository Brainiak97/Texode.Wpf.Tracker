using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Texode.Wpf.Tracker.Models
{
    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private int _averageSteps;
        private int _bestRezult;
        private int _worstRezult;
        private List<DayResult> _dayResults;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int AverageSteps
        {
            get { return _averageSteps; }
            set
            {
                _averageSteps = value;
                OnPropertyChanged("AverageSteps");
            }
        }

        public int BestRezult
        {

            get { return _bestRezult; }
            set
            {
                _bestRezult = value;
                OnPropertyChanged("BestRezult");
            }
        }

        public int WorstResult
        {
            get { return _worstRezult; }
            set
            {
                _worstRezult = value;
                OnPropertyChanged("WorstResult");
            }
        }

        public List<DayResult> DayResults
        {
            get { return _dayResults; }
            set
            {
                _dayResults = value;
                OnPropertyChanged("DayResults");
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
