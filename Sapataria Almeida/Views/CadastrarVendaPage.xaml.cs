using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Sapataria_Almeida.Models;
using Sapataria_Almeida.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sapataria_Almeida.Views
{
    /// <summary>  
    /// An empty page that can be used on its own or navigated to within a Frame.  
    /// </summary>  
    public sealed partial class CadastrarVendaPage : Page
    {
        private CadastroVendaViewModel _viewModel;
        public CadastroVendaViewModel ViewModel => _viewModel;

        public CadastrarVendaPage()
        {
            this.InitializeComponent();
            _viewModel = new CadastroVendaViewModel();
            DataContext = _viewModel;
        }
        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void RemoverItem_Click(object sender, RoutedEventArgs e)
        {
            // Recupera o ItemVenda que veio no Tag  
            var btn = (Button)sender;
            if (btn.Tag is ItemVenda item)
            {
                // Remove da coleção ObservableCollection no ViewModel  
                ViewModel.Carrinho.Remove(item);

                // (Opcional) notifica mudança de disponibilidade de FinalizarCommand  
                //ViewModel.FinalizarCommand.NotifyCanExecuteChanged();
            }
        }

    }
}
