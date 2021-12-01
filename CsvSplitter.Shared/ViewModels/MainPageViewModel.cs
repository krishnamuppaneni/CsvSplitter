using CsvSplitter.Shared;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CsvSplitter.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            IsExecuting = false;
        }

        private string _FilePath { get; set; }
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        private string _OutputPath { get; set; }
        public string OutputPath
        {
            get
            {
                return _OutputPath;
            }
            set
            {
                _OutputPath = value;
                OnPropertyChanged("OutputPath");
            }
        }

        private int _RecordNumber { get; set; }
        public int RecordNumber
        {
            get
            {
                return _RecordNumber;
            }
            set
            {
                _RecordNumber = value;
                OnPropertyChanged("RecordNumber");
            }
        }

        private bool _IsExecuting { get; set; }
        public bool IsExecuting
        {
            get
            {
                return _IsExecuting;
            }
            set
            {
                _IsExecuting = value;
                ExecuteButtonText = value ? "Executing..." : "Execute";
                OnPropertyChanged("IsExecuting");
            }
        }

        private float _Progress { get; set; }
        public float Progress
        {
            get
            {
                return _Progress;
            }
            set
            {
                _Progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public string _ExecuteButtonText { get; set; }
        public string ExecuteButtonText
        {
            get
            {
                return _ExecuteButtonText;
            }
            set
            {
                _ExecuteButtonText = value;
                OnPropertyChanged("ExecuteButtonText");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task Execute(Windows.Storage.StorageFile pickedFile, Windows.Storage.StorageFolder pickedFolder)
        {
            Progress = 0;
            IsExecuting = true;

            try
            {
                var message = Validate();

                if (string.IsNullOrEmpty(message))
                {
                    var progress = new Progress<float>(currentProgress =>
                    {
                        Progress = currentProgress;
                    });

                    var csvProcessor = new CsvProcessor();
                    var splits = await csvProcessor.ProcessCsvAsync(
                        pickedFile,
                        pickedFolder,
                        RecordNumber,
                        progress);

                    message = splits > 0 ? $"The CSV file is split into a total of {splits} files."
                    : $"The CSV file is not split since the number of records in the file is not greater than {RecordNumber:N0}";
                }

                var dialog = new ContentDialog()
                {
                    Title = "CSV Splitter",
                    Content = message,
                    CloseButtonText = "Ok"
                };

                IsExecuting = false;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "Ok"
                };

                IsExecuting = false;
                await dialog.ShowAsync();
            }
        }


        public string Validate()
        {
            var message = string.Empty;

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                message += "The CSV filename is required.\n";
            }

            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                message += "The output folder is required.\n";
            }

            if (RecordNumber <= 0)
            {
                message += "The number of rows is not a valid number.";
            }

            return message;
        }
    }
}
