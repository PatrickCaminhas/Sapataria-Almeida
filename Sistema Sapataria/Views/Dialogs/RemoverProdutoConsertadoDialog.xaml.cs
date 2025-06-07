using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sistema_Sapataria.Models;
using Sistema_Sapataria.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sistema_Sapataria.Views.Dialogs
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoverProdutoConsertadoDialog : ContentDialog
    {
        private readonly IRepositorioDados _repositorio;

        public RemoverProdutoConsertadoDialog(IRepositorioDados repositorio)
        {
            InitializeComponent();
            _repositorio = repositorio;

            // Carrega os produtos do banco
            List<ProdutoConserto> lista = _repositorio.GetProdutosConserto();
            CboProdutos.ItemsSource = lista;
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Lê o nome selecionado
            var nomeSelecionado = CboProdutos.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(nomeSelecionado))
            {
                args.Cancel = true;
                ErrorText.Text = "Você precisa selecionar um produto.";
                ErrorText.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                return;
            }

            bool removed = _repositorio.RemoverProduto(nomeSelecionado);
            if (!removed)
            {
                args.Cancel = true;
                ErrorText.Text = "Falha ao remover. Produto não encontrado.";
                ErrorText.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
            // se removed == true, o diálogo será fechado automaticamente
        }
    }
}
