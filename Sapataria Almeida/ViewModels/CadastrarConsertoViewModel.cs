using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
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
        }

        private void OnSearchCliente()
        {
            ClientesEncontrados.Clear();
            var lista = _db.Clientes
                           .Where(c => c.Nome.Contains(SearchCliente)
                                    || c.Telefone.Contains(SearchCliente))
                           .ToList();
            foreach (var c in lista)
                ClientesEncontrados.Add(c);
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

            var conserto = new Conserto
            {
                ClienteId = cliente.Id,
                MetodoPagamento = MetodoPagamento,
                Sinal = float.TryParse(Sinal, out var s) ? s : 0f,
                Itens = Carrinho.ToList()
            };
            await _db.Consertos.AddAsync(conserto);
            await _db.SaveChangesAsync();

            // limpa estado
            Carrinho.Clear();
            MetodoPagamento = NomeCliente = TelefoneCliente = SearchCliente = Sinal = string.Empty;
            ClientesEncontrados.Clear();
            ClienteSelecionado = null;
            FinalizarCommand.NotifyCanExecuteChanged();
        }
    }
}
