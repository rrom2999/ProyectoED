using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoED.Controllers;
using ProyectoED.Models;

namespace ProyectoED.Controllers
{
    public class MiniSQLController : Controller
    {
        // GET: MiniSQL
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Inicio()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Exito()
        {
            return View();
        }
        public ActionResult DELETE()
        {
            return View();
        }


        //public static  AtributosDeTabla;

        public static ArbolB<Informacion> ArbolNuevo = new ArbolB<Informacion>();
        public static List<string[]> Ingresados = new List<string[]>();
        [HttpPost]
        public  ActionResult Dividir(string Codigo)
        {
            
            string CodigoN = Codigo;
            string[] CodigoSeparado = new string[100];
            CodigoSeparado = CodigoN.Split(' ');

            if (CodigoSeparado[0] == "CREATE" && DiccionarioArboles.ContainsKey(CodigoSeparado[2]) == false)
            {
                if(CodigoSeparado[1] == "TABLE")
                {
                    ArbolNuevo.NombreVariables.Clear();
                    Ingresados.Clear();
                    int ContadorAtributos = (CodigoSeparado.Length - 7) / 2;  //-7 por CREATE TABLE NOMBRE ID INT PRIMARY KEY y DIV 2 porque por cada nombre existe un tipo de variable
                                                                              //AtributosDeTabla = new string[CodigoSeparado.Length - 7];  //Arreglo para guardar todos los atributos y sus tipos
                    if (ContadorAtributos > 10)
                    {
                        return RedirectToAction("/error");
                    }
                    else
                    {
                        ArbolNuevo.NombreVariables.Add("ID");
                        for (int i = 7; i < CodigoSeparado.Length; i++)       //Desde 7 para que comienze luego de KEY
                        {
                            if (i % 2 != 0)
                            {
                                ArbolNuevo.NombreVariables.Add(CodigoSeparado[i]);       //i-7 para comenzar en 0 en AtributosDeTabla
                            }

                        }
                        //en posiciones pares se almacenara el nombre y en posiciones impares el tipo de dato
                        DiccionarioArboles.Add(CodigoSeparado[2], ArbolNuevo);
                        
                        ViewBag.Columnas = ArbolNuevo.NombreVariables;

                        return View();
                    }
                }
                else
                {

                    return RedirectToAction("/error");
                }
            }
            else if (CodigoSeparado[0] == "INSERT" && CodigoSeparado[1] == "INTO") //Revisa sintaxis
            {
                if (DiccionarioArboles.ContainsKey(CodigoSeparado[2]) == true) //Revisa que si exista la tabla
                {
                    int Declarados = 0;           //Contador de atributos declarados antes de "VALUES"
                    int PosicionValues = 0;           //Posicion de vector CodigoSeparado[] donde esta "VALUES" 
                    int Asignados = 0;            //Contador de variables asignadas despues de "VALUES"
                    for (int i = 0; i < CodigoSeparado.Length; i++) //Para contar atributos 
                    {
                        if (CodigoSeparado[i] == "VALUES")
                        {
                            PosicionValues = i;                     //Toma valor de posicion donde VALUES esta
                            break;
                        }
                    }
                    Declarados = PosicionValues - 3;                  //Resta 3 de la posicion VALUES por "INSERT", "INTO" y "VALUES"
                    for (int j = (PosicionValues + 1); j < CodigoSeparado.Length; j++) //Comienza despues de VALUES 
                    {
                        Asignados++;                        //Incrementa por elementos 
                    }
                    if (Declarados != Asignados)
                    {
                        return RedirectToAction("/error");
                    }
                    if (Declarados > 10)
                    {
                        return RedirectToAction("/error");
                    }
                    if (Asignados == Declarados)
                    {
                        string[] Columnas = new string[Declarados];      //Arreglo para imprimir columnas
                        string[] As = new string[Declarados];            //Arreglo para imprimir declarados
                        for (int p = 3; p < PosicionValues; p++)            //Desde posicion 3 para quitar INSERT INTO Nombre, hasta VALUES
                        {
                            Columnas[p - 3] = CodigoSeparado[p];         //Recorre CodigoSeparado para ingresar en Columnas 
                        }
                        for (int p = PosicionValues + 1; p < CodigoSeparado.Length; p++)      //Desde uno luego de VALUES 
                        {
                            As[p - PosicionValues - 1] = CodigoSeparado[p]; //Recorre CodigoSeparado para guardar los asignados
                        }
                        Informacion Datos = new Informacion();  //Se instancia un dato de tipo informacion para poder guardarlo en arbol
                        Datos.IDregistro = int.Parse(As[0]);  //Agrega ID

                        /*foreach (var item in As)     //Agrega cada valor en la lista de registros 
                        {
                            Datos.Registros.Add(item);
                        }
                        DiccionarioArboles[CodigoSeparado[2]].InsertarNodo(DiccionarioArboles[CodigoSeparado[2]].Raiz, Datos);
                        Ingresados.Add(As);

                        Ingresados = Ingresados.OrderBy(o => o[0]).ToList();
                        ViewBag.Columnas = ArbolNuevo.NombreVariables;
                        ViewBag.Insertado = Ingresados;*/
                        return View();
                    }
                }else
                {
                    return RedirectToAction("/error");
                }
                
            }
            if (CodigoSeparado[0] == "SELECT")
            {
                int Marcador = 0;       //Posicion de FROM en arreglo CodigoOrdenado
                for (int i = 0; i < CodigoSeparado.Length; i++)
                {
                    if (CodigoSeparado[i] == "FROM")  //Desde 1 para quitar el SELECT
                    {
                        Marcador = i;       //Obtiene la posicion de FROM
                    }
                }
                string[] ColumnasSelecionadas = new string[Marcador - 1]; //Cantidad de columnas selecionadas sera la posicion del marcador menos uno
                for (int i = 1; i < ColumnasSelecionadas.Length; i++)
                {
                    ColumnasSelecionadas[i - 1] = CodigoSeparado[i]; //Almacena solo las "categorias" que el usuario escribio para obtener
                }

                return RedirectToAction("/exito");
            }
            else if (CodigoSeparado[0] == "DELETE" && CodigoSeparado[1] == "FROM")
            {
                bool TieneWhere = false;
                Informacion ID = new Informacion();
                if(DiccionarioArboles.ContainsKey(CodigoSeparado[3]) == true)
                {
                    for (int i = 0; i < CodigoSeparado.Length; i++) //Busca si el codigo tiene "where"
                    {
                        if(CodigoSeparado[i] == "WHERE")
                        {
                            TieneWhere = true;
                        }
                    }

                    if(TieneWhere == false)
                    {
                        DiccionarioArboles[CodigoSeparado[2]].EliminarArbol();
                    }
                    else
                    {
                        ID.IDregistro = Convert.ToInt32(CodigoSeparado[6]);
                        DiccionarioArboles[CodigoSeparado[3]].Eliminar(ID, DiccionarioArboles[CodigoSeparado[3]].Raiz);
                    }
                }
                foreach(var Vector in Ingresados) //Para eliminar de la vista (del viewbag)
                {
                    if(Vector[0] == CodigoSeparado[6])
                    {
                        Ingresados.Remove(Vector);
                    }
                }
                Ingresados = Ingresados.OrderBy(o => o[0]).ToList();
                return View();
            }
            else if (CodigoSeparado[0] == "DROP")
            {
                if (CodigoSeparado[1] == "TABLE")
                {
                    string NombreTabla = CodigoSeparado[2];
                    if(DiccionarioArboles.ContainsKey(NombreTabla) == true)
                    {
                        DiccionarioArboles.Remove(NombreTabla);
                        Ingresados = null;
                        ArbolNuevo = null;
                        return View("Inicio");
                    }
                    else
                    {

                        return RedirectToAction("/error");
                    }
                }
                else
                {

                    return RedirectToAction("/error");
                }
            }
            else
            {
                return RedirectToAction("/error");
            }

        }
        //Diccionario que contiene cada arbol/ tabla
        public static Dictionary<string, ArbolB<Informacion>> DiccionarioArboles = new Dictionary<string, ArbolB<Informacion>>();


    }
}