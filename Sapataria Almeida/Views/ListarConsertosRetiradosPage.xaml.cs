using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.ViewModels;

namespace Sapataria_Almeida.Views
{
    public sealed partial class ListarConsertosRetiradosPage : Page
    {
        public ListarConsertosRetiradosViewModel ViewModel => (ListarConsertosRetiradosViewModel)DataContext;

        public ListarConsertosRetiradosPage()
        {
            InitializeComponent();
        }
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Ignora o Settings, se estiver visível
            if (args.IsSettingsSelected)
                return;

            if (args.SelectedItemContainer is NavigationViewItem item &&
                item.Tag is string tag)
            {
                switch (tag)
                {
                    case "Index":
                        Frame.Navigate(typeof(MainPage));
                        break;
                    case "CadastrarVenda":
                        Frame.Navigate(typeof(CadastrarVendaPage));
                        break;
                    case "CadastrarConserto":
                        Frame.Navigate(typeof(CadastrarConsertoPage));
                        break;
                    case "ConsertosAbertos":
                        Frame.Navigate(typeof(ListarConsertosPage));
                        break;
                    case "ConsertosFinalizados":
                        Frame.Navigate(typeof(ListarConsertosFinalizadosPage));
                        break;
                    case "ConsertosRetirados":
                        Frame.Navigate(typeof(ListarConsertosRetiradosPage));
                        break;
                    case "DashboardMenu":
                        Frame.Navigate(typeof(DashboardMenuPage));
                        break;
                }
            }
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
