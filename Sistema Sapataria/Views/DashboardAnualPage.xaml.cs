using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sistema_Sapataria.ViewModels;

namespace Sistema_Sapataria.Views
{
    public sealed partial class DashboardAnualPage : Page
    {
        private GraficosAnuaisViewModel ViewModel => (GraficosAnuaisViewModel)DataContext;

        public DashboardAnualPage()
        {
            this.InitializeComponent();
        }

        

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardMenuPage));
        }
     
    }
}
