using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Sistema_Sapataria.Repositories;
using System;
using System.Threading.Tasks;

namespace Sistema_Sapataria.Views.Dialogs
{
    public sealed partial class AdicionarProdutoVendido : ContentDialog
    {
        private readonly IRepositorioDados _repositorio;

        public AdicionarProdutoVendido(IRepositorioDados repositorio)
        {
            InitializeComponent();
            _repositorio = repositorio;
        }



        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var nome = TxtNome.Text?.Trim();
            if (string.IsNullOrWhiteSpace(nome))
            {
                // impede o diálogo de fechar e exibe erro
                args.Cancel = true;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            // grava no banco
            _repositorio.AddProduto(nome);
        }
    }
}
