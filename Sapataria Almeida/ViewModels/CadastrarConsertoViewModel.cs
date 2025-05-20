using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sapataria_Almeida.ViewModels
{
    public partial class CadastroConsertoViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        // — BUSCA DE CLIENTE —
        [ObservableProperty] private string _searchCliente = string.Empty;
        public ObservableCollection<Cliente> ClientesEncontrados { get; } = new();
        [ObservableProperty] private Cliente? _clienteSelecionado;
        [ObservableProperty] private string _nomeCliente = string.Empty;
        [ObservableProperty] private string _telefoneCliente = string.Empty;

        // — INCLUSÃO DE ITEM DE CONSERTO —
        [ObservableProperty] private string _tipoConserto = string.Empty;
        [ObservableProperty] private string _valorConserto = string.Empty;
        [ObservableProperty] private string _descricao = string.Empty;
        public ObservableCollection<ItemConserto> Carrinho { get; } = new();

        // — PAGAMENTO E SINAL —
        public ObservableCollection<string> MetodosPagamento { get; } = new(
            new[] { "Dinheiro", "Pix", "Cartão" });
        [ObservableProperty] private string _metodoPagamento = string.Empty;
        [ObservableProperty] private string _sinal = string.Empty;

        // Data Final
        [ObservableProperty]
        private DateTimeOffset? _dataFinalOffset;





        // Commands
        public IRelayCommand SearchClienteCommand { get; }
        public IRelayCommand<Cliente?> SelectClienteCommand { get; }
        public IRelayCommand AddItemCommand { get; }
        public IAsyncRelayCommand FinalizarCommand { get; }

        public CadastroConsertoViewModel()
        {
            SearchClienteCommand = new RelayCommand(OnSearchCliente);
            SelectClienteCommand = new RelayCommand<Cliente?>(OnSelectCliente);
            AddItemCommand = new RelayCommand(OnAddItem);
            FinalizarCommand = new AsyncRelayCommand(OnFinalizarAsync, CanFinalizar);
            DataFinalOffset = null;
        }

        private void OnSearchCliente()
        {
            ClientesEncontrados.Clear();

            // Normaliza o termo de busca
            var termo = SearchCliente?.Trim().ToLower() ?? string.Empty;

            var lista = _db.Clientes
                .Where(c =>
                    c.Nome.ToLower().Contains(termo) ||
                    c.Telefone.Contains(SearchCliente))
                .ToList();


            foreach (var c in lista)
                ClientesEncontrados.Add(c);
        }


        partial void OnDataFinalOffsetChanged(DateTimeOffset? value)
        {
            FinalizarCommand.NotifyCanExecuteChanged();
        }


        private void OnSelectCliente(Cliente? cliente)
        {
            // Se veio seleção nula, ignora
            if (cliente is null)
                return;

            ClienteSelecionado = cliente;
            NomeCliente = cliente.Nome;
            TelefoneCliente = cliente.Telefone;
        }

        private void OnAddItem()
        {
            if (decimal.TryParse(ValorConserto, out var v) && !string.IsNullOrWhiteSpace(TipoConserto))
            {
                Carrinho.Add(new ItemConserto
                {
                    TipoConserto = TipoConserto,
                    Valor = v,
                    Descricao = string.IsNullOrWhiteSpace(Descricao) ? null : Descricao
                });
                TipoConserto = ValorConserto = Descricao = string.Empty;
                FinalizarCommand.NotifyCanExecuteChanged();
            }
        }

        public bool TryGetClienteConflito(out Cliente? existente)
        {
            existente = _db.Clientes
                            .AsNoTracking()
                            .FirstOrDefault(c => c.Telefone == TelefoneCliente);
            return existente != null && existente.Nome != NomeCliente;
        }

        private bool CanFinalizar()
        {
            return Carrinho.Any()
                && !string.IsNullOrWhiteSpace(MetodoPagamento)
                && !string.IsNullOrWhiteSpace(TelefoneCliente);
        }
        private async Task<int> GerarProximoIdAsync(DateTime dataConserto)
        {
            int ano = dataConserto.Year;
            int mes = dataConserto.Month;

            int baseYm = ano * 100000 + mes * 1000;

            int maxIdNoMes = await _db.Consertos
                .Where(c => c.Id >= baseYm && c.Id < baseYm + 10000)
                .MaxAsync(c => (int?)c.Id) ?? baseYm;

            int seqAtual = maxIdNoMes % 10000;
            int proxSeq = seqAtual + 1;

            if (proxSeq > 9999)
                throw new InvalidOperationException("Limite de IDs atingido para o mês.");

            return baseYm + proxSeq;
        }




        private async Task OnFinalizarAsync()
        {
            Cliente cliente;
            if (ClienteSelecionado != null)
            {
                cliente = ClienteSelecionado;
            }
            else if (await _db.Clientes.AnyAsync(c => c.Telefone == TelefoneCliente))
            {
                cliente = await _db.Clientes.FirstAsync(c => c.Telefone == TelefoneCliente);
            }
            else
            {
                cliente = new Cliente { Nome = NomeCliente, Telefone = TelefoneCliente };
                await _db.Clientes.AddAsync(cliente);
                await _db.SaveChangesAsync();
            }
            var dataBaseId = DataFinalOffset?.DateTime.Date ?? DateTime.Now;
            var novoId = await GerarProximoIdAsync(dataBaseId);

            var estado = "Em Andamento";
            var valorSinal = decimal.TryParse(Sinal, out var s) ? s : 0m;
            var valorMinimoSinal = Carrinho.Sum(i => i.Valor) * 0.5m;
            var verificarPendente = false;
            if (Carrinho.Any(i => i.Valor == 0))
            {
                verificarPendente = true;
            }
            if (verificarPendente == false && (valorSinal == 0 || valorSinal < valorMinimoSinal))
            {
                estado = "Aguardando Sinal";
            }
            else if (verificarPendente == true)
            {
                estado = "Em Orçamento";
            }
            var dataSelecionada = DataFinalOffset?.DateTime.Date;

            DateTime dataFinal = (dataSelecionada.HasValue && dataSelecionada.Value >= DateTime.Today)
                ? dataSelecionada.Value.AddHours(18)
                : DateTime.Today.AddHours(18);

            var conserto = new Conserto
            {
                ClienteId = cliente.Id,
                MetodoPagamento = MetodoPagamento,
                Sinal = valorSinal, // Changed from 0f to 0m
                Estado = estado,
                DataFinal = dataFinal,

                Itens = Carrinho.ToList()
            };
            await _db.Consertos.AddAsync(conserto);
            await _db.SaveChangesAsync();

            // limpa estado
            Carrinho.Clear();
            MetodoPagamento = NomeCliente = TelefoneCliente = SearchCliente = Sinal = string.Empty;
            ClientesEncontrados.Clear();
            ClienteSelecionado = null;
            DataFinalOffset = null;

            FinalizarCommand.NotifyCanExecuteChanged();
        }


    }
}
