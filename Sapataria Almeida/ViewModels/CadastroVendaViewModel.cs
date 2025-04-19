using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.ViewModels
{
    public partial class CadastroVendaViewModel : ObservableObject
    {
        private string _tipoProduto = string.Empty;
        private string _valor = string.Empty;
        private string _metodoPagamento = string.Empty;

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
        public IRelayCommand FinalizarVendaCommand { get; }

        public CadastroVendaViewModel()
        {
            _tipoProduto = string.Empty;
            _valor = string.Empty;
            _metodoPagamento = string.Empty;
            AdicionarItemCommand = new RelayCommand(AdicionarItem);
            FinalizarVendaCommand = new RelayCommand(FinalizarVenda);
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

        private void FinalizarVenda()
        {
            if (Carrinho.Count == 0 || string.IsNullOrEmpty(MetodoPagamento))
            {
                // Implementar lógica de diálogo via serviço/messaging
                return;
            }

            Carrinho.Clear();
            MetodoPagamento = "";
        }
        public ObservableCollection<string> TiposProdutos { get; } = new ObservableCollection<string>
    {
        "Sapato Masculino",
        "Sapato Feminino",
        "Bolsa",
        "Cinto"
    };

        public ObservableCollection<string> MetodosPagamento { get; } = new ObservableCollection<string>
    {
        "Dinheiro",
        "Pix",
        "Cartão"
    };
    }


}