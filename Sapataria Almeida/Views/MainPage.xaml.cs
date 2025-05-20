using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.Data;           
using Sapataria_Almeida.Repositories;   
using Sapataria_Almeida.ViewModels;
using Microsoft.UI.Xaml.Controls.Primitives;            
                      


namespace Sapataria_Almeida.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var dbContext = new AppDbContext();               // configure conforme seu setup
            var repositorio = new RepositorioDados(dbContext);

            this.DataContext = new MainPageViewModel(repositorio);

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
                   // case "ConsertosRetirados":
                        // Implemente sua página de consertos retirados
                     //   Frame.Navigate(typeof(ListarConsertosRetiradosPage));
                       // break;
                }
            }
        }

        // Handler para o evento DayItemChanging
        private void CalendarView_DayItemChanging(
           CalendarView sender,
           CalendarViewDayItemChangingEventArgs args)
        {
            // Aqui você pode, por exemplo, destacar dias com entregas:
            // if (TemEntregaNoDia(args.Item.Date.Date))
            // {
            //     args.Item.Background = new SolidColorBrush(Colors.LightGreen);
            // }
            // Exemplo: destaque dias com entregas
            // var dia = args.Item.Date.Date;
            // if (TemEntregaNoDia(dia)) { … }
        }
    }
}
