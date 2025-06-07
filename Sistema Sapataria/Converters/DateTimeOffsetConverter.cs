// Converters/DateTimeOffsetConverter.cs
using Microsoft.UI.Xaml.Data;
using System;

namespace Sistema_Sapataria.Converters
{
    public class DateTimeOffsetConverter : IValueConverter
    {
        // VM -> Control
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // se vier DateTimeOffset (não-null)
            if (value is DateTimeOffset dto)
                return dto;

            // se vier DateTime (pouco provável aqui, mas deixamos)
            if (value is DateTime dt)
                return new DateTimeOffset(dt);

            // se for null, exibe hoje (mas a VM continua null)
            return DateTimeOffset.Now;
        }

        // Control -> VM
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // o DatePicker sempre devolve DateTimeOffset
            if (value is DateTimeOffset dto)
                return (DateTimeOffset?)dto;

            // caso não venha, assumimos null
            return null;
        }
    }
}
