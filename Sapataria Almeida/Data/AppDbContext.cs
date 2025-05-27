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
            CriarSenhaAdmin();
            CriarProdutoVenda();
            CriarProdutoConserto();
        }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Conserto> Consertos { get; set; }
        public DbSet<ItemConserto> ItensConserto { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Administracao> Administracao { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<ProdutoConserto> ProdutoConserto { get; set; }



        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            "Database", "vendas.db");
            Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }
        protected void CriarSenhaAdmin()
        {
            if (!Administracao.Any())
            {
                var admin = new Administracao
                {
                    celular = "(00) 9 0000-0000",
                    Senha = "Admin" // Senha padrão, deve ser alterada pelo usuário
                };
                Administracao.Add(admin);
                SaveChanges();
            }
        }

        protected void CriarProdutoVenda()
        {
            if (!Produto.Any())
            {
                var produtos = new List<Produto>
                {
                    new Produto { Nome = "Sapato Feminino"},
                    new Produto { Nome = "Sapato Masculino"},
                    new Produto { Nome = "Sapato Social"},
                    new Produto { Nome = "Tênis Esportivo"},
                    new Produto { Nome = "Bota de Couro" },
                    new Produto { Nome = "Sapatilha Feminina"},
                    new Produto { Nome = "Bolsa" },
                    new Produto { Nome = "Cinto" },


                };
                Produto.AddRange(produtos);
                SaveChanges();
            }
        }

        protected void CriarProdutoConserto()
        {
            if (!ProdutoConserto.Any())
            {
                var produtos = new List<ProdutoConserto>
                {
                    new ProdutoConserto { Nome = "Sapato Feminino"},
                    new ProdutoConserto { Nome = "Sapato Masculino"},
                    new ProdutoConserto { Nome = "Sapato Social"},
                    new ProdutoConserto { Nome = "Tênis Esportivo"},
                    new ProdutoConserto { Nome = "Bota de Couro" },
                    new ProdutoConserto { Nome = "Sapatilha Feminina"},
                    new ProdutoConserto { Nome = "Bolsa" },
                    new ProdutoConserto { Nome = "Cinto" },
                    new ProdutoConserto { Nome = "Outro" },

                };
                ProdutoConserto.AddRange(produtos);
                SaveChanges();
            }
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
