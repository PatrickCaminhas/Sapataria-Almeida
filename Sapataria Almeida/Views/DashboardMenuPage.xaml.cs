using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sapataria_Almeida.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardMenuPage : Page
    {
        public DashboardMenuViewModel ViewModel { get; }

        public DashboardMenuPage()
        {
            this.InitializeComponent();
            ViewModel = new DashboardMenuViewModel();
            DataContext = ViewModel;
        }



        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Ignora o Settings, se estiver visível
            if (args.IsSettingsSelected)
                return;

            if (args.SelectedItemContainer is NavigationViewItem item &&
                item.Tag is string tag)
            {
                switch (tag)
                {
                    case "Index":
                        Frame.Navigate(typeof(MainPage));
                        break;
                    case "CadastrarVenda":
                        Frame.Navigate(typeof(CadastrarVendaPage));
                        break;
                    case "CadastrarConserto":
                        Frame.Navigate(typeof(CadastrarConsertoPage));
                        break;
                    case "ConsertosAbertos":
                        Frame.Navigate(typeof(ListarConsertosPage));
                        break;
                    case "ConsertosFinalizados":
                        Frame.Navigate(typeof(ListarConsertosFinalizadosPage));
                        break;
                    case "ConsertosRetirados":
                        Frame.Navigate(typeof(ListarConsertosRetiradosPage));
                        break;
                    case "DashboardMenu":
                        Frame.Navigate(typeof(DashboardMenuPage));
                        break;
                }
            }
        }

        private void IrParaGraficosSemanais(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardSemanalPage));
        }
        private void IrParaGraficosMensais(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardMensalPage));
        }
        private void IrParaGraficosAnuais(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DashboardAnualPage));
        }




    }
}
