using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapataria_Almeida.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.Now;
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal TotalVenda => Itens.Sum(i => i.Valor);
        public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    }
}
