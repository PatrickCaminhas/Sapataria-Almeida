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
    public partial class GraficosSemanaisViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        // Arrecadação Semanal
        [ObservableProperty]
        private ObservableCollection<ISeries> _weeklySeries = new();

        [ObservableProperty]
        private Axis[] _weeklyXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private Axis[] _weeklyYAxes = Array.Empty<Axis>();

        // 1) Novas propriedades para gráficos de Métodos de Pagamento
        [ObservableProperty]
        private ObservableCollection<ISeries> _weeklyConsertoPaymentSeries = new();

        [ObservableProperty]
        private Axis[] _weeklyConsertoPaymentXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private ObservableCollection<ISeries> _weeklyVendaPaymentSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> _weeklyAllPaymentSeries = new();

        [ObservableProperty]
        private string[] _weeklyVendaPaymentLabels = Array.Empty<string>();

        [ObservableProperty]
        private string[] _weeklyAllPaymentLabels = Array.Empty<string>();

        public GraficosSemanaisViewModel()
        {
            // Carrega os gráficos de arrecadação semanal
            _ = LoadArrecadacaoSemanalAsync();
            _ = LoadMetodoPagamentoConsertoAsync();
            _ = LoadMetodoPagamentoVendaAsync();
            _ = LoadMetodoPagamentoGeralAsync();

        }
        private async Task<(DateTime inicio, DateTime fim)> GetSemanaAtualAsync()
        {
            var today = DateTime.Today;
            int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var inicio = today.AddDays(-diff).Date;
            var fim = inicio.AddDays(6).Date;
            return (inicio, fim);
        }

        private async Task LoadArrecadacaoSemanalAsync()
        {
            var (inicioSemana, fimSemana) = await GetSemanaAtualAsync();

            // 1) Buscar todos os consertos da semana
            var consertos = await _db.Consertos
                .AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioSemana && c.DataAbertura <= fimSemana)
                         || (c.DataRetirada >= inicioSemana && c.DataRetirada <= fimSemana))
                .ToListAsync();

            // 2) Buscar todas as vendas da semana
            var vendas = await _db.Vendas
                .AsNoTracking()
                .Include(v => v.Itens) // para garantir que TotalVenda funcione
                .Where(v => v.DataVenda >= inicioSemana && v.DataVenda <= fimSemana)
                .ToListAsync();

            // 3) Preparar a lista de dias (segunda a domingo)
            var dias = Enumerable.Range(0, 7)
                                .Select(i => inicioSemana.AddDays(i))
                                .ToList();

            // 4) Montar labels (abreviações em pt-BR)
            var labels = dias
                .Select(d => d.ToString("ddd", new CultureInfo("pt-BR")))
                .ToArray();

            // 5) Para cada dia, somar:
            //    - Consertos: Sinal (quando DataAbertura == dia) + ValorPagamento (quando DataRetirada == dia)
            //    - Vendas: TotalVenda (quando DataVenda == dia)
            var values = dias.Select(d =>
                // soma dos sinais dos consertos abertos naquele dia
                consertos.Where(c => c.DataAbertura.Date == d).Sum(c => c.Sinal)
                // + soma dos pagamentos finais dos consertos retirados naquele dia
                + consertos.Where(c => c.DataRetirada.Date == d).Sum(c => c.ValorPagamento)
                // + soma do TotalVenda das vendas realizadas naquele dia
                + vendas.Where(v => v.DataVenda.Date == d).Sum(v => v.TotalVenda)
            ).Select(s => (decimal)s).ToArray();

            // 6) Atualizar a Series e os eixos do gráfico
            WeeklySeries = new ObservableCollection<ISeries>
    {
        new ColumnSeries<decimal>
        {
            Name = "Arrecadação",
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

            WeeklyXAxes = new[]
            {
        new Axis { Name = "Dia", Labels = labels }
    };
            WeeklyYAxes = new[]
            {
        new Axis
        {
            Name = "Valor (R$)",
            Labeler = v => v.ToString("C0", new CultureInfo("pt-BR")),
            MinLimit = 0
        }
    };
        }


        private async Task LoadMetodoPagamentoConsertoAsync()
        {
            var (inicio, fim) = await GetSemanaAtualAsync();
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicio && c.DataAbertura <= fim)
                         || (c.DataRetirada >= inicio && c.DataRetirada <= fim))
                .ToListAsync();

            var metodos = consertos.Select(c => c.MetodoPagamentoSinal)
                .Concat(consertos.Select(c => c.MetodoPagamentoFinal))
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct().ToArray();

            var sinais = metodos.Select(m => consertos.Where(c => c.MetodoPagamentoSinal == m).Sum(c => c.Sinal)).Select(d => (decimal)d).ToArray();
            var finais = metodos.Select(m => consertos.Where(c => c.MetodoPagamentoFinal == m).Sum(c => c.ValorPagamento)).Select(d => (decimal)d).ToArray();

            WeeklyConsertoPaymentSeries = new ObservableCollection<ISeries>
        {
            new ColumnSeries<decimal>
            {
                Name = "Sinal",
                Values = new ObservableCollection<decimal>(sinais),
                DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))
            },
            new ColumnSeries<decimal>
            {
                Name = "Final",
                Values = new ObservableCollection<decimal>(finais),
                DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))

            }
        };
            WeeklyConsertoPaymentXAxes = new[] { new Axis { Name = "Método", Labels = metodos } };
        }

        private async Task LoadMetodoPagamentoVendaAsync()
        {
            var (inicio, fim) = await GetSemanaAtualAsync();
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim)
                .ToListAsync();

            var grouped = vendas.GroupBy(v => v.MetodoPagamento)
                .Select(g => new { Metodo = g.Key, Total = g.Sum(v => v.TotalVenda) })
                .Where(x => !string.IsNullOrEmpty(x.Metodo)).ToArray();

            WeeklyVendaPaymentSeries = new ObservableCollection<ISeries>(
                grouped.Select(g => new PieSeries<decimal>
                {
                    Name = g.Metodo,
                    Values = new[] { g.Total },
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => ((ChartPoint<decimal, DoughnutGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))

                } as ISeries)
            );
        }

        private async Task LoadMetodoPagamentoGeralAsync()
        {
            var (inicio, fim) = await GetSemanaAtualAsync();
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicio && c.DataAbertura <= fim) || (c.DataRetirada >= inicio && c.DataRetirada <= fim))
                .ToListAsync();
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicio && v.DataVenda <= fim)
                .ToListAsync();

            var dict = new Dictionary<string, decimal>();
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

            WeeklyAllPaymentSeries = new ObservableCollection<ISeries>(
                dict.Select(kv => new PieSeries<decimal>
                {
                    Name = kv.Key,
                    Values = new[] { kv.Value },
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => ((ChartPoint<decimal, DoughnutGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))

                } as ISeries)
            );
        }
    }
}