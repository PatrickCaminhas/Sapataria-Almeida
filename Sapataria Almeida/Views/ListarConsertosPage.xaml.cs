using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.ViewModels;

namespace Sapataria_Almeida.Views
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

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            // se quiser navegar para outra página:
            // var conserto = (Conserto)e.ClickedItem;
            // Frame.Navigate(typeof(DetalhesConsertoPage), conserto.Id);
        }
        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

    }
}
