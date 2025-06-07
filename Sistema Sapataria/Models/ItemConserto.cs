using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Sapataria.Models
{
    public class ItemConserto
    {
        public int Id { get; set; }
        public string TipoConserto { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string? Descricao { get; set; }

        public string Estado { get; set; } = "Pendente";
        public DateTime DataEntrega { get; set; }

        public int ConsertoId { get; set; }
        public Conserto Conserto { get; set; } = null!;
    }
}
