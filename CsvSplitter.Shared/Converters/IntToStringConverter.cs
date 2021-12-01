using Microsoft.UI.Xaml.Data;
using System;

namespace CsvSplitter.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return int.TryParse((string)value, out int ret) ? ret : 0;
        }
    }
}
