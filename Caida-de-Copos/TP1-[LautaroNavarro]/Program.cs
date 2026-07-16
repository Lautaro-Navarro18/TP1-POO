using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CaidadeCopos
{
    /*
    Realizar un programa que represente una simulación de copos de nieve cayendo en la consola, utilizando el símbolo "*" para cada copo.
    El programa debe cumplir con las siguientes condiciones:
     -Definir una clase Configuracion que almacene parámetros de la simulación, como la cantidad de filas, columnas y la velocidad de caída de los copos.
     -Definir una clase Copo que modele el comportamiento de un copo de nieve. Cada copo debe tener una posición en la consola y un método para mostrarse y desplazarse hacia abajo.
     -Usar una lista para administrar todos los copos activos durante la simulación.
     -Implementar una lógica que controle la caída de los copos de nieve, evitando que se superpongan en la misma posición.
     -Al completarse una fila con copos en todas las columnas, esta debe eliminarse para permitir que continúe la simulación.
     -El programa debe ejecutarse en un ciclo continuo, simulando de manera animada la caída de los copos.
    */
    class Configuracion
    {
        public int Filas { get; set; }
        public int Columnas { get; set; }
        public int VelocidadCaida { get; set; }
        public int ProbaNuevoCopo { get; set; }
        public char SimboloCopo { get; set; }
        public Configuracion(int filas = 15, int columnas = 30, int velocidadCaida = 150, int probaNuevoCopo = 25, char simboloCopo = '*')
        {
            Filas = filas;
            Columnas = columnas;
            VelocidadCaida = velocidadCaida;
            ProbaNuevoCopo = probaNuevoCopo;
            SimboloCopo = simboloCopo;
        }
    }
    class Copo
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
        private char simbolo;
        public Copo(int fila, int columna, char simbolo)
        {
            Fila = fila;
            Columna = columna;
            this.simbolo = simbolo;
        }
        public void Mostrar()
        {
            Console.SetCursorPosition(Columna, Fila);
            Console.Write(simbolo);
        }
        public void Borrar()
        {
            Console.SetCursorPosition(Columna, Fila);
            Console.Write(' ');
        }
        public void Descender()
        {
            Fila++;
        }
    }
    class Program
    {
        static Configuracion config = new Configuracion();
        static List<Copo> copos = new List<Copo>();
        static bool[,] ocupado;
        static Random ale = new Random();
        static int contadorBorrados = 0;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(Math.Max(config.Columnas + 2, Console.WindowWidth),
            Math.Max(config.Filas + 3, Console.WindowHeight));
            Console.Clear();
            ocupado = new bool[config.Filas, config.Columnas];
            while (true)
            {
                GenerarCopo();
                MoverCopos();
                EliminarFilasCompletas();
                MostrarEncabezado();
                Thread.Sleep(config.VelocidadCaida);
            }
        }
        static void GenerarCopo()
        {
            if (ale.Next(0, 100) >= config.ProbaNuevoCopo)
            {
                return;
            }
            List<int> columnasLibres = new List<int>();
            for (int i = 0; i < config.Columnas; i++)
            {
                if (!ocupado[0, i])
                {
                    columnasLibres.Add(i);
                }
            }
            if (columnasLibres.Count == 0)
            {
                return;
            }
            int columna = columnasLibres[ale.Next(columnasLibres.Count)];
            Copo nuevo = new Copo(0, columna, config.SimboloCopo);
            copos.Add(nuevo);
            ocupado[0, columna] = true;
            nuevo.Mostrar();
        }
        static void MoverCopos()
        {
            var ordenados = copos.OrderByDescending(c => c.Fila).ToList();
            foreach (var copo in ordenados)
            {
                int filaDestino = copo.Fila + 1;
                bool puedeBajar = filaDestino < config.Filas && !ocupado[filaDestino, copo.Columna];
                if (puedeBajar)
                {
                    copo.Borrar();
                    ocupado[copo.Fila, copo.Columna] = false;
                    copo.Descender();
                    ocupado[copo.Fila, copo.Columna] = true;
                    copo.Mostrar();
                }
            }
        }
        static void EliminarFilasCompletas()
        {
            for (int i = config.Filas - 1; i >= 0; i--)
            {
                bool filaCompleta = true;
                for (int e = 0; e < config.Columnas; e++)
                {
                    if (!ocupado[i, e])
                    {
                        filaCompleta = false;
                        break;
                    }
                }
                if (filaCompleta)
                {
                    copos.RemoveAll(cp => cp.Fila == i);

                    foreach (var copo in copos.Where(cp => cp.Fila < i))
                    {
                        copo.Descender();
                    }
                    ocupado = new bool[config.Filas, config.Columnas];
                    foreach (var copo in copos)
                    {
                        ocupado[copo.Fila, copo.Columna] = true;
                    }
                    RedibujarTodo();
                    contadorBorrados += config.Columnas;
                }
            }
        }
        static void RedibujarTodo()
        {
            Console.Clear();
            foreach (var copo in copos)
            {
                copo.Mostrar();
            }
        }
        static void MostrarEncabezado()
        {
            Console.SetCursorPosition(0, config.Filas + 1);
            Console.WriteLine($"Copos activos: {copos.Count}");
            Console.Write($"Copos borrados: {contadorBorrados}");
        }
    }
}
