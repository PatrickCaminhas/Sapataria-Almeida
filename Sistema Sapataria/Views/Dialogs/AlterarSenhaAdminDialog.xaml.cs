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


namespace Sistema_Sapataria.Views.Dialogs;


public sealed partial class AlterarSenhaAdminDialog : ContentDialog
{
    private readonly RepositorioDados _repositorio;

    public AlterarSenhaAdminDialog(RepositorioDados repositorio)
    {
        InitializeComponent();
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
            _repositorio.SetDashboardPassword(PwdBoxNew.Password);
            return true;
        }
        else
        {
            // mostra erro e mantém o diálogo aberto
            ErrorText.Text = "Senha incorreta. Tente novamente.";
            ErrorText.Visibility = Visibility.Visible;
        }
        return await RequestPasswordAsync();
    }
}
