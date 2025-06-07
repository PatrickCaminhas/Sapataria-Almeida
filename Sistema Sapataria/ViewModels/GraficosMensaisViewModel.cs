using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Sistema_Sapataria.Data;
using Sistema_Sapataria.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_Sapataria.ViewModels
{
    public partial class GraficosMensaisViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        // Arrecadação Semanal
        [ObservableProperty]
        private ObservableCollection<ISeries> _monthlySeries = new();

        [ObservableProperty]
        private Axis[] _monthlyXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private Axis[] _monthlyYAxes = Array.Empty<Axis>();

        // 1) Novas propriedades para gráficos de Métodos de Pagamento
        [ObservableProperty]
        private ObservableCollection<ISeries> _monthlyConsertoPaymentSeries = new();

        [ObservableProperty]
        private Axis[] _monthlyConsertoPaymentXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private ObservableCollection<ISeries> _monthlyVendaPaymentSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> _monthlyAllPaymentSeries = new();

        [ObservableProperty]
        private string[] _monthlyVendaPaymentLabels = Array.Empty<string>();

        [ObservableProperty]
        private string[] _monthlyAllPaymentLabels = Array.Empty<string>();


        public GraficosMensaisViewModel()
        {
            // Carrega os gráficos de arrecadação semanal
            _ = LoadArrecadacaoMensalAsync();
            _ = LoadMetodoPagamentoConsertoAsync();
            _ = LoadMetodoPagamentoVendaAsync();
            _ = LoadMetodoPagamentoGeralAsync();

        }
        private (DateTime inicio, DateTime fim) GetMesAtual()
        {
            var today = DateTime.Today;
            var inicio = new DateTime(today.Year, today.Month, 1);
            var fim = inicio.AddMonths(1).AddDays(-1);
            return (inicio, fim);
        }

        private async Task LoadArrecadacaoMensalAsync()
        {
            var (inicioMes, fimMes) = GetMesAtual();

            // 1) Buscar todos os consertos do mês
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioMes && c.DataAbertura <= fimMes)
                         || (c.DataRetirada >= inicioMes && c.DataRetirada <= fimMes))
                .ToListAsync();

            // 2) Buscar todas as vendas do mês (incluindo os itens para calcular TotalVenda)
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicioMes && v.DataVenda <= fimMes)
                .ToListAsync();

            // 3) Criar lista com cada dia do mês atual
            var diasDoMes = Enumerable.Range(1, fimMes.Day)
                .Select(d => inicioMes.AddDays(d - 1))
                .ToList();

            // 4) Labels (1, 2, 3, …) para o eixo X
            var labels = diasDoMes
                .Select(d => d.Day.ToString("00"))
                .ToArray();

            // 5) Para cada dia, somar:
            //    • Consertos abertos naquele dia (c.DataAbertura == d): c.Sinal
            //    • Consertos retirados naquele dia (c.DataRetirada == d): c.ValorPagamento
            //    • Vendas realizadas naquele dia (v.DataVenda == d): v.TotalVenda
            var values = diasDoMes.Select(d =>
                consertos.Where(c => c.DataAbertura.Date == d).Sum(c => c.Sinal)
              + consertos.Where(c => c.DataRetirada.Date == d).Sum(c => c.ValorPagamento)
              + vendas.Where(v => v.DataVenda.Date == d).Sum(v => v.TotalVenda)
            )
            .Select(s => (decimal)s)
            .ToArray();

            // 6) Atualizar a série e eixos do gráfico
            MonthlySeries = new ObservableCollection<ISeries>
    {
        new ColumnSeries<decimal>
        {
            Name = "Arrecadação do Mês (R$)",
            Values = new ObservableCollection<decimal>(values),
            Fill = new SolidColorPaint(SKColors.Green),
            DataLabelsPosition = DataLabelsPosition.Top,
            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
            DataLabelsFormatter = point =>
                ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point)
                .Model
                .ToString("N2", new CultureInfo("pt-BR"))
        }
    };

            MonthlyXAxes = new[]
            {
        new Axis { Name = "Dia do Mês", Labels = labels }
    };
            MonthlyYAxes = new[]
            {
        new Axis
        {
            Name = "Valor (R$)",
            Labeler = v => v.ToString("C0", CultureInfo.GetCultureInfo("pt-BR")),
            MinLimit = 0
        }
    };
        }


        private async Task LoadMetodoPagamentoConsertoAsync()
        {
            var (inicio, fim) = GetMesAtual();
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicio && c.DataAbertura <= fim)
                         || (c.DataRetirada >= inicio && c.DataRetirada <= fim))
                .ToListAsync();

            var metodos = consertos.Select(c => c.MetodoPagamentoSinal)
                .Concat(consertos.Select(c => c.MetodoPagamentoFinal))
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct()
                .ToArray();

            var sinais = metodos.Select(m =>
                consertos.Where(c => c.MetodoPagamentoSinal == m).Sum(c => c.Sinal)
            ).Select(v => (decimal)v).ToArray();

            var finais = metodos.Select(m =>
                consertos.Where(c => c.MetodoPagamentoFinal == m).Sum(c => c.ValorPagamento)
            ).Select(v => (decimal)v).ToArray();

            MonthlyConsertoPaymentSeries = new ObservableCollection<ISeries>
        {
            new ColumnSeries<decimal>
            {
                Name = "Sinal",
                Values = new ObservableCollection<decimal>(sinais),
                DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR")),
                DataLabelsPosition = DataLabelsPosition.Top
            },
            new ColumnSeries<decimal>
            {
                Name = "Final",
                Values = new ObservableCollection<decimal>(finais),
                DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR")),
                DataLabelsPosition = DataLabelsPosition.Top
            }
        };
            MonthlyConsertoPaymentXAxes = new[] { new Axis { Name = "Método", Labels = metodos } };
        }

        private async Task LoadMetodoPagamentoVendaAsync()
        {
            var (inicio, fim) = GetMesAtual();
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim)
                .ToListAsync();

            var grouped = vendas.GroupBy(v => v.MetodoPagamento)
                .Select(g => new { Metodo = g.Key, Total = g.Sum(v => v.TotalVenda) })
                .Where(x => !string.IsNullOrEmpty(x.Metodo))
                .ToArray();

            MonthlyVendaPaymentSeries = new ObservableCollection<ISeries>(
                grouped.Select(g =>
                    new PieSeries<decimal>
                    {
                        Name = g.Metodo,
                        Values = new[] { g.Total },
                        DataLabelsPosition = PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => ((ChartPoint<decimal, DoughnutGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))
                    } as ISeries
                )
            );
        }

        private async Task LoadMetodoPagamentoGeralAsync()
        {
            var (inicio, fim) = GetMesAtual();
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicio && c.DataAbertura <= fim)
                         || (c.DataRetirada >= inicio && c.DataRetirada <= fim))
                .ToListAsync();
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim)
                .ToListAsync();

            var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var c in consertos)
            {
                if (!string.IsNullOrEmpty(c.MetodoPagamentoSinal))
                    dict[c.MetodoPagamentoSinal] = dict.GetValueOrDefault(c.MetodoPagamentoSinal) + c.Sinal;
                if (!string.IsNullOrEmpty(c.MetodoPagamentoFinal))
                    dict[c.MetodoPagamentoFinal] = dict.GetValueOrDefault(c.MetodoPagamentoFinal) + c.ValorPagamento;
            }
            foreach (var v in vendas)
            {
                if (!string.IsNullOrEmpty(v.MetodoPagamento))
                    dict[v.MetodoPagamento] = dict.GetValueOrDefault(v.MetodoPagamento) + v.TotalVenda;
            }

            MonthlyAllPaymentSeries = new ObservableCollection<ISeries>(
                dict.Select(kv =>
                    new PieSeries<decimal>
                    {
                        Name = kv.Key,
                        Values = new[] { kv.Value },
                        DataLabelsPosition = PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => ((ChartPoint<decimal, DoughnutGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))
                    } as ISeries
                )
            );
        }
    }
}
