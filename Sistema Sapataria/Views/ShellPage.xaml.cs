using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sistema_Sapataria.Data;
using Sistema_Sapataria.Repositories;
using Sistema_Sapataria.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sistema_Sapataria.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        InitializeComponent();
        // Navegue para a página inicial assim que abrir
        ContentFrame.Navigate(typeof(MainPage));
    }

    private async void NavView_SelectionChangedAsync(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected) return;
        if (args.SelectedItemContainer is not NavigationViewItem item ||
            item.Tag is not string tag) return;

        switch (tag)
        {
            case "Index":
                ContentFrame.Navigate(typeof(MainPage));
                break;
            case "CadastrarVenda":
                ContentFrame.Navigate(typeof(CadastrarVendaPage));
                break;
            case "CadastrarConserto":
                ContentFrame.Navigate(typeof(CadastrarConsertoPage));
                break;
            case "ConsertosAbertos":
                ContentFrame.Navigate(typeof(ListarConsertosPage));
                break;
            case "ConsertosFinalizados":
                ContentFrame.Navigate(typeof(ListarConsertosFinalizadosPage));
                break;
            case "ConsertosRetirados":
                ContentFrame.Navigate(typeof(ListarConsertosRetiradosPage));
                break;
            case "DashboardMenu":
                // Chama seu método de password dialog
                await HandleDashboardAsync();
                break;
        }
    }

    private async Task HandleDashboardAsync()
    {
        var repo = new RepositorioDados(new AppDbContext());
        var dlg = new DashboardPasswordDialog(repo) { XamlRoot = this.ContentFrame.XamlRoot };
        if (await dlg.RequestPasswordAsync())
            ContentFrame.Navigate(typeof(DashboardMenuPage));
    }
}
