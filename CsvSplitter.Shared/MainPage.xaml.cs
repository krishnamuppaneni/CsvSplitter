using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CsvSplitter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StorageFile pickedFile;
        private StorageFolder pickedFolder;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void FilenameBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(".csv");
            pickedFile = await fileOpenPicker.PickSingleFileAsync();

            if (!string.IsNullOrWhiteSpace(pickedFile?.Path))
            {
                ViewModel.FilePath = pickedFile.Path;
            }
        }

        private async void OutputPathBtn_Click(object sender, RoutedEventArgs e)
        {
#if HAS_UNO_SKIA_WPF
            var folderBrowser = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

            if (folderBrowser.ShowDialog().Value)
            {
                var selectedFolder = folderBrowser.SelectedPath;
                pickedFolder = await StorageFolder.GetFolderFromPathAsync(selectedFolder);
            }
#else
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            pickedFolder = await folderPicker.PickSingleFolderAsync();
#endif

            if (!string.IsNullOrWhiteSpace(pickedFolder?.Path))
            {
                ViewModel.OutputPath = pickedFolder.Path;
            }
        }

        private void RecordNumberBox_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
        {
            if (double.IsNaN(e.NewValue))
            {
                (sender as NumberBox).Value = 0;
            }
        }

        private async void ExcuteBtn_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.Execute(pickedFile, pickedFolder);
        }
    }
}
