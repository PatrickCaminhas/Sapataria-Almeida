using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sistema_Sapataria.ViewModels;

namespace Sistema_Sapataria.Views
{
    public sealed partial class ListarConsertosRetiradosPage : Page
    {
        public ListarConsertosRetiradosViewModel ViewModel => (ListarConsertosRetiradosViewModel)DataContext;

        public ListarConsertosRetiradosPage()
        {
            InitializeComponent();
        }
        

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadConsertosRetiradosCommand.ExecuteAsync(null);
        }

        private void OnEditarConsertoRetiradoClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
                Frame.Navigate(typeof(DetalhesConsertoRetiradoPage), id);
        }

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

    }
}
