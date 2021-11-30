using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CsvSplitter
{
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
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;           
            folderPicker.FileTypeFilter.Add("*");
            pickedFolder = await folderPicker.PickSingleFolderAsync();

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
