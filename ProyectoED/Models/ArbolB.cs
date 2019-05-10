using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoED.Models
{
    public class ArbolB<T> : IEnumerable<T> where T : IComparable
    {
        public NodoArbolB<T> Raiz { get; set; }
        public int Siguiente { get; set; }
        public List<string> NombreVariables { get; set; } // Lista de atributos
        public ArbolB()
        {
            Raiz = new NodoArbolB<T>();
            Siguiente = 2;
            NombreVariables = new List<string>();
        }

        public void EliminarArbol()
        {
            Raiz = null;
        }
        public bool TieneHijos(NodoArbolB<T> Nodo)
        {

            if (Nodo.Hijos.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool TieneEspacio(NodoArbolB<T> Nodo)
        {
            if (Nodo.ListaRegistros.Count <= Nodo.Maximo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public void InsertarNodo(NodoArbolB<T> Nodo, T valor)
        {
            if (Raiz.ID == 0)
            {
                Raiz = new NodoArbolB<T>();
                Raiz.ObtenerGrado(Raiz, 3);
                Raiz.ListaRegistros.Add(valor);
                Raiz.ID = 1;
                Nodo = Raiz;
            }
            //ES HOJA
            else if (TieneHijos(Nodo) == false)  //Cambiar a Nodo.Hijos.Count != 0 Borrar el metodo para ver si tiene hijos
            {
                ValorAgregadoOrdenado(valor, Nodo);
            }
            //NO ES HOJA
            else if (TieneHijos(Nodo) == true)
            {
                var NodoHijo = new NodoArbolB<T>();
                NodoHijo = Nodo.Hijos[IndiceHijo(Nodo, valor)]; //BUSCA POSICION CORRESPONDIENTE
                InsertarNodo(NodoHijo, valor);
            }

            if (TieneEspacio(Nodo) == false)
            {
                DividirEnNodos(Nodo);
            }
        }


        public void CrearNodo(NodoArbolB<T> nuevo, NodoArbolB<T> nodo)
        {
            nuevo.Maximo = nodo.Maximo;
            nuevo.Minimo = nodo.Minimo;
            nuevo.ID = Siguiente;
            Siguiente++;
        }

        public void DividirEnNodos(NodoArbolB<T> Nodo)
        {
            NodoArbolB<T> Izquierdo = new NodoArbolB<T>();
            NodoArbolB<T> AuxiliarP = new NodoArbolB<T>();
            NodoArbolB<T> Derecho = new NodoArbolB<T>();
            CrearNodo(Izquierdo, Nodo);
            CrearNodo(Derecho, Izquierdo);

            for (int i = 0; i < Nodo.Minimo; i++)
            {
                Izquierdo.ListaRegistros.Add(Nodo.ListaRegistros[i]);
            }

            for (int i = Nodo.Minimo + 1; i <= Nodo.Maximo; i++)
            {
                Derecho.ListaRegistros.Add(Nodo.ListaRegistros[i]);
            }


            if (Nodo.Padre != null) //Si es cualquier hijo
            {
                EnlacePadreHijo(Nodo.Padre, Izquierdo);
                EnlacePadreHijo(Nodo.Padre, Derecho);

                Nodo.Padre.ListaRegistros.Add(Nodo.ListaRegistros[Nodo.Minimo]);
                Nodo.Padre.ListaRegistros.Sort((x, y) => x.CompareTo(y));

                int indice = 0;

                for (int i = 0; i < Nodo.Padre.Hijos.Count; i++)
                {
                    if (Nodo.Padre.Hijos[i].ListaRegistros.Count > 4)
                    {
                        indice = i;
                        break;
                    }
                }

                if (Nodo.Hijos.Count > 0)
                {
                    HijosHijo(Nodo, Izquierdo, 0, Nodo.Minimo);
                    HijosHijo(Nodo, Derecho, Nodo.Minimo + 1, Nodo.Maximo + 1);
                }

                Nodo.Padre.Hijos.RemoveAt(indice);
                Nodo.Padre.Hijos.Sort((x, y) => x.CompareTo(y));
                Nodo = null;
            }//Si es la raiz y aun caben valores en el nodo
            else if (Nodo.Padre == null && Nodo.Hijos.Count < 5)
            {
                AuxiliarP.ListaRegistros.Add(Nodo.ListaRegistros[Nodo.Minimo]);
                EnlacePadreHijo(Nodo, Izquierdo);
                EnlacePadreHijo(Nodo, Derecho);
                Nodo.ListaRegistros.Sort((x, y) => x.CompareTo(y));
                Nodo.ListaRegistros = AuxiliarP.ListaRegistros;
            }//Si es raiz y no caben valores
            else if (Nodo.Padre == null && Nodo.Hijos.Count >= 5)
            {
                T val = Nodo.ListaRegistros[Nodo.Minimo];

                HijosHijo(Nodo, Izquierdo, 0, Nodo.Minimo);
                HijosHijo(Nodo, Derecho, Nodo.Minimo + 1, Nodo.Maximo + 1);

                Nodo.Hijos.Clear();
                EnlacePadreHijo(Nodo, Izquierdo);
                EnlacePadreHijo(Nodo, Derecho);

                Nodo.ListaRegistros.Clear();
                Nodo.ListaRegistros.Add(val);
            }

        }

        public void HijosHijo(NodoArbolB<T> Nodo, NodoArbolB<T> NodoH, int Inicio, int Final)
        {
            for (int i = Inicio; i <= Final; i++)
            {
                NodoH.Hijos.Add(Nodo.Hijos[i]);
            }
            foreach (var item in NodoH.Hijos)
            {
                item.Padre = NodoH;
            }
        }

        public void EnlacePadreHijo(NodoArbolB<T> Papa, NodoArbolB<T> Hijo)
        {
            Hijo.Padre = Papa;
            Papa.Hijos.Add(Hijo);
        }

        public int IndiceHijo(NodoArbolB<T> Nodo, T valor)
        {
            if (Nodo.ListaRegistros.Count == 1) //Solo tiene un valor
            {
                if (valor.CompareTo(Nodo.ListaRegistros[0]) < 0)  //si es menor que el existente
                {
                    return 0; //Indice es 0 
                }
                else //Si es mayor que el existente 
                {
                    return 1; //Indice es 1
                }
            }
            else //Si tiene mas de un valor
            {
                for (int i = 0; i < Nodo.ListaRegistros.Count - 1; i++)  //Recorre las posicions de la lista
                {
                    if (valor.CompareTo(Nodo.ListaRegistros[i]) < 0)  //Si llega a ser menor en algun momento, retorna este indice
                    {
                        return i;
                    }
                    else if (valor.CompareTo(Nodo.ListaRegistros[i]) > 0 && valor.CompareTo(Nodo.ListaRegistros[i + 1]) < 0)
                    {
                        return i + 1;   //Si esta entre dos valores, retorna el indice del mayor para "correr" el arreglo 
                    }
                }
                return Nodo.ListaRegistros.Count;
            }
        }

        public void ValorAgregadoOrdenado(T Valor, NodoArbolB<T> NodoAOrdenar)  //Agrega valor en el nodo y ordena la lista de valores
        {
            NodoAOrdenar.ListaRegistros.Add(Valor);
            NodoAOrdenar.ListaRegistros.Sort((a, b) => a.CompareTo(b));
        }


        static T Encontrar;
        public T BuscarNodo(T Valbuscar, NodoArbolB<T> Nodo)
        {
            bool Encontrado = false;
            foreach (var item in Nodo.ListaRegistros) //Recorre en los valores del nodo 
            {
                if (item.CompareTo(Valbuscar) == 0)
                {
                    Encontrar = item; Encontrado = true;
                    break;
                }
            }
            if (Encontrado == false && Nodo.Hijos.Count > 0)
            {
                NodoArbolB<T> NodoHijo = new NodoArbolB<T>();
                NodoHijo = Nodo.Hijos[IndiceHijo(Nodo, Valbuscar)];
                return BuscarNodo(Valbuscar, NodoHijo);     //Busca en hijo
            }
            else if (Encontrado == true)
            {
                return Encontrar;
            }
            else if (Nodo.Hijos.Count == 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                return Encontrar;
            }

        }

        public bool CantidadMenor(NodoArbolB<T> Nodo) //Revisa si tiene la cantidad minima de valores del nodo 
        {
            if (Nodo.ListaRegistros.Count < Nodo.Minimo)
                return true;
            else
                return false;
        }

        static int Cont = 0;
        static NodoArbolB<T> Cambiar = new NodoArbolB<T>();
        static NodoArbolB<T> Dar = new NodoArbolB<T>();

        public void Eliminar(T valor, NodoArbolB<T> Nodo)
        {
            int Indice = 0;
            bool Inicial = false;
            bool Encontrado = false;
            for (int i = 0; i < Nodo.ListaRegistros.Count; i++)
            {
                if (Nodo.ListaRegistros[i].CompareTo(valor) == 0)
                {
                    Encontrado = true;
                    if (TieneHijos(Nodo) == true)
                    {
                        Indice = i;  //Posicion de valor a sustituir
                        Cambiar = Nodo; //Nodo a sustituir
                        Cont++; //revisar para que se usa
                        Inicial = true;
                        Dar = ValorMasIzquierdodeDerecha(Indice, Nodo, false);

                        Cambiar.ListaRegistros.Add(Dar.ListaRegistros[0]);
                        Cambiar.ListaRegistros.RemoveAt(Indice);
                        Dar.ListaRegistros.RemoveAt(0);

                        Cambiar.ListaRegistros.Sort((x, y) => x.CompareTo(y));

                        if (CantidadMenor(Dar) == true)
                        {
                            PrestarDeHermano(Dar.Padre, Dar); //Ver si hermano puede prestar
                        }
                    }
                    else
                    {
                        Nodo.ListaRegistros.RemoveAt(i);
                    }
                    if (Inicial == true)
                    {
                        Nodo = Cambiar;
                    }
                    if (CantidadMenor(Nodo) == true)
                    {
                        PrestarDeHermano(Nodo.Padre, Nodo); //Ver si hermano puede prestar
                    }
                }
            }
            if (Encontrado == false && Nodo.Hijos.Count > 0)
            {
                NodoArbolB<T> NodoHijo = new NodoArbolB<T>();
                NodoHijo = Nodo.Hijos[IndiceHijo(Nodo, valor)];
                Eliminar(valor, NodoHijo);
            }
        }


        public void PrestarDeHermano(NodoArbolB<T> Padre, NodoArbolB<T> Nodo)
        {

            NodoArbolB<T> Hermano = new NodoArbolB<T>(); int PosicionPadre = 0;
            for (int i = 0; i < Padre.Hijos.Count; i++)
            {   //Hermano derecho puede prestar
                if (Padre.Hijos[i] == Nodo && i == 0 && Padre.Hijos[i + 1].ListaRegistros.Count > Padre.Minimo)
                {
                    Hermano = Padre.Hijos[i + 1];
                    PosicionPadre = i;
                    break;
                }
                //Hermano izquierdo puede prestar
                else if (Padre.Hijos[i] == Nodo && i == (Padre.Hijos.Count - 1) && Padre.Hijos[i - 1].ListaRegistros.Count > Padre.Minimo)
                {
                    Hermano = Padre.Hijos[i - 1];
                    PosicionPadre = i - 1;
                    break;
                }
                //Nodo intermedio y presta del hermano izquierdo
                else if (Padre.Hijos[i] == Nodo && i > 0 && i < Padre.Hijos.Count - 1 && Padre.Hijos[i - 1].ListaRegistros.Count > Padre.Minimo)
                {
                    Hermano = Padre.Hijos[i - 1];
                    PosicionPadre = i - 1;
                    break;
                }
                //Nodo intermedio y presta del hermano derecho
                else if (Padre.Hijos[i] == Nodo && i > 0 && i < Padre.Hijos.Count - 1 && Padre.Hijos[i + 1].ListaRegistros.Count > Padre.Minimo)
                {
                    Hermano = Padre.Hijos[i + 1];
                    PosicionPadre = i;
                    break;
                }

            }
            if (Hermano.Padre == null)//Si ningun hermano puede prestar valor
            {
                for (int i = 0; i < Padre.Hijos.Count; i++)
                {
                    //SI NO SE PUEDEN PRESTAR VALORES SOLO PASA LA POSICION DEL PADRE
                    //Si el primer nodo tiene menor cantidad a Minimo
                    if (Padre.Hijos[i] == Nodo && i == 0)
                    {
                        PosicionPadre = i;
                        Hermano = Padre.Hijos[PosicionPadre + 1];
                        break;
                    }
                    //Si el ultimo nodo tiene menor cantidad a Minimo
                    else if (Padre.Hijos[i] == Nodo && i == (Padre.Hijos.Count - 1))
                    {
                        PosicionPadre = i - 1;
                        Hermano = Padre.Hijos[PosicionPadre];
                        break;
                    }
                    //Si un nodo medio tiene menor cantidad a Minimo
                    else if (Padre.Hijos[i] == Nodo && i > 0 && i < Padre.Hijos.Count - 1)
                    {
                        PosicionPadre = i;
                        if (Padre.Hijos[PosicionPadre + 1].ListaRegistros.Count > Padre.Hijos[PosicionPadre - 1].ListaRegistros.Count)
                        {
                            Hermano = Padre.Hijos[PosicionPadre + 1];
                        }
                        else if (Padre.Hijos[PosicionPadre + 1].ListaRegistros.Count < Padre.Hijos[PosicionPadre - 1].ListaRegistros.Count)
                        {
                            Hermano = Padre.Hijos[PosicionPadre - 1];
                        }
                        else
                        {
                            Hermano = Padre.Hijos[PosicionPadre + 1];
                        }
                        break;
                    }
                }

                Unir(Padre, PosicionPadre, Nodo, Hermano);
            }
            //Encontro valor solo se mueve
            else
            {
                MoverValor(Hermano, PosicionPadre, Padre, Nodo);
            }
        }

        public void MoverValor(NodoArbolB<T> hijoEmisor, int posicionvalorpadre, NodoArbolB<T> padre, NodoArbolB<T> nodoReceptor)
        {

            //NODO HERMANO IZQUIERDO: Si debo de pasar el ultimo dato
            if (hijoEmisor.ListaRegistros[hijoEmisor.ListaRegistros.Count - 1].CompareTo(padre.ListaRegistros[posicionvalorpadre]) < 0) //Hijo a la izquierda
            {
                padre.ListaRegistros.Add(hijoEmisor.ListaRegistros[hijoEmisor.ListaRegistros.Count - 1]); //Subir valor
                hijoEmisor.ListaRegistros.Remove(hijoEmisor.ListaRegistros[hijoEmisor.ListaRegistros.Count - 1]); //quita valor que se paso a padre
                nodoReceptor.ListaRegistros.Add(padre.ListaRegistros[posicionvalorpadre]); //bajar valor de padre
                padre.ListaRegistros.Remove(padre.ListaRegistros[posicionvalorpadre]);
                nodoReceptor.ListaRegistros.Sort((x, y) => x.CompareTo(y)); // ordenar hijo que recibe

                //hijoEmisor.Valores.RemoveAt(hijoEmisor.Valores.Count - 1);

                padre.ListaRegistros.Sort((x, y) => x.CompareTo(y));
            }

            //HIJO A LA DERECHA: Si debo de pasar el primer dato
            else if (hijoEmisor.ListaRegistros[0].CompareTo(padre.ListaRegistros[posicionvalorpadre]) > 0)
            {
                padre.ListaRegistros.Add(hijoEmisor.ListaRegistros[0]);
                nodoReceptor.ListaRegistros.Add(padre.ListaRegistros[posicionvalorpadre]);
                nodoReceptor.ListaRegistros.Sort((x, y) => x.CompareTo(y));

                hijoEmisor.ListaRegistros.Remove(hijoEmisor.ListaRegistros[0]);

                padre.ListaRegistros.Remove(padre.ListaRegistros[posicionvalorpadre]);

                padre.ListaRegistros.Sort((x, y) => x.CompareTo(y));
            }

        }

        public void Unir(NodoArbolB<T> padre, int posicionvalorpadre, NodoArbolB<T> hijo, NodoArbolB<T> hermano)
        {

            if (hijo.ListaRegistros[0].CompareTo(padre.ListaRegistros[padre.ListaRegistros.Count - 1]) > 0) //HIJO A LA DERECHA: Si es el ultimo nodo
            {
                hermano = padre.Hijos[posicionvalorpadre];
                for (int i = 0; i < hermano.ListaRegistros.Count; i++)
                {
                    hijo.ListaRegistros.Add(hermano.ListaRegistros[i]);
                }

                if (TieneHijos(hijo) == true)
                {
                    for (int i = 0; i < hermano.Hijos.Count; i++)
                    {
                        hijo.Hijos.Add(hermano.Hijos[i]);
                    }
                }

                padre.Hijos.RemoveAt(posicionvalorpadre); //Elimina el nodo hermano
                hijo.ListaRegistros.Add(padre.ListaRegistros[posicionvalorpadre]); //Manda valor de raiz
                hijo.ListaRegistros.Sort((x, y) => x.CompareTo(y));

                padre.ListaRegistros.Remove(padre.ListaRegistros[posicionvalorpadre]);

                if (padre.ListaRegistros.Count < padre.Minimo)
                {
                    PrestarDeHermano(padre.Padre, padre);
                }
            }

            else if (hijo.ListaRegistros[hijo.ListaRegistros.Count - 1].CompareTo(padre.ListaRegistros[0]) < 0) //HIJO A LA IZQUIERDA DEL PADRE
            {
                hermano = padre.Hijos[posicionvalorpadre + 1];
                for (int i = 0; i < hermano.ListaRegistros.Count; i++)
                {
                    hijo.ListaRegistros.Add(hermano.ListaRegistros[i]);
                }

                if (TieneHijos(hijo) == true)
                {
                    for (int i = 0; i < hermano.Hijos.Count; i++)
                    {
                        hijo.Hijos.Add(hermano.Hijos[i]);
                    }
                }

                padre.Hijos.RemoveAt(posicionvalorpadre + 1); //Elimina el nodo hermano
                hijo.ListaRegistros.Add(padre.ListaRegistros[0]); //Manda valor de raiz
                hijo.ListaRegistros.Sort((x, y) => x.CompareTo(y));

                padre.ListaRegistros.Remove(padre.ListaRegistros[0]);

                if (padre.ListaRegistros.Count < padre.Minimo && padre.Padre != null)
                {
                    PrestarDeHermano(padre.Padre, padre);
                }
            }

            else if (hijo.ListaRegistros[0].CompareTo(padre.ListaRegistros[0]) > 0 && hijo.ListaRegistros[hijo.ListaRegistros.Count - 1].CompareTo(padre.ListaRegistros[padre.ListaRegistros.Count - 1]) < 0) //Si es uno de enmedio
            {
                if (hijo.ListaRegistros[0].CompareTo(padre.ListaRegistros[posicionvalorpadre]) > 0)
                {
                    hermano = padre.Hijos[posicionvalorpadre];
                    for (int i = 0; i < hermano.ListaRegistros.Count; i++)
                    {
                        hijo.ListaRegistros.Add(hermano.ListaRegistros[i]);
                    }

                    if (TieneHijos(hijo) == true)
                    {
                        for (int i = 0; i < hermano.Hijos.Count; i++)
                        {
                            hijo.Hijos.Add(hermano.Hijos[i]);
                        }
                    }

                    padre.Hijos.RemoveAt(posicionvalorpadre); //Elimina el nodo hermano
                    hijo.ListaRegistros.Add(padre.ListaRegistros[padre.ListaRegistros.Count - 1]); //Manda valor de raiz
                    hijo.ListaRegistros.Sort((x, y) => x.CompareTo(y));
                }
                else
                {
                    hermano = padre.Hijos[posicionvalorpadre + 1];
                    for (int i = 0; i < hermano.ListaRegistros.Count; i++)
                    {
                        hijo.ListaRegistros.Add(hermano.ListaRegistros[i]);
                    }

                    if (TieneHijos(hijo) == true)
                    {
                        for (int i = 0; i < hermano.Hijos.Count; i++)
                        {
                            hijo.Hijos.Add(hermano.Hijos[i]);
                        }
                    }

                    padre.Hijos.RemoveAt(posicionvalorpadre + 1); //Elimina el nodo hermano
                    hijo.ListaRegistros.Add(padre.ListaRegistros[posicionvalorpadre]); //Manda valor de raiz
                    padre.ListaRegistros.RemoveAt(posicionvalorpadre);
                    hijo.ListaRegistros.Sort((x, y) => x.CompareTo(y));
                }





            }

        }

        public NodoArbolB<T> ValorMasIzquierdodeDerecha(int IndiceValor, NodoArbolB<T> Nodo, bool IrDerecha)
        {
            if (IrDerecha == false && Nodo.Hijos.Count > 0)
            {
                IrDerecha = true;
                return ValorMasIzquierdodeDerecha(IndiceValor, Nodo.Hijos[IndiceValor + 1], IrDerecha);
            }
            else if (IrDerecha == true && Nodo.Hijos.Count > 0)
            {
                return ValorMasIzquierdodeDerecha(IndiceValor, Nodo.Hijos[0], IrDerecha);
            }
            else if (IrDerecha == true && Nodo.Hijos.Count == 0)
            {
                return Nodo;
            }
            else
            {
                return Nodo;
            }

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