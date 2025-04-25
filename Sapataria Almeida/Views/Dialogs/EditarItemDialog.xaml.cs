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
        }

        private void OnSaveClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // converte string -> decimal e atribui
            if (decimal.TryParse(ValorBox.Text, out var v))
                Item.Valor = v;

            Item.Descricao = DescricaoBox.Text;
            // ap�s esse m�todo, o ShowAsync() retorna Primary e a p�gina chama SaveChangesAsync()
        }
       
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Aqui voc� pode validar valores antes de fechar
        }
    }
}
