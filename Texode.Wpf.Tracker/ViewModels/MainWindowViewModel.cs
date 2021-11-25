using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Texode.Wpf.Tracker.Infrastructure;
using Texode.Wpf.Tracker.Models;

namespace Texode.Wpf.Tracker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        private RelayCommand _addCommand;

        public ObservableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(LoadFiles, true);
            }
        }

        public void LoadFiles()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Json files (*.json)|*.json",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var qwe = ReadJson(openFileDialog.FileNames);
            }

            //Users = new ObservableCollection<User>()
            //        {
            //            new User { Name = "Vlad Popkov", AverageSteps = 20000, BestRezult = 55555, WorstResult = 11111, DayResults = new() },
            //            new User { Name = "Anna Malanicheva", AverageSteps = 50000, BestRezult = 77777, WorstResult = 33333, DayResults = new() },
            //            new User { Name = "Violetta Vishnevezthkya", AverageSteps = 23333, BestRezult = 44444, WorstResult = 21111, DayResults = new() },
            //            new User { Name = "Gleb Pavlichev", AverageSteps = 123123, BestRezult = 31222, WorstResult = 11000, DayResults = new() },
            //            new User { Name = "2Vlad Popkov", AverageSteps = 20000, BestRezult = 55555, WorstResult = 11111, DayResults = new() },
            //            new User { Name = "2Anna Malanicheva", AverageSteps = 50000, BestRezult = 77777, WorstResult = 33333, DayResults = new() },
            //            new User { Name = "2Violetta Vishnevezthkya", AverageSteps = 23333, BestRezult = 44444, WorstResult = 21111, DayResults = new() },
            //            new User { Name = "2Gleb Pavlichev", AverageSteps = 123123, BestRezult = 31222, WorstResult = 11000, DayResults = new() }
            //        };

            OnPropertyChanged("Users");
        }

        private List<List<DayResult>> ReadJson(string[] fileNames)
        {
            var days = new List<List<DayResult>>();

            for (int i = 0; i < fileNames.Length; i++)
            {
                using StreamReader fs = new(fileNames[i]);
                var _list = JsonConvert.DeserializeObject<List<DayResult>>(fs.ReadToEnd());

                foreach(var dayResult in _list)
                {
                    dayResult.Day = Convert.ToInt32(Regex.Replace(fileNames[i], @"[^\d]+", ""));
                }

                days.Add(_list);
            }
            return days;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
