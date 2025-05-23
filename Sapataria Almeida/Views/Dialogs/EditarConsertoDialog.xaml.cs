using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.Views.Dialogs
{
    public sealed partial class EditarConsertoDialog : ContentDialog
    {
        public Conserto Conserto { get; }

        public EditarConsertoDialog(Conserto conserto)
        {
            InitializeComponent();

            Conserto = conserto;
            DataContext = this;

            // Inicializa DatePicker e min date
            var hoje = DateTime.Today;
            var hojeOffset = new DateTimeOffset(hoje);

            DataFinalPicker.MinYear = hojeOffset;
            DataFinalPicker.Date =
                conserto.DataFinal > DateTime.MinValue
                    ? new DateTimeOffset(conserto.DataFinal.Date)
                    : hojeOffset;

            // Começa com botão habilitado (a data inicial já é válida)
            this.IsPrimaryButtonEnabled = true;

            // Popula combo estado (sua lógica existente)…
            var opcoes = new List<string> { Conserto.Estado };
            if (Conserto.Estado == "Aberto")
                opcoes.Add("Em Andamento");
                opcoes.Add("Esperando orçamento");
            if (Conserto.Estado != "Finalizado")
                opcoes.Add("Finalizado");
            EstadoCombo.ItemsSource = opcoes;
            EstadoCombo.SelectedItem = Conserto.Estado;

            if (Conserto.Estado == "Finalizado" || Conserto.Estado == "Retirado")
            {
                DataFinalPicker.IsEnabled = false;
                SinalBox.IsEnabled = false;
                EstadoCombo.IsEnabled = false;
            }
        }

        // Evento disparado sempre que o usuário muda a data no DatePicker
        private void DataFinalPicker_DateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            var picker = (DatePicker)sender;
            var selecionada = args.NewDate.DateTime.Date;
            if (selecionada < DateTime.Today)
            {
                ErrorMessage.Visibility = Visibility.Visible;
                this.IsPrimaryButtonEnabled = false;
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Collapsed;
                this.IsPrimaryButtonEnabled = true;
            }
        }


        private void OnSaveClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Redundante; o botão só fica habilitado se a data for válida,
            // mas mantemos a checagem por segurança.
            var sel = DataFinalPicker.Date.DateTime.Date;
            if (sel < DateTime.Today)
            {
                args.Cancel = true;
                return;
            }

            Conserto.DataFinal = sel;

            if (decimal.TryParse(SinalBox.Text, out var sinal))
                Conserto.Sinal = sinal;

            if (EstadoCombo.SelectedItem is string novoEstado)
                Conserto.Estado = novoEstado;
        }
    }
}
