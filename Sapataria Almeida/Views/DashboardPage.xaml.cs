// Views/DashboardPage.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sapataria_Almeida.ViewModels; // Importante

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sapataria_Almeida.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DashboardPage : Page
{
    // O ViewModel agora � acessado via DataContext
    // public DashboardViewModel ViewModel { get; } // Opcional, se precisar de acesso tipado no code-behind

    public DashboardPage()
    {
        this.InitializeComponent();
        // O DataContext � definido no XAML.
        // Se preferir definir aqui:
        // ViewModel = new DashboardViewModel();
        // this.DataContext = ViewModel;
        // ViewModel.LoadArrecadacaoSemanal(); // Se n�o for chamado no construtor do VM
    }

    private void VoltarParaMainPage(object sender, RoutedEventArgs e)
    {
        // Certifique-se que MainPage existe e o namespace est� correto
        if (this.Frame.CanGoBack)
        {
            this.Frame.GoBack();
        }
        else
        {
            // Se n�o puder voltar, navegue para a MainPage como fallback,
            // mas idealmente voc� teria um hist�rico de navega��o.
            // Frame.Navigate(typeof(MainPage)); // Descomente se necess�rio
        }
    }
}