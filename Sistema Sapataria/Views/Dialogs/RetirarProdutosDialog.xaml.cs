using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sistema_Sapataria.Models;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RetirarProdutosDialog : ContentDialog
    {


        public Conserto Conserto { get; }

        public RetirarProdutosDialog(Conserto conserto)
        {
            InitializeComponent();

            Conserto = conserto;
            DataContext = this;

            // Inicializa DatePicker e min date
            var hoje = DateTime.Today;
            var hojeOffset = new DateTimeOffset(hoje);



            // Popula combo estado (sua lógica existente)…
            var opcoes = new List<string> { Conserto.Estado };
            var metodosPagamento = new List<string> { "Dinheiro", "Cartão de Crédito", "Cartão de Débito", "Pix", "Cheque" };
            // if (Conserto.Estado == "Aberto")
            //     opcoes.Add("Em Andamento");
            // opcoes.Add("Esperando orçamento");
            // if (Conserto.Estado != "Finalizado")
            //     opcoes.Add("Finalizado");
            //     opcoes.Add("Retirado");
            PagamentoCombo.ItemsSource = metodosPagamento;
            PagamentoCombo.SelectedItem = Conserto.MetodoPagamentoFinal;

        }

   


  private void OnSaveClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var valorPagamento = Conserto.Total - Conserto.Sinal;
            Conserto.ValorPagamento = valorPagamento;
            Conserto.DataRetirada = DateTime.Now;

            if (PagamentoCombo.SelectedItem is string pagamento)
                Conserto.MetodoPagamentoFinal = pagamento;

            foreach (var item in Conserto.Itens)
            {
                item.Estado = "Retirado";
            }
            Conserto.Estado = "Retirado";
        }
    }

}