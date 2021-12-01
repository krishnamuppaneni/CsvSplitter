using System;
using Microsoft.UI.Xaml.Data;

namespace CsvSplitter.Converters
{
    /// <summary>
    /// Converts a Boolean into a Visibility.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// If set to True, conversion is reversed: True will become Collapsed.
        /// </summary>
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (targetType != typeof(bool))
                    throw new InvalidOperationException("The target must be a boolean");

                return !(bool)value;
            }
            catch (Exception)
            {
                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
