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


            // 1) Sempre inclua o estado atual como primeira opção
            var opcoes = new List<string> { Item.Estado };

            // 2) Se ainda estiver em "Aberto", pode ir para "Em Andamento"
            if (Item.Estado == "Pendente")
            {
                opcoes.Add("Em conserto");
                opcoes.Add("Orçamento");
            }
            // 3) 
       
            if(Item.Estado == "Orçamento")
                opcoes.Add("Em conserto");
            // 4) Se não for "Finalizado", sempre oferecer "Finalizado" como próximo passo

            if (Item.Estado != "Finalizado")
                opcoes.Add("Finalizado");

            // 5) Atribui ao ComboBox e posiciona no estado atual
            EstadoCombo.ItemsSource = opcoes;
            EstadoCombo.SelectedItem = Item.Estado;

            // 6) Se já estiver finalizado, desabilita totalmente a edição e oferece opção de entreuge
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
            // após esse método, o ShowAsync() retorna Primary e a página chama SaveChangesAsync()
            if (EstadoCombo.SelectedItem is string estadoSelecionado)
                Item.Estado = estadoSelecionado;
        }



       
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Aqui você pode validar valores antes de fechar
        }
    }
}
