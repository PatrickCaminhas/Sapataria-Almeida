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


    }
}
