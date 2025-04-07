using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MejorAppTG1.Models
{
    public class Factor
    {
        public string NumeroFactor { get; set; }
        public int Puntuacion { get; set; }
        public string Nivel { get; set; }
        public Factor(string NumeroFactor, int Puntuacion, string Nivel)
        {
            this.NumeroFactor = NumeroFactor;
            this.Puntuacion = Puntuacion;
            this.Nivel = Nivel;
        }
    }
}
