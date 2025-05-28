using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Repositories;
using Sapataria_Almeida.ViewModels;
using Sapataria_Almeida.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sapataria_Almeida.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardMenuPage : Page
    {
        public DashboardMenuViewModel ViewModel { get; }
        private readonly RepositorioDados _repositorio;


        public DashboardMenuPage()
        {
            
            this.InitializeComponent();
            var ctx = new AppDbContext();
            _repositorio = new RepositorioDados(ctx);

            ViewModel = new DashboardMenuViewModel();
            DataContext = ViewModel;

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

        private void IrParaGraficosSemanais(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardSemanalPage));
        }
        private void IrParaGraficosMensais(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardMensalPage));
        }
        private void IrParaGraficosAnuais(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardAnualPage));
        }

        private async void BtnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            // Cria o contexto e injeta no repositório
            var ctx = new AppDbContext();
            var repositorio = new RepositorioDados(ctx);

            // Cria o diálogo e define o XamlRoot
            var dialog = new AlterarSenhaAdminDialog(repositorio)
            {
                XamlRoot = this.XamlRoot   // <-- aqui
            };

            // Abre o diálogo
            bool trocou = await dialog.RequestPasswordAsync();

            if (trocou)
            {
                var success = new ContentDialog
                {
                    Title = "Sucesso",
                    Content = "Senha alterada com sucesso!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot  // também para este outro diálogo
                };
                await success.ShowAsync();
            }
        }

        private async void BtnAdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            // instancia seu repositório (mesmo padrão que você já usa)
            var ctx = new AppDbContext();
            var repo = new RepositorioDados(ctx);

            // cria o diálogo e atribui o XamlRoot
            var dialog = new AdicionarProdutoVendido(repo)
            {
                XamlRoot = this.XamlRoot
            };

            // Substitui a chamada ao método inacessível por uma abordagem alternativa
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var success = new ContentDialog
                {
                    Title = "Sucesso",
                    Content = "Produto adicionado com sucesso!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await success.ShowAsync();
            }
        }
        private async void BtnAdicionarProdutoConserto_Click(object sender, RoutedEventArgs e)
        {
            // instancia seu repositório (mesmo padrão que você já usa)
            var ctx = new AppDbContext();
            var repo = new RepositorioDados(ctx);

            // cria o diálogo e atribui o XamlRoot
            var dialog = new AdicionarProdutoConsertado(repo)
            {
                XamlRoot = this.XamlRoot
            };

            // Substitui a chamada ao método inacessível por uma abordagem alternativa
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var success = new ContentDialog
                {
                    Title = "Sucesso",
                    Content = "Produto adicionado com sucesso!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await success.ShowAsync();
            }
        }

        private async void BtnRemoverProduto_Click(object sender, RoutedEventArgs e)
        {
            var ctx = new AppDbContext();
            var repo = new RepositorioDados(ctx);

            var dialog = new RemoverProdutoVendidoDialog(repo)
            {
                XamlRoot = this.XamlRoot
            };

            // Substitui a chamada ao método inacessível por uma abordagem alternativa
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var success = new ContentDialog
                {
                    Title = "Sucesso",
                    Content = "Produto removido com sucesso!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await success.ShowAsync();
            }

            // Opcional: recarregar a lista de produtos na ViewModel após remoção
        }

        private async void BtnRemoverProdutoConserto_Click(object sender, RoutedEventArgs e)
        {
            var ctx = new AppDbContext();
            var repo = new RepositorioDados(ctx);

            var dialog = new RemoverProdutoConsertadoDialog(repo)
            {
                XamlRoot = this.XamlRoot
            };

            // Substitui a chamada ao método inacessível por uma abordagem alternativa
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var success = new ContentDialog
                {
                    Title = "Sucesso",
                    Content = "Produto removido com sucesso!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await success.ShowAsync();
            }

            // Opcional: recarregar a lista de produtos na ViewModel após remoção
        }





    }
}
