using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.ViewModels;

namespace Sapataria_Almeida.Views
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
