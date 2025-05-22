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
    public sealed partial class CadastrarConsertoPage : Page
    {
        public CadastroConsertoViewModel ViewModel { get; }
        private bool _isFormatting = false;
        private bool _isFormattingAmount;

        public CadastrarConsertoPage()
        {
            InitializeComponent();
            ViewModel = new CadastroConsertoViewModel();
            DataContext = ViewModel;
        }

        private void OnClienteSelecionado(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectClienteCommand.CanExecute(ViewModel.ClienteSelecionado))
                ViewModel.SelectClienteCommand.Execute(ViewModel.ClienteSelecionado);
        }


        private async void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;

            if (vm.TryGetClienteConflito(out var existente))
            {
                var dialog = new ContentDialog
                {
                    Title = "Telefone já cadastrado",
                    Content = $"O número {vm.TelefoneCliente} já pertence a '{existente.Nome}'.\nO que deseja fazer?",
                    PrimaryButtonText = "Corrigir Nome",
                    SecondaryButtonText = "Corrigir Telefone",
                    CloseButtonText = "Cancelar",
                };

                // <-- aqui: vincula ao XamlRoot da página
                dialog.XamlRoot = this.XamlRoot;

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    vm.NomeCliente = existente.Nome;
                    return;
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    vm.TelefoneCliente = string.Empty;
                    return;
                }
                else
                {
                    return;
                }
            }

            // sem conflito, chama o comando do ViewModel
            if (vm.FinalizarCommand.CanExecute(null))
                await vm.FinalizarCommand.ExecuteAsync(null);
        }

        private void VoltarParaMainPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void TelefoneTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            // evita reentrância
            if (_isFormatting) return;
            _isFormatting = true;

            // extrai apenas dígitos
            var onlyDigits = new string(sender.Text.Where(char.IsDigit).ToArray());

            // gera a máscara adequada
            sender.Text = FormatarTelefone(onlyDigits);

            // coloca o cursor no fim
            sender.SelectionStart = sender.Text.Length;

            _isFormatting = false;
        }
        private string FormatarTelefone(string digits)
        {
            // (xx) x xxxx-xxxx  -> 11 dígitos (celular)
            // (xx) xxxx-xxxx    -> 10 dígitos (fixo)
            if (digits.Length <= 2)
            {
                return "(" + digits;
            }
            if (digits.Length <= 6)
            {
                // 2+4: (xx) yyyy
                return $"({digits[..2]}) {digits[2..]}";
            }
            if (digits.Length <= 10)
            {
                // 2 + 4 + 4: (xx) yyyy-zzzz
                var ddd = digits[..2];
                var part1 = digits[2..^4];
                var part2 = digits[^4..];
                return $"({ddd}) {part1}-{part2}";
            }
            // >10: considera 11 dígitos (celular)
            digits = digits[..11]; // ignora extras
            var dddCel = digits[..2];
            var first = digits[2..3];
            var middle = digits[3..^4];
            var last4 = digits[^4..];
            return $"({dddCel}) {first} {middle}-{last4}";
        }

        // 1) Apenas remove tudo que não for dígito ou vírgula
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
            // Recupera o ItemConserto que veio no Tag
            var btn = (Button)sender;
            if (btn.Tag is ItemConserto item)
            {
                // Remove da coleção ObservableCollection no ViewModel
                ViewModel.Carrinho.Remove(item);

                // (Opcional) notifica mudança de disponibilidade de FinalizarCommand
                ViewModel.FinalizarCommand.NotifyCanExecuteChanged();
            }
        }




    }
}
