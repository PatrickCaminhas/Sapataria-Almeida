using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sistema_Sapataria.Data;
using Sistema_Sapataria.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace Sistema_Sapataria.ViewModels
{
    public partial class DetalhesConsertoViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        [ObservableProperty] private Conserto _conserto = null!;
        public ObservableCollection<ItemConserto> Itens { get; } = new();

        public IAsyncRelayCommand<int> LoadCommand { get; }
        public decimal ValorTotal => Conserto.Itens.Sum(i => i.Valor);

        public DateTime DataAbertura => Conserto.DataAbertura;
        public DateTime Prazo => Conserto.DataFinal;
        public decimal Sinal => Conserto.Sinal;
        public string Estado => Conserto.Estado;


        public DetalhesConsertoViewModel()
        {
            LoadCommand = new AsyncRelayCommand<int>(LoadAsync);
        }
        private async Task LoadAsync(int id)
        {
            var c = await _db.Consertos
                             .Include(x => x.Cliente)
                             // corrigido: propriedade se chama Itens, não ItensConserto
                             .Include(x => x.Itens)
                             .FirstOrDefaultAsync(x => x.Id == id);

            if (c != null)
            {
                Conserto = c;

                Itens.Clear();
                foreach (var i in c.Itens)    // aqui também Itens
                    Itens.Add(i);

                // notifica total
                OnPropertyChanged(nameof(ValorTotal));
            }
        }
        public async Task SaveChangesAsync()
        {
            // O contexto _db já está rastreando as entidades carregadas em LoadAsync
            await _db.SaveChangesAsync();
        }

        // método público para recalcular total após edição
        public void RefreshTotal() => OnPropertyChanged(nameof(ValorTotal));

        public void RefreshConserto()
        {
            // Notifica binding das propriedades DataFinal, Sinal e Estado
            OnPropertyChanged(nameof(Conserto));
            OnPropertyChanged(nameof(DataAbertura));    // se expuser separadamente
            OnPropertyChanged(nameof(Prazo));
            OnPropertyChanged(nameof(Sinal));
            OnPropertyChanged(nameof(Estado));
        }

        public void RefreshItem(ItemConserto editedItem)
        {
            var index = Itens.IndexOf(editedItem);
            if (index < 0) return;
            Itens[index] = editedItem;
        }

        public void RefreshAllItens()
        {
            var snapshot = Itens.ToList();
            Itens.Clear();
            foreach (var item in snapshot)
                Itens.Add(item);
        }



    }
}
