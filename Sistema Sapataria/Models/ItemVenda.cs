using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Sapataria.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }
        public string TipoProduto { get; set; } = string.Empty;
        public decimal Valor { get; set; }

        public int VendaId { get; set; }           // foreign key
        public Venda Venda { get; set; } = null!; // navegação para o pai
    }
}
