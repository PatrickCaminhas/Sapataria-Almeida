using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.Views.Dialogs
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
