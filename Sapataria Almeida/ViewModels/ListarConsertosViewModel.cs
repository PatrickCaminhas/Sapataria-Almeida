using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Sapataria_Almeida.ViewModels
{
    // Tem que ser public e ter um construtor público sem parâmetros
    public partial class ListarConsertosViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        public ObservableCollection<Conserto> Consertos { get; } = new();
        public IAsyncRelayCommand LoadConsertosCommand { get; }

        public ListarConsertosViewModel()
        {
            LoadConsertosCommand = new AsyncRelayCommand(LoadConsertosAsync);
        }

        private async Task LoadConsertosAsync()
        {
            Consertos.Clear();
            // carrega os consertos incluindo cliente
            var lista = await _db.Consertos
                                 .Include(c => c.Cliente)
                                 .ToListAsync();
            foreach (var c in lista)
                Consertos.Add(c);
        }
    }
}
