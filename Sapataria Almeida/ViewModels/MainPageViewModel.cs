using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;    // para INotifyPropertyChanged
using System.Linq;
using System.Runtime.CompilerServices;  // CallerMemberName
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Commands;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using Sapataria_Almeida.Repositories;

namespace Sapataria_Almeida.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        private int _consertosEmAndamento;
        public int ConsertosEmAndamento
        {
            get => _consertosEmAndamento;
            set { _consertosEmAndamento = value; OnPropertyChanged(); }
        }

        private int _consertosAguardandoOrcamento;
        public int ConsertosAguardandoOrcamento
        {
            get => _consertosAguardandoOrcamento;
            set { _consertosAguardandoOrcamento = value; OnPropertyChanged(); }
        }

        private int _consertosFinalizados;
        public int ConsertosFinalizados
        {
            get => _consertosFinalizados;
            set { _consertosFinalizados = value; OnPropertyChanged(); }
        }

        private int _consertosAtrasados;
        public int ConsertosAtrasados
        {
            get => _consertosAtrasados;
            set { _consertosAtrasados = value; OnPropertyChanged(); }
        }

        private int _consertosVencemHoje;
        public int ConsertosVencemHoje
        {
            get => _consertosVencemHoje;
            set { _consertosVencemHoje = value; OnPropertyChanged(); }
        }

        public ObservableCollection<NotificacaoViewModel> AlertasDoDia { get; }
         = new ObservableCollection<NotificacaoViewModel>();

        private readonly IRepositorioDados _repositorio;
        // Construtor onde você carrega os valores reais

        private readonly AppDbContext _ctx;


        private Dictionary<DateTime, List<Conserto>> _consertosPorDia = new();
        public Dictionary<DateTime, List<Conserto>> ConsertosPorDia
        {
            get => _consertosPorDia;
            private set { _consertosPorDia = value; OnPropertyChanged(); }
        }

        public MainPageViewModel(IRepositorioDados repositorio)
        {
            _repositorio = repositorio;
            _ctx = new AppDbContext();

            // Exemplo estático; troque pela sua lógica de leitura de banco
            ConsertosEmAndamento = 98;
            ConsertosAguardandoOrcamento = 93;
            ConsertosFinalizados = 95;
            ConsertosAtrasados = 92;
            ConsertosVencemHoje = 99;
            // Carrega notificações não lidas do banco
            _ = CarregarAlertasAsync();

        }

        private async Task CarregarAlertasAsync()
        {
            var hoje = DateTime.Today;

            // Carrega todos os consertos
            var consertos = await _ctx.Consertos
                .Include(c => c.Cliente)
                .ToListAsync();
            // Consertos abertos
            var estadosDesejados = new[] { "Em Aberto", "Em Andamento", "Aguardando Sinal" };
            ConsertosEmAndamento = consertos.Count(c => estadosDesejados.Contains(c.Estado, StringComparer.OrdinalIgnoreCase));
            //Consertos esperando orçamento
            ConsertosAguardandoOrcamento = consertos.Count(c =>
                string.Equals(c.Estado, "Em Orçamento", StringComparison.OrdinalIgnoreCase));
            //Consertos Finalizados
            ConsertosFinalizados = consertos.Count(c =>
                string.Equals(c.Estado, "Finalizado", StringComparison.OrdinalIgnoreCase));
            //Consertos atrasados (e estejam abertos)
            ConsertosAtrasados = consertos.Count(c => c.DataFinal.Date < hoje &&
            !string.Equals(c.Estado, "Finalizado", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, "Retirado", StringComparison.OrdinalIgnoreCase));
            //Consertos vencendo hoje (e estejam abertos)
            ConsertosVencemHoje = consertos.Count(c => c.DataFinal.Date == hoje &&
            !string.Equals(c.Estado, "Finalizado", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, "Retirado", StringComparison.OrdinalIgnoreCase));
            //Consertos por dia
            ConsertosPorDia = consertos.Where(c => !string.Equals(c.Estado, "Finalizado", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(c.Estado, "Retirado", StringComparison.OrdinalIgnoreCase)).
            GroupBy(c => c.DataFinal.Date).ToDictionary(g => g.Key, g => g.ToList());
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
