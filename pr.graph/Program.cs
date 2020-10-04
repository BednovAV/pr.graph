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
                Console.Clear();
                Console.WriteLine("Возможные действия:");
                Console.WriteLine("\t1. Создать пустой граф");
                Console.WriteLine("\t2. Загрузить граф из файла");
                Console.WriteLine("\t0. Выйти");

                Console.Write("Ваш выбор: ");
                select = int.Parse(Console.ReadLine());


                switch (select)
                {
                    case 1:
                        Console.Write("\tОриентированный(t - true): ");
                        bool directed = (Console.ReadLine() == "t") ? true : false;

                        Console.Write("\tВзвешенный(t - true): ");
                        bool weighted = (Console.ReadLine() == "t") ? true : false;

                        MenuGraph(new Graph(directed, weighted));
                        break;
                    case 2:
                        Console.Write("\tНазвание файла: ");
                        MenuGraph(new Graph(Console.ReadLine()));
                        break;
                    default:
                        break;
                }
            }
        }

        public static void MenuGraph(Graph g)
        {
            Console.Clear();
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
                Console.WriteLine("\t0. Выйти");
                

                Console.Write("Ваш выбор: ");
                select = int.Parse(Console.ReadLine());


                switch (select)
                {
                    // добавить вершину
                    case 1:
                        {
                            Console.Write("\tНазвание вершины: ");
                            g.AddVertex(Console.ReadLine());
                            break;
                        }
                    // добавить связь
                    case 2:
                        {
                            if (g.directed)
                            {
                                Console.Write("\tДобавить дугу: ");
                            }
                            else
                            {
                                Console.Write("\tДобавить ребро: ");
                            }

                            string[] link = Console.ReadLine().Split();

                            if (g.weighted)
                            {
                                g.AddLink(link[0], link[1], int.Parse(link[2]));
                            }
                            else
                            {
                                g.AddLink(link[0], link[1]);
                            }
                            break;
                        }
                    // удалить вершину
                    case 3:
                        {
                            Console.Write("\tНазвание вершины: ");
                            g.RemoveVertex(Console.ReadLine());
                            break;
                        }
                    // удалить связь
                    case 4:
                        {
                            if (g.directed)
                            {
                                Console.Write("\tУдалить дугу: ");
                            }
                            else
                            {
                                Console.Write("\tУдалить ребро: ");
                            }
                            string[] link = Console.ReadLine().Split();

                            g.RemoveLink(link[0], link[1]);
                            break;
                        }
                    // показать список смежности
                    case 5:
                        {
                            ShowLinks(g);
                            break;
                        }
                    case 6:
                        {
                            Console.Write("\tНазвание файла: ");
                            g.Save(Console.ReadLine());
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public static void ShowLinks(Graph g)
        {
            Console.WriteLine();
            foreach (var item in g.GetLinks())
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            MenuCreate();
        }
    }
}
