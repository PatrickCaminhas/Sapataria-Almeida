using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.ViewModels;

namespace Sapataria_Almeida.Views
{
    public sealed partial class ListarConsertosFinalizadosPage : Page
    {
        public ListarConsertosFinalizadosViewModel ViewModel => (ListarConsertosFinalizadosViewModel)DataContext;

        public ListarConsertosFinalizadosPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadConsertosFinalizadosCommand.ExecuteAsync(null);
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
