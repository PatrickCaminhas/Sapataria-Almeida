using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sapataria_Almeida
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow appWindow;

        public MainWindow()
        {
            this.InitializeComponent();
            // Recupera o AppWindow
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            // Define tamanho mínimo
            var minSize = new SizeInt32 { Width = 1280, Height = 650 }; // largura x altura mínima
            appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.SetBorderAndTitleBar(true, true);
            presenter.IsResizable = true;

            // Corrigido: Substituir SetPreferredMinSize por ResizeClient
            appWindow.ResizeClient(minSize);
            presenter.Maximize();

            MainFrame.Navigate(typeof(Views.MainPage));
        }
    }
}
