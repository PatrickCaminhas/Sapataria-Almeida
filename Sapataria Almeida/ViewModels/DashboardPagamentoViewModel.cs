using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sapataria_Almeida.ViewModels
{
    public partial class DashboardPagamentoViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();
        // 1) Novas propriedades para gráficos de Métodos de Pagamento
        [ObservableProperty]
        private ObservableCollection<ISeries> _consertoPaymentSeries = new();

        [ObservableProperty]
        private Axis[] _consertoPaymentXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private ObservableCollection<ISeries> _vendaPaymentSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> _allPaymentSeries = new();

        [ObservableProperty]
        private string[] _vendaPaymentLabels = Array.Empty<string>();

        [ObservableProperty]
        private string[] _allPaymentLabels = Array.Empty<string>();

        // 2) Chamada aos novos carregamentos no construtor

        public DashboardPagamentoViewModel()
        {
            // Dispara carregamento assíncrono sem bloquear a UI
            _ = LoadMetodoPagamentoConsertoAsync();
            _ = LoadMetodoPagamentoVendaAsync();
            _ = LoadMetodoPagamentoGeralAsync();

        }

        // 3) Carrega proporção de métodos de pagamento dos Consertos
        private async Task LoadMetodoPagamentoConsertoAsync()
        {
            var consertos = await _db.Consertos.AsNoTracking().ToListAsync();

            // Lista de todos métodos encontrados
            var metodos = consertos
                .Select(c => c.MetodoPagamentoSinal)
                .Concat(consertos.Select(c => c.MetodoPagamentoFinal))
                .Distinct()
                .ToArray();

            // Contagens por método
            var sinalCounts = metodos.Select(m => consertos.Count(c => c.MetodoPagamentoSinal == m)).ToArray();
            var finalCounts = metodos.Select(m => consertos.Count(c => c.MetodoPagamentoFinal == m)).ToArray();

            ConsertoPaymentSeries = new ObservableCollection<ISeries>
    {
        new ColumnSeries<int>
        {
            Name = "Sinal",
            Values = new ObservableCollection<int>(sinalCounts),
            DataLabelsPosition = DataLabelsPosition.Top
        },
        new ColumnSeries<int>
        {
            Name = "Final",
            Values = new ObservableCollection<int>(finalCounts),
            DataLabelsPosition = DataLabelsPosition.Top
        }
    };

            ConsertoPaymentXAxes = new[]
            {
        new Axis
        {
            Name = "Método",
            Labels = metodos,
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            NamePaint = new SolidColorPaint(SKColors.Black)
        }
    };
        }

        // 4) Carrega proporção de métodos de pagamento das Vendas
        private async Task LoadMetodoPagamentoVendaAsync()
        {
            var vendas = await _db.Vendas.AsNoTracking().ToListAsync();
            var grouped = vendas.GroupBy(v => v.MetodoPagamento)
                                .Select(g => new { Metodo = g.Key, Count = g.Count() })
                                .ToArray();

            VendaPaymentLabels = grouped.Select(g => g.Metodo).ToArray();

            VendaPaymentSeries = new ObservableCollection<ISeries>(
                grouped.Select(g =>
                    new PieSeries<int>
                    {
                        Name = g.Metodo,
                        Values = new[] { g.Count },
                        DataLabelsPosition = PolarLabelsPosition.Middle
                    } as ISeries
                )
            );
        }

        // 5) Carrega proporção geral somada
        private async Task LoadMetodoPagamentoGeralAsync()
        {
            var consertos = await _db.Consertos.AsNoTracking().ToListAsync();
            var vendas = await _db.Vendas.AsNoTracking().ToListAsync();

            // Dicionário de contagem geral
            var totalCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var c in consertos)
            {
                if (!string.IsNullOrEmpty(c.MetodoPagamentoSinal))
                    totalCounts[c.MetodoPagamentoSinal] = totalCounts.GetValueOrDefault(c.MetodoPagamentoSinal) + 1;
                if (!string.IsNullOrEmpty(c.MetodoPagamentoFinal))
                    totalCounts[c.MetodoPagamentoFinal] = totalCounts.GetValueOrDefault(c.MetodoPagamentoFinal) + 1;
            }
            foreach (var v in vendas)
            {
                if (!string.IsNullOrEmpty(v.MetodoPagamento))
                    totalCounts[v.MetodoPagamento] = totalCounts.GetValueOrDefault(v.MetodoPagamento) + 1;
            }

            var methods = totalCounts.Keys.ToArray();
            var counts = methods.Select(m => totalCounts[m]).ToArray();

            AllPaymentLabels = methods;

            AllPaymentSeries = new ObservableCollection<ISeries>(
                methods.Select((m, i) =>
                    new PieSeries<int>
                    {
                        Name = m,
                        Values = new[] { counts[i] },
                        DataLabelsPosition = PolarLabelsPosition.Middle
                    } as ISeries
                )
            );
        }


    }
}
