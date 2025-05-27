using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Repositories;
using Sapataria_Almeida.ViewModels;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI;
using Microsoft.UI.Text;
using System.Text;
using System;



namespace Sapataria_Almeida.Views
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel vm => (MainPageViewModel)DataContext;

        public MainPage()
        {
            this.InitializeComponent();
            var dbContext = new AppDbContext();
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

        // 3.1) Destacar os dias com conserto
        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            var dia = args.Item.Date.Date;
            if (vm.ConsertosPorDia.ContainsKey(dia))
            {
                // Exemplo: fundo amarelo claro  
                args.Item.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black);
                args.Item.FontWeight = FontWeights.SemiBold;
                if (dia == DateTime.Today)
                {
                    args.Item.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White);
                    args.Item.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Orange);
                }
                if (dia > DateTime.Today)
                {
                    args.Item.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White);
                    args.Item.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Green);
                }
                if (dia < DateTime.Today)
                {
                    args.Item.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White);
                    args.Item.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.DarkRed);
                    args.Item.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Green);

                }
            }
            else if (!vm.ConsertosPorDia.ContainsKey(dia))
            {
                if (dia == DateTime.Today)
                {
                    args.Item.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White);
                    args.Item.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black);


                }
            }

        }
        // 3.2) Ao selecionar uma data, abrir um dialog com os consertos
        private async void CalendarView_SelectedDatesChanged(
            CalendarView sender,
            CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count == 0)
                return;

            var data = args.AddedDates[0].Date;
            if (!vm.ConsertosPorDia.TryGetValue(data, out var lista) || lista.Count == 0)
                return;

            // Monta o conteúdo
            var sb = new StringBuilder();
            var estadoEmAndamento = new StringBuilder();
            var estadoAguardandoSinal = new StringBuilder();
            var estadoEmOrcamento = new StringBuilder();
            var estadoOutro = new StringBuilder();
            foreach (var c in lista)
            {
                if (c.Estado == "Em Andamento")
                {
                    estadoEmAndamento.AppendLine($"Conserto ID: {c.Id}  -  Cliente: {c.Cliente.Nome}");
                }
                else if (c.Estado == "Aguardando Sinal")
                {
                    estadoAguardandoSinal.AppendLine($"Conserto ID: {c.Id}  -  Cliente: {c.Cliente.Nome}");
                }
                else if (c.Estado == "Em Orçamento")
                {
                    estadoEmOrcamento.AppendLine($"Conserto ID: {c.Id}  -  Cliente: {c.Cliente.Nome}");
                }
                else
                {
                    estadoOutro.AppendLine($"Conserto ID: {c.Id} - Estado: {c.Estado} -  Cliente: {c.Cliente.Nome}");
                }

            }
            if (estadoAguardandoSinal.Length != 0)
            {
                sb.AppendLine($"Consertos aguardando Sinal:");
                sb.AppendLine(estadoAguardandoSinal.ToString());
            }
            if (estadoEmAndamento.Length != 0)
            {
                sb.AppendLine($"Consertos em Andamento:");
                sb.AppendLine(estadoEmAndamento.ToString());
            }
            if (estadoEmOrcamento.Length != 0)
            {
                sb.AppendLine($"Consertos em Orçamento:");
                sb.AppendLine(estadoEmOrcamento.ToString());
            }
            if (estadoOutro.Length != 0)
            {
                sb.AppendLine($"Outros Estados:");
                sb.AppendLine(estadoOutro.ToString());
            }


            var dialog = new ContentDialog
            {
                Title = $"Consertos em {data:dd/MM/yyyy}",
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = sb.ToString(),
                        TextWrapping = TextWrapping.Wrap
                    },
                    Height = 200
                },
                CloseButtonText = "Fechar"
            };
            dialog.XamlRoot = this.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}
