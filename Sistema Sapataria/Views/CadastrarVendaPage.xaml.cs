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
using Sistema_Sapataria.Models;
using Sistema_Sapataria.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sistema_Sapataria.Views
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

        private void ValorTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs e)
        {
            // guarda posição original
            int caret = sender.SelectionStart;

            // filtra
            string filtered = new string(sender.Text.Where(c => char.IsDigit(c) || c == ',').ToArray());

            if (filtered != sender.Text)
            {
                sender.Text = filtered;
                // reposiciona o caret sem jogá-lo pro fim
                sender.SelectionStart = Math.Min(caret, filtered.Length);
            }
        }

        // 2) Ao perder o foco, completa as casas decimais
        private void ValorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            var txt = tb.Text;

            // Se estiver vazio, nada a fazer
            if (string.IsNullOrWhiteSpace(txt))
                return;

            // Garante que reste só dígitos e vírgula
            txt = new string(txt.Where(c => char.IsDigit(c) || c == ',').ToArray());

            // Se não tem vírgula, basta acrescentar ",00"
            if (!txt.Contains(','))
            {
                tb.Text = $"{txt},00";
                return;
            }

            // Tem vírgula — separa parte inteira e decimal
            var parts = txt.Split(new[] { ',' }, StringSplitOptions.None);
            var intPart = parts[0];
            var fracPart = parts.Length > 1 ? parts[1] : string.Empty;

            // Limita fração a no máximo 2 dígitos
            if (fracPart.Length > 2)
                fracPart = fracPart.Substring(0, 2);

            // Completa zeros na fração
            if (fracPart.Length == 0)
                fracPart = "00";
            else if (fracPart.Length == 1)
                fracPart += "0";

            tb.Text = $"{intPart},{fracPart}";
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
