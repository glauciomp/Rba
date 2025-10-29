using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rba.Models
{
    public class TipoLixo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Cor { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string OrigemDescricao { get; set; } = string.Empty;
        public string DestinoAmbiental { get; set; } = string.Empty;
        public string Exemplos { get; set; } = string.Empty;

        // mudança atual
        // Caminho da imagem
        public string Imagem { get; set; }
    }
   
}
