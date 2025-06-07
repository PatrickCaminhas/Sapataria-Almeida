using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sistema_Sapataria.ViewModels;

namespace Sistema_Sapataria.Views
{
    public sealed partial class ListarConsertosPage : Page
    {
        public ListarConsertosViewModel ViewModel => (ListarConsertosViewModel)DataContext;

        public ListarConsertosPage()
        {
            InitializeComponent();
        }
        

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadConsertosCommand.ExecuteAsync(null);
        }

        private void OnEditarConsertoClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
                Frame.Navigate(typeof(DetalhesConsertoPage), id);
        }

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

    }
}
