using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using Texode.Wpf.Tracker.Models;

namespace Texode.Wpf.Tracker.Infrastructure
{
    public static class Export
    {
        private static readonly CommonOpenFileDialog dlg = new()
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

        public static void ExportToXML(User _selectedUser)
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

        public static void ExportToJson(User _selectedUser)
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

        public static void ExportToCSV(User _selectedUser)
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

    }
}
