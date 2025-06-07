using Microsoft.UI.Xaml.Data;
using System;

namespace Sistema_Sapataria.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        // value: o DateTime; parameter: a string de formato, ex. "dd/MM/yyyy" ou "HH:mm"
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dt)
            {
                var fmt = parameter as string;
                if (!string.IsNullOrEmpty(fmt))
                    return dt.ToString(fmt);
                return dt.ToString();
            }
            return value ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
