using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoED.Models
{
    public class Informacion : IComparable
    {
        public List<string> Registros { get; set; }

        public int IDregistro { get; set; }
        public Informacion()
        {
            Registros = new List<string>();
            IDregistro = 0;
        }

        public int CompareTo(object obj)
        {
            var Comparado = (Informacion)obj;
            return IDregistro.CompareTo(Comparado.IDregistro);
        }
    }
}