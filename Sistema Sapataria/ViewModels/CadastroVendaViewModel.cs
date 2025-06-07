using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;
using Sistema_Sapataria.Data;
using Sistema_Sapataria.Models;
using Sistema_Sapataria.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_Sapataria.ViewModels
{
    public partial class CadastroVendaViewModel : ObservableObject
    {
        private readonly RepositorioDados _repositorio;

        private string _tipoProduto = string.Empty;
        private string _valor = string.Empty;
        private string _metodoPagamento = string.Empty;
        private readonly AppDbContext _db = new AppDbContext();
        public ObservableCollection<string> TiposProdutos { get; }



        public string TipoProduto
        {
            get => _tipoProduto;
            set => SetProperty(ref _tipoProduto, value);
        }
  

        public string Valor
        {
            get => _valor;
            set => SetProperty(ref _valor, value);
        }

        public string MetodoPagamento
        {
            get => _metodoPagamento;
            set => SetProperty(ref _metodoPagamento, value);
        }



        public ObservableCollection<ItemVenda> Carrinho { get; } = new();

        public IRelayCommand AdicionarItemCommand { get; }
        public IAsyncRelayCommand FinalizarVendaCommand { get; }

        public CadastroVendaViewModel()
        {
            _tipoProduto = string.Empty;
            _valor = string.Empty;
            _metodoPagamento = string.Empty;
            _repositorio = new RepositorioDados(_db);
            AdicionarItemCommand = new RelayCommand(AdicionarItem);
            FinalizarVendaCommand = new AsyncRelayCommand(FinalizarVendaAsync);
            TiposProdutos = new ObservableCollection<string>();
            LoadTiposProdutos();

        }

        private void AdicionarItem()
        {
            if (decimal.TryParse(Valor, out decimal valorDecimal) && !string.IsNullOrEmpty(TipoProduto))
            {
                Carrinho.Add(new ItemVenda { TipoProduto = TipoProduto, Valor = valorDecimal });
                Valor = "";
                TipoProduto = "";
            }
        }
        private void LoadTiposProdutos()
        {
            var lista = _repositorio.GetProdutos();
            foreach (var prod in lista)
            {
                TiposProdutos.Add(prod.Nome);
            }
        }

        private async Task FinalizarVendaAsync()
        {
            if (Carrinho.Count == 0 || string.IsNullOrEmpty(MetodoPagamento))
            {
                // avisar usuário…
                return;
            }

            var novaVenda = new Venda
            {
                MetodoPagamento = this.MetodoPagamento,
                Itens = this.Carrinho.ToList()
            };

            await _db.Vendas.AddAsync(novaVenda);
            await _db.SaveChangesAsync();

            Carrinho.Clear();
            MetodoPagamento = string.Empty;
        }
        //public ObservableCollection<string> TiposProdutos { get; } = new ObservableCollection<string>
        //{"Sapato Masculino", "Sapato Feminino", "Bolsa", "Cinto" };
    

        public ObservableCollection<string> MetodosPagamento { get; } = new ObservableCollection<string>
         { "Dinheiro", "Pix", "Cartão de Credito", "Cartão de Debito", "Cheque" };
   
    }


}