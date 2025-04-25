using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sapataria_Almeida.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void AbrirCadastrarVenda_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CadastrarVendaPage));
        }
        private void AbrirCadastrarConserto_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CadastrarConsertoPage));
        }
        private void ListarConsertosPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ListarConsertosPage));
        }
        private void EditarItensConsertoPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditarItensConsertoPage));
        }
    }
}
