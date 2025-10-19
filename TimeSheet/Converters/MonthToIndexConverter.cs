// Converters/MonthToIndexConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace TimeSheet.Converters
{
    public class MonthToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int month ? month - 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int index ? index + 1 : 1;
        }
    }
}