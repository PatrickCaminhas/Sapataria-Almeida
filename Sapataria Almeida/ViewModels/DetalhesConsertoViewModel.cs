using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace Sapataria_Almeida.ViewModels
{
    public partial class DetalhesConsertoViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        [ObservableProperty] private Conserto _conserto = null!;
        public ObservableCollection<ItemConserto> Itens { get; } = new();

        public IAsyncRelayCommand<int> LoadCommand { get; }
        public decimal ValorTotal => Conserto.Itens.Sum(i => i.Valor);

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


    }
}
