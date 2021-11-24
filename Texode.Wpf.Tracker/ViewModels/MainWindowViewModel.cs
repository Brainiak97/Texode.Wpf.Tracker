using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Texode.Wpf.Tracker.Infrastructure;
using Texode.Wpf.Tracker.Models;

namespace Texode.Wpf.Tracker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        private Command _addCommand;

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

        public Command AddCommand
        {
            get
            {
                return _addCommand ??= new Command(obj =>
                {
                    User user = new();
                    Users.Insert(0, user);
                    SelectedUser = user;
                });
            }
        }

        public MainWindowViewModel()
        {
            Users = new ObservableCollection<User>()
            {
                new User { Name = "Vlad Popkov", AverageSteps = 20000, BestRezult = 55555, WorstResult = 11111, DayResults = new() },
                new User { Name = "Anna Malanicheva", AverageSteps = 50000, BestRezult = 77777, WorstResult = 33333, DayResults = new() },
                new User { Name = "Violetta Vishnevezthkya", AverageSteps = 23333, BestRezult = 44444, WorstResult = 21111, DayResults = new() },
                new User { Name = "Gleb Pavlichev", AverageSteps = 123123, BestRezult = 31222, WorstResult = 11000, DayResults = new() },
                new User { Name = "2Vlad Popkov", AverageSteps = 20000, BestRezult = 55555, WorstResult = 11111, DayResults = new() },
                new User { Name = "2Anna Malanicheva", AverageSteps = 50000, BestRezult = 77777, WorstResult = 33333, DayResults = new() },
                new User { Name = "2Violetta Vishnevezthkya", AverageSteps = 23333, BestRezult = 44444, WorstResult = 21111, DayResults = new() },
                new User { Name = "2Gleb Pavlichev", AverageSteps = 123123, BestRezult = 31222, WorstResult = 11000, DayResults = new() }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
