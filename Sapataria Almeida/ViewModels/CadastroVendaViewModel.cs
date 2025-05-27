using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;

using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.ViewModels
{
    public partial class CadastroVendaViewModel : ObservableObject
    {
        private string _tipoProduto = string.Empty;
        private string _valor = string.Empty;
        private string _metodoPagamento = string.Empty;
        private readonly AppDbContext _db = new AppDbContext();




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
            AdicionarItemCommand = new RelayCommand(AdicionarItem);
            FinalizarVendaCommand = new AsyncRelayCommand(FinalizarVendaAsync);
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
        public ObservableCollection<string> TiposProdutos { get; } = new ObservableCollection<string>
        {"Sapato Masculino", "Sapato Feminino", "Bolsa", "Cinto" };

        public ObservableCollection<string> MetodosPagamento { get; } = new ObservableCollection<string>
         { "Dinheiro", "Pix", "Cartão de Credito", "Cartão de Debito", "Cheque" };
   
    }


}