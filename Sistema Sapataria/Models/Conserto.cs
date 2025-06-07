using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Sapataria.Models
{
    public class Conserto
    {
        public int Id { get; set; }
        public DateTime DataAbertura { get; set; } = DateTime.Now;
        public string MetodoPagamentoSinal { get; set; } = string.Empty;
        public string MetodoPagamentoFinal { get; set; } = string.Empty;
        public decimal Sinal { get; set; }
        public DateTime DataFinal { get; set; } 

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public decimal Total => Itens.Sum(i => i.Valor);
        public string Estado { get; set; } = "Aberto";

        public decimal ValorPagamento { get; set; }

        public DateTime DataRetirada { get; set; }


        public ICollection<ItemConserto> Itens { get; set; } = new List<ItemConserto>();
    }
}
