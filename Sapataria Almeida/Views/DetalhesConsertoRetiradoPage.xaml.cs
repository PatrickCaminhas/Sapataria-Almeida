using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime; // para AsTask()
using System.Text;
using QuestPDF.Infrastructure;

using WinRT.Interop;
using Windows.ApplicationModel.DataTransfer;         // Clipboard
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Sapataria_Almeida.Models;
using Sapataria_Almeida.ViewModels;
using Sapataria_Almeida.Views.Dialogs;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Printing;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.Storage;
using QuestPDF.Fluent;
using System.IO;
using System.Linq;

namespace Sapataria_Almeida.Views
{
    public sealed partial class DetalhesConsertoRetiradoPage : Page
    {
        private readonly DetalhesConsertoViewModel ViewModel;


        public DetalhesConsertoRetiradoPage()
        {
            this.InitializeComponent();
            ViewModel = new DetalhesConsertoViewModel();
            this.DataContext = ViewModel;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // caso o parâmetro seja long
            if (e.Parameter is long idLong)
            {
                await ViewModel.LoadCommand.ExecuteAsync(idLong);
            }
            // opcionalmente, para compatibilidade, trate int também
            else if (e.Parameter is int idInt)
            {
                await ViewModel.LoadCommand.ExecuteAsync(idInt);
            }
        }


