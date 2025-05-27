using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sapataria_Almeida.ViewModels
{
    public partial class DashboardMenuViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        [ObservableProperty]
        private ObservableCollection<ISeries> _stateSeries = new();

        public DashboardMenuViewModel()
        {
            _ = LoadConsertoStatesAsync();
        }

        private async Task LoadConsertoStatesAsync()
        {
            // 1) Puxa todos os consertos sem tracking
            var consertos = await _db.Consertos
                                     .AsNoTracking()
                                     .ToListAsync();

            // 2) Pega todos os estados distintos, exceto "Finalizado"
            var estados = consertos
                .Select(c => c.Estado)
                .Where(e => !e.Equals("Finalizado", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // 3) Para cada estado restante, conta quantos consertos há
            var counts = estados
                .Select(e => consertos.Count(c =>
                    c.Estado.Equals(e, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            // 4) Cria uma série de pizza para cada estado
            var series = new ObservableCollection<ISeries>();
            for (int i = 0; i < estados.Length; i++)
            {
                series.Add(new PieSeries<int>
                {
                    Name = estados[i],
                    Values = new[] { counts[i] },
                    DataLabelsPosition = PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White) 
                });
            }

            StateSeries = series;
        }

    }
}
