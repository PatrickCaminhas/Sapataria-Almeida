using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Sistema_Sapataria.Models;

namespace Sistema_Sapataria.Views.Dialogs
{
    public sealed partial class VisualizarItemDialog : ContentDialog
    {
        public ItemConserto Item { get; }

        public VisualizarItemDialog(ItemConserto item)
        {
            this.InitializeComponent();
            Item = item;
            this.DataContext = Item;


        }

    }
}
