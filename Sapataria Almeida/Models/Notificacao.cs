using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapataria_Almeida.Models
{
    public class Notificacao
    {
        public int Id { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Lida { get; set; } = false;
    }

}
