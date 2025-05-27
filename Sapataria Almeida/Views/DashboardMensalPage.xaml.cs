using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.ViewModels;

namespace Sapataria_Almeida.Views
{
    public sealed partial class DashboardMensalPage : Page
    {
        private GraficosMensaisViewModel ViewModel => (GraficosMensaisViewModel)DataContext;

        public DashboardMensalPage()
        {
            this.InitializeComponent();
        }

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
     
    }
}
