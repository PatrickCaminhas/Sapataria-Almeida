﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class GraficosAnuaisViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        // Arrecadação Semanal
        [ObservableProperty]
        private ObservableCollection<ISeries> _annualSeries = new();

        [ObservableProperty]
        private Axis[] _annualXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private Axis[] _annualYAxes = Array.Empty<Axis>();

        // 1) Novas propriedades para gráficos de Métodos de Pagamento
        [ObservableProperty]
        private ObservableCollection<ISeries> _annualConsertoPaymentSeries = new();

        [ObservableProperty]
        private Axis[] _annualConsertoPaymentXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private ObservableCollection<ISeries> _annualVendaPaymentSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> _annualAllPaymentSeries = new();

        [ObservableProperty]
        private string[] _annualVendaPaymentLabels = Array.Empty<string>();

        [ObservableProperty]
        private string[] _annualAllPaymentLabels = Array.Empty<string>();


        public GraficosAnuaisViewModel()
        {
            // Carrega os gráficos de arrecadação semanal
            _ = LoadArrecadacaoAnualAsync();
            _ = LoadMetodoPagamentoConsertoAsync();
            _ = LoadMetodoPagamentoVendaAsync();
            _ = LoadMetodoPagamentoGeralAsync();

        }
        private (DateTime inicio, DateTime fim) GetAnoAtual()
        {
            var today = DateTime.Today;
            var inicio = new DateTime(today.Year, 1, 1);
            var fim = new DateTime(today.Year, 12, 31);
            return (inicio, fim);
        }

        private async Task LoadArrecadacaoAnualAsync()
        {
            var (inicioAno, fimAno) = GetAnoAtual();

            // 1) Buscar todos os consertos do ano
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioAno && c.DataAbertura <= fimAno)
                         || (c.DataRetirada >= inicioAno && c.DataRetirada <= fimAno))
                .ToListAsync();

            // 2) Buscar todas as vendas do ano (include de Itens para calcular TotalVenda)
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicioAno && v.DataVenda <= fimAno)
                .ToListAsync();

            // 3) Criar lista dos 12 meses do ano atual, com início e fim de cada mês
            var meses = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Inicio = new DateTime(inicioAno.Year, m, 1),
                    Fim = new DateTime(inicioAno.Year, m, DateTime.DaysInMonth(inicioAno.Year, m))
                })
                .ToList();

            // 4) Labels para o eixo X: "Jan", "Fev", "Mar", etc. em pt-BR
            var labels = meses
                .Select(m => m.Inicio.ToString("MMM", new CultureInfo("pt-BR")))
                .ToArray();

            // 5) Para cada mês, somar:
            //    • Consertos abertos no mês: c.Sinal (quando DataAbertura está dentro do intervalo do mês)
            //    • Consertos retirados no mês: c.ValorPagamento (quando DataRetirada está dentro do intervalo do mês)
            //    • Vendas realizadas no mês: v.TotalVenda (quando DataVenda está dentro do intervalo do mês)
            var values = meses.Select(m =>
                // soma de sinais dos consertos abertos naquele mês
                consertos.Where(c => c.DataAbertura.Date >= m.Inicio && c.DataAbertura.Date <= m.Fim).Sum(c => c.Sinal)
              // + soma dos pagamentos finais dos consertos retirados naquele mês
              + consertos.Where(c => c.DataRetirada.Date >= m.Inicio && c.DataRetirada.Date <= m.Fim).Sum(c => c.ValorPagamento)
              // + soma do TotalVenda das vendas realizadas naquele mês
              + vendas.Where(v => v.DataVenda.Date >= m.Inicio && v.DataVenda.Date <= m.Fim).Sum(v => v.TotalVenda)
            )
            .Select(s => (decimal)s)
            .ToArray();

            // 6) Atualizar AnnualSeries e eixos
            AnnualSeries = new ObservableCollection<ISeries>
    {
        new ColumnSeries<decimal>
        {
            Name = "Arrecadação do Ano (R$)",
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

            AnnualXAxes = new[] { new Axis { Name = "Mês", Labels = labels } };
            AnnualYAxes = new[]
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
            var (inicioAno, fimAno) = GetAnoAtual();
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioAno && c.DataAbertura <= fimAno)
                         || (c.DataRetirada >= inicioAno && c.DataRetirada <= fimAno))
                .ToListAsync();

            var metodos = consertos.Select(c => c.MetodoPagamentoSinal)
                .Concat(consertos.Select(c => c.MetodoPagamentoFinal))
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct().ToArray();

            var sinais = metodos.Select(m => (decimal)consertos.Where(c => c.MetodoPagamentoSinal == m).Sum(c => c.Sinal)).ToArray();
            var finais = metodos.Select(m => (decimal)consertos.Where(c => c.MetodoPagamentoFinal == m).Sum(c => c.ValorPagamento)).ToArray();

            AnnualConsertoPaymentSeries = new ObservableCollection<ISeries>
        {
            new ColumnSeries<decimal>
            {
                Name = "Sinal",
                Values = new ObservableCollection<decimal>(sinais),
                DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR")),
                DataLabelsPosition = DataLabelsPosition.Top            },
            new ColumnSeries<decimal>
            { Name = "Final",
                Values = new ObservableCollection<decimal>(finais),
                DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR")),
                DataLabelsPosition = DataLabelsPosition.Top            }
        };
            AnnualConsertoPaymentXAxes = new[] { new Axis { Name = "Método", Labels = metodos } };
        }

        private async Task LoadMetodoPagamentoVendaAsync()
        {
            var (inicioAno, fimAno) = GetAnoAtual();
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicioAno && v.DataVenda <= fimAno)
                .ToListAsync();

            var grouped = vendas.GroupBy(v => v.MetodoPagamento)
                .Select(g => new { Metodo = g.Key, Total = g.Sum(v => v.TotalVenda) })
                .Where(x => !string.IsNullOrEmpty(x.Metodo)).ToArray();

            AnnualVendaPaymentSeries = new ObservableCollection<ISeries>(
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
            var (inicioAno, fimAno) = GetAnoAtual();
            var consertos = await _db.Consertos.AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioAno && c.DataAbertura <= fimAno)
                         || (c.DataRetirada >= inicioAno && c.DataRetirada <= fimAno))
                .ToListAsync();
            var vendas = await _db.Vendas.AsNoTracking()
                .Include(v => v.Itens)
                .Where(v => v.DataVenda >= inicioAno && v.DataVenda <= fimAno)
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

            AnnualAllPaymentSeries = new ObservableCollection<ISeries>(
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
