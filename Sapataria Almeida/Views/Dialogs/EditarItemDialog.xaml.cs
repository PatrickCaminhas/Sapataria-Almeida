using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.Views.Dialogs
{
    public sealed partial class EditarItemDialog : ContentDialog
    {
        public ItemConserto Item { get; }

        public EditarItemDialog(ItemConserto item)
        {
            this.InitializeComponent();
            Item = item;
            this.DataContext = Item;


            // 1) Sempre inclua o estado atual como primeira op��o
            var opcoes = new List<string> { Item.Estado };

            // 2) Se ainda estiver em "Aberto", pode ir para "Em Andamento"
            if (Item.Estado == "Pendente")
            {
                opcoes.Add("Em conserto");
                opcoes.Add("Or�amento");
            }
            // 3) 
       
            if(Item.Estado == "Or�amento")
                opcoes.Add("Em conserto");
            // 4) Se n�o for "Finalizado", sempre oferecer "Finalizado" como pr�ximo passo

            if (Item.Estado != "Finalizado")
                opcoes.Add("Finalizado");

            // 5) Atribui ao ComboBox e posiciona no estado atual
            EstadoCombo.ItemsSource = opcoes;
            EstadoCombo.SelectedItem = Item.Estado;

            // 6) Se j� estiver finalizado, desabilita totalmente a edi��o e oferece op��o de entreuge
            if (Item.Estado == "Finalizado") {
                opcoes.Add("Em conserto");
                opcoes.Add("Entregue");
                DescricaoBox.IsEnabled = false;
                ValorBox.IsEnabled = false;
            }

        }

        private void OnSaveClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // converte string -> decimal e atribui
            if (decimal.TryParse(ValorBox.Text, out var v))
                Item.Valor = v;

            Item.Descricao = DescricaoBox.Text;
            // ap�s esse m�todo, o ShowAsync() retorna Primary e a p�gina chama SaveChangesAsync()
            if (EstadoCombo.SelectedItem is string estadoSelecionado)
                Item.Estado = estadoSelecionado;
        }



       
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Aqui voc� pode validar valores antes de fechar
        }
    }
}
