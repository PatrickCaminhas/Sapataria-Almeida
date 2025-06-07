using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sistema_Sapataria.Repositories;
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

namespace Sistema_Sapataria.Views.Dialogs;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DashboardPasswordDialog : ContentDialog
{
    private readonly RepositorioDados _repositorio;

    public DashboardPasswordDialog(RepositorioDados repositorio)
    {

        this.InitializeComponent();

        _repositorio = repositorio;
    }

    public async Task<bool> RequestPasswordAsync()
    {
        var result = await this.ShowAsync();
        if (result != ContentDialogResult.Primary)
            return false;

        var correta = _repositorio.GetDashboardPassword();
        if (PwdBox.Password == correta)
        {
            return true;
        }
        else
        {
            // mostra erro e mantém o diálogo aberto
            ErrorText.Text = "Senha incorreta. Tente novamente. ";
            ErrorText.Visibility = Visibility.Visible;
        }
        return await RequestPasswordAsync();
    }
}
