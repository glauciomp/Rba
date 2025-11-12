using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rba.Models
{
    public class PontoColeta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string TipoLixo { get; set; }
        public string Contato { get; set; }
        public string Horario { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
