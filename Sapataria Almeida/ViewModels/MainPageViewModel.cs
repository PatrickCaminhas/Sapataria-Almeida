using System;
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

        public MainPageViewModel(IRepositorioDados repositorio)
        {
            _repositorio = repositorio;
            _ctx = new AppDbContext();

            // Exemplo estático; troque pela sua lógica de leitura de banco
            ConsertosEmAndamento = 8;
            ConsertosAguardandoOrcamento = 3;
            ConsertosFinalizados = 5;
            ConsertosAtrasados = 2;
            ConsertosVencemHoje = 1;
            // Carrega notificações não lidas do banco
            _ = CarregarAlertasAsync();

        }

        private async Task CarregarAlertasAsync()
        {
            var hoje = DateTime.Today;

            // Carrega todos os consertos
            var consertos = await _ctx.Consertos.ToListAsync();

            // 1) Finalizados
            ConsertosFinalizados = consertos.Count(c =>
                string.Equals(c.Estado, "Finalizado", StringComparison.OrdinalIgnoreCase));

            // 2) Atrasados e vencendo hoje
            ConsertosAtrasados = consertos.Count(c => c.DataFinal.Date < hoje);
            ConsertosVencemHoje = consertos.Count(c => c.DataFinal.Date == hoje);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
