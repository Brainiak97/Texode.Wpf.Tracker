using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using Texode.Wpf.Tracker.Infrastructure;
using Texode.Wpf.Tracker.Models;

namespace Texode.Wpf.Tracker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        private RelayCommand _addCommand;
        private RelayCommand _exportSelectedUserXMLCommand;
        private RelayCommand _exportSelectedUserJsonCommand;
        private RelayCommand _exportSelectedUserCSVCommand;
        private PlotModel _model;
        private int _difference;
        private bool _isSelected;
        private bool _isUsersLoaded;
        private readonly CommonOpenFileDialog dlg = new()
        {
            Title = "Выберите папку для экспорта",
            IsFolderPicker = true,
            InitialDirectory = Assembly.GetExecutingAssembly().Location,
            AddToMostRecentlyUsedList = false,
            AllowNonFileSystemItems = false,
            DefaultDirectory = Assembly.GetExecutingAssembly().Location,
            EnsureFileExists = true,
            EnsurePathExists = true,
            EnsureReadOnly = false,
            EnsureValidNames = true,
            Multiselect = false,
            ShowPlacesList = true
        };

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

                this.Model = CreatePlot(_selectedUser);

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

        public RelayCommand ExportSelectedUserXMLCommand
        {
            get
            {
                return _exportSelectedUserXMLCommand ??= new RelayCommand(ExportSelectedUserXML, true);
            }
        }

        public RelayCommand ExportSelectedUserJsonCommand
        {
            get
            {
                return _exportSelectedUserJsonCommand ??= new RelayCommand(ExportSelectedUserJson, true);
            }
        }

        public RelayCommand ExportSelectedUserCSVCommand
        {
            get
            {
                return _exportSelectedUserCSVCommand ??= new RelayCommand(ExportSelectedUserCSV, true);
            }
        }

        public PlotModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                OnPropertyChanged("Model");
            }
        }

        public int Difference
        {
            get
            {
                return _difference;
            }
            set
            {
                _difference = value;
                OnPropertyChanged("Difference");
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool IsUsersLoaded
        {
            get
            {
                return _isUsersLoaded;
            }
            set
            {
                _isUsersLoaded = value;
                OnPropertyChanged("IsUsersLoaded");
            }
        }

        private void ExportSelectedUserXML()
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    XmlSerializer xmlSerializer = new(typeof(User));
                    xmlSerializer.Serialize(File.Create(dlg.FileName + $"{_selectedUser.Name}-analysis results.XML"), _selectedUser);

                    MessageBox.Show("Результаты успешно сохранены в файл *.XML");
                }
                catch { MessageBox.Show("Ошибка сохранения результатов"); }
            }
        }

        private void ExportSelectedUserJson()
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    File.WriteAllText(
                        dlg.FileName + $"{_selectedUser.Name}-analysis results.json",
                        JsonConvert.SerializeObject(_selectedUser)
                        );

                    MessageBox.Show("Результаты успешно сохранены в файл *.json");
                }
                catch { MessageBox.Show("Ошибка сохранения результатов"); }
            }
        }

        private void ExportSelectedUserCSV()
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    FileStream fileStream = new(dlg.FileName + $"{_selectedUser.Name}-analysis results.CSV", FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                    using StreamWriter streamWriter = new(fileStream);

                    using CsvWriter csvWriter = new(streamWriter,
                        new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            Encoding = Encoding.UTF8
                        });

                    csvWriter.WriteField("Name");
                    csvWriter.WriteField(_selectedUser.Name);
                    csvWriter.WriteField("Average");
                    csvWriter.WriteField(_selectedUser.AverageSteps);
                    csvWriter.WriteField("Best");
                    csvWriter.WriteField(_selectedUser.BestRezult);
                    csvWriter.WriteField("Worst");
                    csvWriter.WriteField(_selectedUser.WorstResult);
                    csvWriter.NextRecord();

                    csvWriter.WriteField("Day");
                    csvWriter.WriteField("Rank");
                    csvWriter.WriteField("Status");
                    csvWriter.WriteField("Steps");
                    csvWriter.NextRecord();

                    foreach (var dayResult in _selectedUser.DayResults)
                    {
                        csvWriter.WriteField(dayResult.Day);
                        csvWriter.WriteField(dayResult.Rank);
                        csvWriter.WriteField(dayResult.Status);
                        csvWriter.WriteField(dayResult.Steps);
                        csvWriter.NextRecord();
                    }

                    streamWriter.Flush();
                }
                catch { MessageBox.Show("Ошибка сохранения результатов"); }

                MessageBox.Show("Результаты успешно сохранены в файл *.CSV");
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
                Users = new ObservableCollection<User>(CreateUsers(ReadJson(openFileDialog.FileNames)));
            }

            OnPropertyChanged("Users");
        }

        private List<List<DayResult>> ReadJson(string[] fileNames)
        {
            var days = new List<List<DayResult>>();

            for (int i = 0; i < fileNames.Length; i++)
            {
                using StreamReader fs = new(fileNames[i]);
                var _list = JsonConvert.DeserializeObject<List<DayResult>>(fs.ReadToEnd());

                foreach (var dayResult in _list)
                {
                    try
                    {
                        dayResult.Day = Convert.ToInt32(Regex.Replace(fileNames[i], @"[^\d]+", ""));
                    }
                    catch
                    {
                        MessageBox.Show($"Ошибка чтения файла {fileNames[i]}");
                    }
                }

                days.Add(_list);
            }
            return days;
        }

        private List<User> CreateUsers(List<List<DayResult>> dayResults)
        {
            List<User> users = new();

            int index = 1;

            foreach (var days in dayResults)
            {
                users.AddRange(from results in days
                               where !users.Exists(u => u.Name == results.User)
                               select new User()
                               {
                                   Id = index++,
                                   Name = results.User
                               });
            }

            foreach (var user in users)
            {
                user.AverageSteps = GetAverageSteps(user, dayResults);
                user.BestRezult = GetBestRezult(user, dayResults);
                user.WorstResult = GetWorstRezult(user, dayResults);
                user.DayResults = GetDaysResults(user, dayResults);
            }

            if (users is not null)
            {
                IsUsersLoaded = true;
            }
            else
            {
                IsUsersLoaded = false;
            }

            return users;
        }

        private int GetAverageSteps(User user, List<List<DayResult>> dayResults)
        {
            int averageSteps = 0;

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null)
                {
                    averageSteps += day.FirstOrDefault(d => d.User == user.Name).Steps;
                }
            }

            return averageSteps / dayResults.Count;
        }

        private int GetBestRezult(User user, List<List<DayResult>> dayResults)
        {
            int max = 0;

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null && day.FirstOrDefault(d => d.User == user.Name).Steps > max)
                {
                    max = day.FirstOrDefault(d => d.User == user.Name).Steps;
                }
            }

            return max;
        }

        private int GetWorstRezult(User user, List<List<DayResult>> dayResults)
        {
            int min = user.BestRezult;

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null && day.FirstOrDefault(d => d.User == user.Name).Steps < min)
                {
                    min = day.FirstOrDefault(d => d.User == user.Name).Steps;
                }
            }

            return min;
        }

        private List<DayResult> GetDaysResults(User user, List<List<DayResult>> dayResults)
        {
            List<DayResult> userDaysR = new();

            foreach (var day in dayResults)
            {
                if (day.FirstOrDefault(d => d.User == user.Name) is not null)
                {
                    userDaysR.Add(day.FirstOrDefault(d => d.User == user.Name));
                }
            }

            return userDaysR;
        }

        private PlotModel CreatePlot(User selectedUser)
        {
            _model = new();

            if (selectedUser is not null)
            {
                IsSelected = true;

                var mainSeries = new LineSeries();
                var minSeries = new LineSeries();
                var maxSeries = new LineSeries();

                List<DataPoint> points = new();

                _model.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = selectedUser.DayResults.Select(d => d.Steps).Min() / 1.5,
                    Maximum = selectedUser.DayResults.Select(d => d.Steps).Max() * 1.5,
                    Title = "Шаги"
                });

                _model.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = 1,
                    Maximum = 30,
                    Title = "День"
                });

                foreach (var dayresult in selectedUser.DayResults)
                {
                    points.Add(new DataPoint(dayresult.Day, dayresult.Steps));
                }

                minSeries.Points.Add(
                    new DataPoint(
                    selectedUser.DayResults.FirstOrDefault(d => d.Steps == selectedUser.DayResults.Select(d => d.Steps).Min()).Day,
                    selectedUser.DayResults.Select(d => d.Steps).Min()));

                maxSeries.Points.Add(
                    new DataPoint(
                    selectedUser.DayResults.FirstOrDefault(d => d.Steps == selectedUser.DayResults.Select(d => d.Steps).Max()).Day,
                    selectedUser.DayResults.Select(d => d.Steps).Max()));

                mainSeries.Points.AddRange(points);
                mainSeries.MarkerType = MarkerType.Circle;
                mainSeries.Color = OxyColors.RoyalBlue;

                minSeries.MarkerType = MarkerType.Circle;
                minSeries.Color = OxyColors.Red;
                minSeries.MarkerSize = 5;

                maxSeries.MarkerType = MarkerType.Circle;
                maxSeries.Color = OxyColors.LimeGreen;
                maxSeries.MarkerSize = 5;

                _model.Series.Add(mainSeries);
                _model.Series.Add(minSeries);
                _model.Series.Add(maxSeries);

                _model.PlotMargins = new OxyThickness(70, 20, 20, 50);
            }
            else
            {
                IsSelected = false;
            }

            return _model;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
