using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime; // para AsTask()
using System.Text;
using Windows.ApplicationModel.DataTransfer;         // Clipboard
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.Models;
using Sapataria_Almeida.ViewModels;
using Sapataria_Almeida.Views.Dialogs;

namespace Sapataria_Almeida.Views
{
    public sealed partial class DetalhesConsertoPage : Page
    {
        private readonly DetalhesConsertoViewModel ViewModel;

        public DetalhesConsertoPage()
        {
            this.InitializeComponent();
            ViewModel = new DetalhesConsertoViewModel();
            this.DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int id)
                await ViewModel.LoadCommand.ExecuteAsync(id);
        }

        private void VoltarParaListagem(object sender, RoutedEventArgs e)
            => Frame.GoBack();


        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ItemConserto item)
            {
                var dialog = new EditarItemDialog(item);
                dialog.XamlRoot = this.XamlRoot;
                var result = await dialog.ShowAsync().AsTask();
                if (result == ContentDialogResult.Primary)
                {
                    // primeiro persiste a alteração no banco
                    await ViewModel.SaveChangesAsync();
                    // então atualiza o total na tela
                    ViewModel.RefreshTotal();
                }
            }
        }

        private async void OnGerarTextoClick(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;
            var c = vm.Conserto;

            // 1) monta a mensagem
            var sb = new StringBuilder();
            sb.AppendLine($"Prezado {c.Cliente.Nome},");
            sb.AppendLine($"Seu pedido de conserto foi realizado no dia {c.DataAbertura:dd/MM/yyyy} às {c.DataAbertura:HH:mm}.");
            sb.AppendLine("O(s) item(ns) para conserto são:");
            foreach (var it in vm.Itens)
            {
                var desc = string.IsNullOrWhiteSpace(it.Descricao) ? "" : it.Descricao;
                sb.AppendLine($"- *{it.TipoConserto}*. Valor: {it.Valor:C}");
                sb.AppendLine($"*Descrição*");
                sb.AppendLine($"{desc}");
            }
            sb.AppendLine();
            sb.AppendLine($"*Total: {vm.ValorTotal:C}*");
            sb.AppendLine();
            sb.AppendLine("[Sapataria Almeida]");

            string mensagem = sb.ToString();

            // 2) Cria um ScrollViewer com TextBlock para exibir tudo
            var scroll = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = mensagem,
                    TextWrapping = TextWrapping.Wrap
                },
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 300
            };

            // 3) Cria e exibe o dialog
            var dialog = new ContentDialog
            {
                Title = "Mensagem para Cliente",
                Content = scroll,
                PrimaryButtonText = "Copiar",
                CloseButtonText = "Fechar",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync().AsTask();

            // 4) Se clicou “Copiar”, manda para a área de transferência
            if (result == ContentDialogResult.Primary)
            {
                var dp = new DataPackage();
                dp.SetText(mensagem);
                Clipboard.SetContent(dp);
            }
        }

    }
}
