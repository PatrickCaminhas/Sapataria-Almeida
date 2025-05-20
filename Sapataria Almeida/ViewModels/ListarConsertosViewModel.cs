using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sapataria_Almeida.ViewModels
{
    // Tem que ser public e ter um construtor público sem parâmetros
    public partial class ListarConsertosViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        private int _paginaAtual = 1;
        private int _tamanhoPagina = 5;
        private List<Conserto> _todosConsertos = new();

        public ObservableCollection<Conserto> Consertos { get; } = new();
        public IAsyncRelayCommand LoadConsertosCommand { get; }


        public int PaginaAtual
        {
            get => _paginaAtual;
            set => SetProperty(ref _paginaAtual, value);
        }

        public int TotalPaginas => (int)Math.Ceiling((double)_todosConsertos.Count / _tamanhoPagina);

        public IRelayCommand ProximaPaginaCommand { get; }
        public IRelayCommand PaginaAnteriorCommand { get; }

        public ListarConsertosViewModel()
        {
            LoadConsertosCommand = new AsyncRelayCommand(LoadConsertosAsync);
            ProximaPaginaCommand = new RelayCommand(ProximaPagina, PodeIrParaProximaPagina);
            PaginaAnteriorCommand = new RelayCommand(PaginaAnterior, PodeIrParaPaginaAnterior);
        }

        private async Task LoadConsertosAsync()
        {
            var lista = await _db.Consertos.Include(c => c.Cliente).Where(c => c.Estado != "Finalizado").ToListAsync();
            _todosConsertos = lista.OrderByDescending(c => c.DataAbertura).ToList();
            PaginaAtual = 1;
            AtualizarPagina();
        }

        private void AtualizarPagina()
        {
            Consertos.Clear();
            var itensPagina = _todosConsertos
                .Skip((_paginaAtual - 1) * _tamanhoPagina)
                .Take(_tamanhoPagina);

            foreach (var c in itensPagina)
                Consertos.Add(c);

            ProximaPaginaCommand.NotifyCanExecuteChanged();
            PaginaAnteriorCommand.NotifyCanExecuteChanged();
        }

        private void ProximaPagina()
        {
            if (PodeIrParaProximaPagina())
            {
                PaginaAtual++;
                AtualizarPagina();
            }
        }

        private bool PodeIrParaProximaPagina() => _paginaAtual < TotalPaginas;

        private void PaginaAnterior()
        {
            if (PodeIrParaPaginaAnterior())
            {
                PaginaAtual--;
                AtualizarPagina();
            }
        }

        private bool PodeIrParaPaginaAnterior() => _paginaAtual > 1;


    }

}
