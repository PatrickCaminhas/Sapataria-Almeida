using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Models;
using Windows.Storage;    // precisa do namespace do WinRT



namespace Sapataria_Almeida.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            // Cria o arquivo e o esquema (tabelas) se ainda não existirem
            Database.EnsureCreated();            // :contentReference[oaicite:2]{index=2}
        }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Conserto> Consertos { get; set; }
        public DbSet<ItemConserto> ItensConserto { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            "Database", "vendas.db");
            Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura Id de Conserto para ser gerado manualmente (ValueGeneratedNever)
            modelBuilder.Entity<Conserto>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id);

                // Relacionamentos
                entity.HasMany(c => c.Itens)
                      .WithOne(i => i.Conserto)
                      .HasForeignKey(i => i.ConsertoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Cliente)
                      .WithMany()
                      .HasForeignKey(c => c.ClienteId);
            });

            modelBuilder.Entity<Venda>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasMany(v => v.Itens)
                      .WithOne(i => i.Venda)
                      .HasForeignKey(i => i.VendaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }


    }
    
}
