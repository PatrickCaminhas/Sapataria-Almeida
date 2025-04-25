using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using System.Collections.ObjectModel;

namespace Sapataria_Almeida.ViewModels
{
    public partial class EditarItensConsertoViewModel : ObservableObject
    {
        private readonly AppDbContext _db = new AppDbContext();

        [ObservableProperty]
        private int _consertoId;

        public ObservableCollection<ItemConserto> Itens { get; } = new();

        [ObservableProperty]
        private ItemConserto? _itemSelecionado;

        [ObservableProperty]
        private string _novaDescricao = string.Empty;

        public IAsyncRelayCommand LoadItensCommand { get; }
        public IAsyncRelayCommand SaveDescricaoCommand { get; }

        public EditarItensConsertoViewModel()
        {
            LoadItensCommand = new AsyncRelayCommand(LoadItensAsync);
            SaveDescricaoCommand = new AsyncRelayCommand(SaveDescricaoAsync, CanSave);
        }

        private async Task LoadItensAsync()
        {
            Itens.Clear();
            if (ConsertoId <= 0) return;

            var itens = await _db.ItensConserto
                                 .Where(i => i.ConsertoId == ConsertoId)
                                 .ToListAsync();
            foreach (var i in itens)
                Itens.Add(i);
        }

        private bool CanSave()
            => ItemSelecionado != null
               && NovaDescricao != null;

        private async Task SaveDescricaoAsync()
        {
            if (ItemSelecionado == null) return;

            // Carrega a entidade no contexto
            var item = await _db.ItensConserto.FindAsync(ItemSelecionado.Id);
            if (item == null) return;

            item.Descricao = NovaDescricao;
            await _db.SaveChangesAsync();

            // Atualiza na lista
            ItemSelecionado.Descricao = NovaDescricao;
            NovaDescricao = string.Empty;
        }
    }
}
