using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sistema_Sapataria.ViewModels;

namespace Sistema_Sapataria.Views
{
    public sealed partial class DashboardSemanalPage : Page
    {
        private GraficosSemanaisViewModel ViewModel => (GraficosSemanaisViewModel)DataContext;

        public DashboardSemanalPage()
        {
            this.InitializeComponent();
        }
        

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardMenuPage));
        }
    }
}
