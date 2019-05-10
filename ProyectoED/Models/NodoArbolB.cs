using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ProyectoED.Models
{
    public class NodoArbolB<T> : IEnumerable<T> where T : IComparable
    {
        public NodoArbolB<T> Padre { get; set; }
        public List<NodoArbolB<T>> Hijos { get; set; }
        public List<T> ListaRegistros { get; set; } //Lista de registros de tipo informacion

        public int ID { get; set; }
        public int Maximo { get; set; }
        public int Minimo { get; set; }

        public NodoArbolB()
        {
            Padre = null;  Hijos = new List<NodoArbolB<T>>();  ListaRegistros = new List<T>();
            ID = 0; Maximo = 0; Minimo = 0;
        }
        public int CompareTo(object obj)
        {
            var Comparado = (NodoArbolB<T>)obj;
            return ID.CompareTo(Comparado.ID);
        }

        public void ObtenerGrado(NodoArbolB<T> Tmp, int Grado)
        {
            Tmp.Minimo = (Grado-1)/ 2;   Tmp.Maximo = Grado-1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}