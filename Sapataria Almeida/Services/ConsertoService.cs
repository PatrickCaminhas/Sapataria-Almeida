using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;
using Sapataria_Almeida.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sapataria_Almeida.Services
{
    public class ConsertoService
    {
        private readonly AppDbContext _context;
        public ConsertoService(AppDbContext context) => _context = context;

        public async Task<Conserto?> BuscarPorIdAsync(int id)
            => await _context.Consertos
                .Include(c => c.Cliente)
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<Conserto>> BuscarTodosAsync()
            => await _context.Consertos
                .Include(c => c.Cliente)
                .ToListAsync();
    }
}