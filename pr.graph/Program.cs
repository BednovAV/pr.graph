using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace pr.graph
{
    class Program
    {
        // начальное меню
        public static void MenuCreate()
        {
            int select = 1;

            while (select != 0)
            {
                Console.WriteLine("Возможные действия:");
                Console.WriteLine("\t1. Создать пустой граф");
                Console.WriteLine("\t2. Загрузить граф из файла");
                Console.WriteLine("\t0. Выйти");

                Console.Write("Ваш выбор: ");
                select = int.Parse(Console.ReadLine());


                switch (select)
                {
                    case 1:
                        Console.Write("\t\tОриентированный: ");
                        bool directed = (Console.ReadLine() == "y") ? true : false;

                        Console.Write("\t\tВзвешенный: ");
                        bool weighted = (Console.ReadLine() == "y") ? true : false;

                        MenuGraph(new Graph(directed, weighted));
                        break;
                    case 2:
                        Console.Write("\t\tНазвание файла:");
                        MenuGraph(new Graph(Console.ReadLine()));
                        break;
                    default:
                        break;
                }
            }
        }

        public static void MenuGraph(Graph g)
        {
            int select = 1;

            while (select != 0)
            {
                Console.WriteLine("Возможные действия:");
                Console.WriteLine("\t1. Добавить вершину");
                Console.WriteLine("\t2. Добавить связь");
                Console.WriteLine("\t3. Удалить вершину");
                Console.WriteLine("\t4. Удалить связь");
                Console.WriteLine("\t5. Показать список смежности");
                Console.WriteLine("\t6. Сохранить граф");
                Console.WriteLine("\t7. Выйти");
                

                Console.Write("Ваш выбор: ");
                select = int.Parse(Console.ReadLine());


                switch (select)
                {
                    case 1:
                        Console.Write("Название вершины: ");
                        g.AddVertex(Console.ReadLine());
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        ShowLinks(g);
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    default:
                        break;
                }
            }
        }

        public static void ShowLinks(Graph g)
        {
            foreach (var item in g.GetLinks())
            {
                Console.WriteLine(item);
            }
        }

        static void Main(string[] args)
        {
            MenuCreate();


        }
    }
}