        private void VoltarParaListagem(object sender, RoutedEventArgs e)
            => Frame.GoBack();


        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ItemConserto item)
            {
                var dialog = new VisualizarItemDialog(item);
                dialog.XamlRoot = this.XamlRoot;
                var result = await dialog.ShowAsync().AsTask();
                if (result == ContentDialogResult.Primary)
                {
                    // primeiro persiste a alteração no banco
                    await ViewModel.SaveChangesAsync();
                    // então atualiza o total na tela
                    ViewModel.RefreshItem(item);
                    bool temItemEmOrcamento = ViewModel.Conserto.Itens.Any(item => item.Estado == "Orçamento");
                    if (item.Estado == "Em conserto" && ViewModel.Conserto.Estado == "Finalizado" && !temItemEmOrcamento)
                    {
                        ViewModel.Conserto.Estado = "Em Andamento";
                        await ViewModel.SaveChangesAsync();
                        ViewModel.RefreshConserto();
                    }
                    //bool todosItemsForamRetirados = ViewModel.Conserto.Itens.All(item => item.Estado == "Retirado");
                    //if(item.Estado == "Retirado" && todosItemsForamRetirados)
                    //{
                    //    ViewModel.Conserto.Estado = "Retirado";
                    //    await ViewModel.SaveChangesAsync();
                    //    ViewModel.RefreshConserto();
                    //}
                        await VerificarSeEstadosSaoFinalizadosAsync();

                    ViewModel.RefreshTotal();


                }
            }
        }

        private async Task VerificarSeEstadosSaoFinalizadosAsync()
        {
            // Usa LINQ para checar todos de uma vez
            bool todosFinalizados = ViewModel.Itens.All(i => i.Estado == "Finalizado");
            if (!todosFinalizados)
                return;

            // Atualiza o estado da ordem
            ViewModel.Conserto.Estado = "Finalizado";
            ViewModel.Conserto.DataFinal = DateTime.Now.Date;

            // Persiste e notifica UI
            await ViewModel.SaveChangesAsync();
            ViewModel.RefreshConserto();
        }

        private async void OnAlterarConsertoClick(object sender, RoutedEventArgs e)
        {
            var dialog = new EditarConsertoDialog(ViewModel.Conserto)
            {
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync().AsTask();
            if (result == ContentDialogResult.Primary)
            {
                // Persiste no banco
                await ViewModel.SaveChangesAsync();

                // Atualiza UI (notifique propriedades ou recarregue o Conserto)
                ViewModel.RefreshConserto();
            }
        }

        private async void OnFinalizarConsertoClick(object sender, RoutedEventArgs e)
        {
            var dialog = new RetirarProdutosDialog(ViewModel.Conserto)
            {
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync().AsTask();
            if (result == ContentDialogResult.Primary)
            {
                // Persiste no banco
                await ViewModel.SaveChangesAsync();

                // Atualiza UI (notifique propriedades ou recarregue o Conserto)
                ViewModel.RefreshConserto();
            }
        }



        private async void OnGerarTextoClick(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;
            var c = vm.Conserto;

            // 1) monta a mensagem
            var sb = new StringBuilder();
            sb.AppendLine($"Prezado {c.Cliente.Nome},");
            sb.AppendLine($"Seu pedido de conserto foi realizado no dia {c.DataAbertura:dd/MM/yyyy} às {c.DataAbertura:HH:mm}.");
            if (vm.Itens.Count > 1)
            {
                sb.AppendLine("Os itens para conserto são:");
            }
            else
            {
                sb.AppendLine("O item para conserto é:");
            }
            foreach (var it in vm.Itens)
            {
                var desc = string.IsNullOrWhiteSpace(it.Descricao) ? "" : it.Descricao;
                sb.AppendLine($"\U0001F4CD *{it.TipoConserto}*. Valor: {it.Valor:C}");
                sb.AppendLine($"Estado do conserto: *{it.Estado}*");
                sb.AppendLine($"*Descrição*");
                sb.AppendLine($"{desc}");
            }
            sb.AppendLine();
            sb.AppendLine($"*Total: {vm.ValorTotal:C}*");
            sb.AppendLine();
            sb.AppendLine("*[Sapataria Almeida]*");

            string mensagem = sb.ToString();

            // 2) Cria um ScrollViewer com TextBlock para exibir tudo
            var scroll = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = mensagem,
                    TextWrapping = TextWrapping.Wrap
                },
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 300
            };

            // 3) Cria e exibe o dialog
            var dialog = new ContentDialog
            {
                Title = "Mensagem para Cliente",
                Content = scroll,
                PrimaryButtonText = "Copiar",
                CloseButtonText = "Fechar",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync().AsTask();

            // 4) Se clicou “Copiar”, manda para a área de transferência
            if (result == ContentDialogResult.Primary)
            {
                var dp = new DataPackage();
                dp.SetText(mensagem);
                Clipboard.SetContent(dp);
            }

        }




        private async void OnGerarImprimirClick(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;
            var c = vm.Conserto;

            // 1) monta a mensagem
            var sb = new StringBuilder();
            int quantidade = 1;
            foreach (var it in vm.Itens)
            {
                sb.AppendLine($"-------------");
                sb.AppendLine($"[CONSERTO]");
                sb.AppendLine($"ID: {c.Id}");
                sb.AppendLine($"Cliente: {c.Cliente.Nome}");
                if (c.Sinal == c.Total)
                {
                    sb.AppendLine($"Sinal: Pago");
                }
                else
                {
                    sb.AppendLine($"Sinal: {c.Sinal}");
                }
                sb.AppendLine($"-----");

                sb.AppendLine($"[ITEM]");
                var desc = string.IsNullOrWhiteSpace(it.Descricao) ? "" : it.Descricao;
                sb.AppendLine($"{it.TipoConserto}");
                if (it.Estado == "Orçamento")
                {
                    sb.AppendLine($"Em orçamento");
                }
                else
                {
                    sb.AppendLine($"Valor: R${it.Valor:c}");
                }
                sb.AppendLine($"Conclusão: {c.DataAbertura:dd/MM/yyyy}");
                sb.AppendLine($"Descrição: {desc}");
                sb.AppendLine($"{quantidade} de {vm.Itens.Count} itens deixados.");
                sb.AppendLine($"-------------");

                sb.AppendLine();
                quantidade++;
            }




            string mensagem = sb.ToString();

            // 2) Cria um ScrollViewer com TextBlock para exibir tudo
            var scroll = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = mensagem,
                    TextWrapping = TextWrapping.Wrap
                },
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 300
            };

            // 3) Cria e exibe o dialog
            var dialog = new ContentDialog
            {
                Title = "Imprimir",
                Content = scroll,
                PrimaryButtonText = "Copiar",
                SecondaryButtonText = "Salvar PDF / Imprimir",

                CloseButtonText = "Fechar",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync().AsTask();

            // 4) Se clicou “Copiar”, manda para a área de transferência
            if (result == ContentDialogResult.Primary)
            {
                var dp = new DataPackage();
                dp.SetText(mensagem);
                Clipboard.SetContent(dp);
            }



            else if (result == ContentDialogResult.Secondary)
            {
                var savePicker = new FileSavePicker();

                // Inicializa janela nativa (corrigido)
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("PDF", new List<string>() { ".pdf" });
                savePicker.SuggestedFileName = $"Conserto_{c.Id}";

                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    // Cria o PDF usando QuestPDF
                    var stream = await file.OpenStreamForWriteAsync();

                    var mensagemCopy = mensagem; // Precisa capturar variável externa

                    QuestPDF.Fluent.Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(30);
                            page.Size(QuestPDF.Helpers.PageSizes.A4);
                            page.Content()
                                .Text(mensagemCopy)
                                .FontSize(12);
                        });
                    }).GeneratePdf(stream);

                    stream.Close();

                    await new ContentDialog
                    {
                        Title = "PDF salvo",
                        Content = $"PDF salvo com sucesso em:\n{file.Path}",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    }.ShowAsync();
                }
            }




        }


        private async void OnGerarEstadoTexto_Click(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;
            var c = vm.Conserto;

            // Recupera o ItemConserto específico a partir do botão clicado
            var botao = (Button)sender;
            var item = (ItemConserto)botao.Tag; // <- Aqui está o Binding recebido

            // 1) monta a mensagem
            var sb = new StringBuilder();
            sb.AppendLine($"Prezado {c.Cliente.Nome},");
            if (vm.Conserto.Estado == "Retirado")
                sb.AppendLine($"O conserto de {item.TipoConserto} foi finalizado e retirado no dia {item.DataEntrega:dd/MM/yyyy} às {item.DataEntrega:HH:mm}.");
            else if (item.Estado == "Finalizado")
            {
                sb.AppendLine($"O conserto de {item.TipoConserto} está pronto para retirada.");
            }
            else if (item.Estado == "Pendente")
            {
                sb.AppendLine($"O conserto de {item.TipoConserto} ainda não foi iniciado.");
            }
            else if (item.Estado == "Em conserto")
            {
                sb.AppendLine($"O item {item.TipoConserto} ainda está em processo de conserto.");
            }
            else if (item.Estado == "Orçamento")
            {
                sb.AppendLine($"O pedido de {item.TipoConserto} ainda está aguardando orçamento do sapateiro.");
            }

            var desc = string.IsNullOrWhiteSpace(item.Descricao) ? "" : item.Descricao;
            if (item.Estado != "Orcamento")
            {
                sb.AppendLine($"\U0001F4CD *{item.TipoConserto}*. Valor: {item.Valor:C}");
            }
            else
            {
                sb.AppendLine($"\U0001F4CD *{item.TipoConserto}*.");
            }
            sb.AppendLine($"Estado do conserto: *{item.Estado}*");
            sb.AppendLine($"*Descrição*");
            sb.AppendLine($"{desc}");
            sb.AppendLine();
            if (item.Estado == "Finalizado")
            {
                sb.AppendLine($"*ESTÁ PRONTO PARA RETIRADA*");
            }
            sb.AppendLine("*[Sapataria Almeida]*");

            string mensagem = sb.ToString();

            // 2) Cria um ScrollViewer com TextBlock para exibir tudo
            var scroll = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = mensagem,
                    TextWrapping = TextWrapping.Wrap
                },
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 300
            };

            // 3) Cria e exibe o dialog
            var dialog = new ContentDialog
            {
                Title = "Mensagem para Cliente",
                Content = scroll,
                PrimaryButtonText = "Copiar",
                CloseButtonText = "Fechar",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync().AsTask();

            // 4) Se clicou “Copiar”, manda para a área de transferência
            if (result == ContentDialogResult.Primary)
            {
                var dp = new DataPackage();
                dp.SetText(mensagem);
                Clipboard.SetContent(dp);
            }
        }


        private static IntPtr GetWindowHandle(FrameworkElement element)
        {
            var window = element.XamlRoot?.Content as UIElement;
            if (window == null) return IntPtr.Zero;

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            return hwnd;
        }



    }
}
