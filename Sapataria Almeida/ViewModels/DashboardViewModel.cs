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
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        // Arrecadação Mensal
        [ObservableProperty]
        private ObservableCollection<ISeries> _monthlySeries = new();

        [ObservableProperty]
        private Axis[] _monthlyXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private Axis[] _monthlyYAxes = Array.Empty<Axis>();
        // Arrecadação Semanal
        [ObservableProperty]
        private ObservableCollection<ISeries> _weeklySeries = new();

        [ObservableProperty]
        private Axis[] _weeklyXAxes = Array.Empty<Axis>();

        [ObservableProperty]
        private Axis[] _weeklyYAxes = Array.Empty<Axis>();

        public DashboardViewModel()
        {
            // Dispara carregamento assíncrono sem bloquear a UI
           

            _ = LoadArrecadacaoSemanalAsync();
            _ = LoadArrecadacaoMensalAsync();
        }

        private async Task LoadArrecadacaoSemanalAsync()
        {
            // 1. Determinar a semana atual (segunda a domingo)
            var today = DateTime.Today;
            int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var inicioSemana = today.AddDays(-1 * diff).Date;
            var fimSemana = inicioSemana.AddDays(6).Date;

            // 2. Buscar consertos do banco dentro do intervalo
            var consertos = await _db.Consertos
                .AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioSemana && c.DataAbertura <= fimSemana)
                         || (c.DataRetirada != null && c.DataRetirada >= inicioSemana && c.DataRetirada <= fimSemana))
                .ToListAsync();

            // 3. Preparar arrays para nomes de dias e valores
            var diasDaSemana = Enumerable.Range(0, 7)
                .Select(i => inicioSemana.AddDays(i))
                .ToList();

            var labels = diasDaSemana
                .Select(d => d.ToString("ddd", new CultureInfo("pt-BR")))
                .ToArray();

            var arrecadacao = diasDaSemana
                .Select(d =>
                    consertos.Where(c => c.DataAbertura.Date == d).Sum(c => c.Sinal)
                    + consertos.Where(c => c.DataRetirada.Date == d && c.ValorPagamento > 0)
                              .Sum(c => c.ValorPagamento)
                )
                .Select(sum => (decimal)sum)
                .ToArray();

            // 4. Atualizar propriedades do gráfico na UI thread
            WeeklySeries = new ObservableCollection<ISeries>
            {
                new ColumnSeries<decimal>
                {
                    Name = "Arrecadação (R$)",
                    Values = new ObservableCollection<decimal>(arrecadacao),
                    Fill = new SolidColorPaint(SKColors.Green),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    
                    DataLabelsPosition = DataLabelsPosition.Top,
                    DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("C2", new CultureInfo("pt-BR"))
                }
            };

            WeeklyXAxes = new[]
            {
                new Axis
                {
                    Name = "Dia da Semana",
                    Labels = labels,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColors.Black),   // labels em preto
                    NamePaint   = new SolidColorPaint(SKColors.Black)    // nome do eixo em preto
                }
            };

            WeeklyYAxes = new[]
            {
                new Axis
                {
                    Name = "Valor Arrecadado (R$)",
                    Labeler = val => val.ToString("C0", new CultureInfo("pt-BR")),
                    MinLimit = 0,
                    TextSize = 12,
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    SeparatorsPaint = new SolidColorPaint(SKColors.Black),
                    NamePaint   = new SolidColorPaint(SKColors.Black)

                }
            };
        }


        private async Task LoadArrecadacaoMensalAsync()
        {
            var today = DateTime.Today;
            var inicioMes = new DateTime(today.Year, today.Month, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            var consertos = await _db.Consertos
                .AsNoTracking()
                .Where(c => (c.DataAbertura >= inicioMes && c.DataAbertura <= fimMes)
                         || (c.DataRetirada != null && c.DataRetirada >= inicioMes && c.DataRetirada <= fimMes))
                .ToListAsync();

            var diasDoMes = Enumerable.Range(1, fimMes.Day)
                .Select(d => new DateTime(today.Year, today.Month, d))
                .ToList();

            var labels = diasDoMes
                .Select(d => d.Day.ToString("00"))
                .ToArray();

            var arrecadacao = diasDoMes
                .Select(d =>
                    consertos.Where(c => c.DataAbertura.Date == d).Sum(c => c.Sinal)
                    + consertos.Where(c => c.DataRetirada.Date == d && c.ValorPagamento > 0)
                              .Sum(c => c.ValorPagamento)
                )
                .Select(sum => (decimal)sum)
                .ToArray();

            MonthlySeries = new ObservableCollection<ISeries>
    {
        new ColumnSeries<decimal>
        {
            Name = "Arrecadação do Mês (R$)",
            Values = new ObservableCollection<decimal>(arrecadacao),
            Fill = new SolidColorPaint(SKColors.Green),
            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
            DataLabelsPosition = DataLabelsPosition.Top,
            DataLabelsFormatter = point => ((ChartPoint<decimal, RoundedRectangleGeometry, LabelGeometry>)point).Model.ToString("N2", new CultureInfo("pt-BR"))
        }
    };

            MonthlyXAxes = new[]
            {
        new Axis
        {
            Name = "Dia do Mês",
            Labels = labels,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            NamePaint = new SolidColorPaint(SKColors.Black)
        }
    };

            MonthlyYAxes = new[]
            {
        new Axis
        {
            Name = "Valor Arrecadado (R$)",
            Labeler = val => val.ToString("C0", new CultureInfo("pt-BR")),
            MinLimit = 0,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            SeparatorsPaint = new SolidColorPaint(SKColors.Black),
            NamePaint = new SolidColorPaint(SKColors.Black)
        }
    };
        }




    }
}
