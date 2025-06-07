using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sistema_Sapataria.ViewModels;

namespace Sistema_Sapataria.Views
{
    public sealed partial class DashboardTempoPage : Page
    {
        private DashboardTempoViewModel ViewModel => (DashboardTempoViewModel)DataContext;

        public DashboardTempoPage()
        {
            this.InitializeComponent();
        }
        

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
     
    }
}
