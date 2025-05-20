using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;


namespace Sapataria_Almeida.Converters
{
    public class EstadoParaCorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var estado = value as string;

            return estado switch
            {
                "Em Orçamento" => new SolidColorBrush(Colors.Red),
                "Aguardando Sinal" => new SolidColorBrush(Colors.Goldenrod),
                "Em Andamento" => new SolidColorBrush(Colors.Green),
                "Finalizado" => new SolidColorBrush(Colors.Gray),
                _ => new SolidColorBrush(Colors.White)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
