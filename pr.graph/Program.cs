using System;
using System.IO;
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
            string select = "1";

            while (select != "0")
            {
                Console.Clear();
                Console.WriteLine("Возможные действия:");
                Console.WriteLine("\t1. Создать пустой граф");
                Console.WriteLine("\t2. Загрузить граф из файла");
                Console.WriteLine("\t0. Выйти");

                Console.Write("Ваш выбор: ");
                select = Console.ReadLine();


                switch (select)
                {
                    case "1":
                        Console.Write("\tОриентированный(t - true): ");
                        bool directed = (Console.ReadLine() == "t") ? true : false;

                        Console.Write("\tВзвешенный(t - true): ");
                        bool weighted = (Console.ReadLine() == "t") ? true : false;

                        MenuGraph(new Graph(directed, weighted));
                        break;
                    case "2":
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
            string select = "1";

            Console.WriteLine("Ориентированность - {0}", g.directed);
            Console.WriteLine("Взвешенность - {0}", g.weighted);

            Console.WriteLine("Возможные действия:");
            Console.WriteLine("\t1. Добавить вершину");
            if (g.directed)
            {
                Console.WriteLine("\t2. Добавить дугу");
            }
            else
            {
                Console.WriteLine("\t2. Добавить ребро");
            }
            Console.WriteLine("\t3. Удалить вершину");
            if (g.directed)
            {
                Console.WriteLine("\t4. Удалить дугу");
            }
            else
            {
                Console.WriteLine("\t4. Удалить ребро");
            }
            Console.WriteLine("\t5. Показать список смежности");
            Console.WriteLine("\t6. Сохранить граф");
            Console.WriteLine("\t7. Вывести все вершины графа, не смежные с данной");
            Console.WriteLine("\t8. Определить, существует ли вершина, в которую есть дуга из вершины u, но нет из v. Вывести такую вершину");
            Console.WriteLine("\t9. Построить орграф, являющийся обращением данного орграфа (каждая дуга перевёрнута)");
            Console.WriteLine("\t10. Найти сильно связные компоненты орграфa");
            Console.WriteLine("\t11. Вывести длины кратчайших (по числу рёбер) путей от всех вершин до u.");
            Console.WriteLine("\t12. Найти каркас минимального веса");
            Console.WriteLine("\t13. Вывести все кратчайшие пути до вершины u.");
            Console.WriteLine("\t14. *Найти k кратчайших путей между вершинами u и v");
            Console.WriteLine("\t15. Вывести цикл отрицательного веса, если он есть.");


            Console.WriteLine("\t0. Выйти");

            while (select != "0")
            {
                Console.Write("Ваш выбор: ");
                select = Console.ReadLine();


                switch (select)
                {
                    // добавить вершину
                    case "1":
                        {
                            Console.Write("\tНазвание добавляемой вершины: ");

                            // вершина добавляется в граф
                            if (g.AddVertex(Console.ReadLine()))
                            {
                                Console.WriteLine("Вершина добавлена");
                            }
                            else
                            {
                                Console.WriteLine("Вершина уже существует");
                            }
                            break;
                        }
                    // добавить связь
                    case "2":
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
                                if (link.Length == 3)
                                {
                                    if (!g.ContainsVertex(link[0]))
                                    {
                                        Console.WriteLine("Вершина {0} добавлена", link[0]);
                                    }

                                    if (!g.ContainsVertex(link[1]))
                                    {
                                        Console.WriteLine("Вершина {0} добавлена", link[1]);
                                    }

                                    g.AddLink(link[0], link[1], int.Parse(link[2]));
                                }
                                else
                                {
                                    Console.WriteLine("Неверное колличество параметров");
                                }
                            }
                            else
                            {
                                if (link.Length == 2)
                                {
                                    if (!g.ContainsVertex(link[0]))
                                    {
                                        Console.WriteLine("Вершина {0} добавлена", link[0]);
                                    }

                                    if (!g.ContainsVertex(link[1]))
                                    {
                                        Console.WriteLine("Вершина {0} добавлена", link[1]);
                                    }
                                    g.AddLink(link[0], link[1]);
                                }
                                else
                                {
                                    Console.WriteLine("Неверное колличество параметров");
                                }
                            }
                            break;
                        }
                    // удалить вершину
                    case "3":
                        {
                            Console.Write("\tНазвание удаляемой вершины: ");
                            if (g.RemoveVertex(Console.ReadLine()))
                            {
                                Console.WriteLine("Вершина удалена");
                            }
                            else
                            {
                                Console.WriteLine("Такой вершины нет");
                            }
                            break;
                        }
                    // удалить связь
                    case "4":
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

                            if (link.Length == 2)
                            {
                                if (g.RemoveLink(link[0], link[1]))
                                {
                                    Console.WriteLine("Удаление прошло успешно");
                                }
                                else
                                {
                                    Console.WriteLine("Что-то пошло не так");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неверное колличество параметров");
                            }
                                break;
                        }
                    // показать список смежности
                    case "5":
                        {
                            foreach (var item in g.LinkList())
                            {
                                Console.WriteLine(item);
                            }
                            Console.WriteLine();
                            break;
                        }
                    // сохранить граф
                    case "6":
                        {
                            Console.Write("\tНазвание файла: ");
                            g.Save(Console.ReadLine());
                            break;
                        }
                    // Вывести все вершины графа, не смежные с данной
                    case "7":
                        {
                            Console.Write("\tВершина: ");
                            foreach (var item in g.NotAdjacent(Console.ReadLine()))
                            {
                                Console.Write($"{item} ");
                            }
                            Console.WriteLine();
                            break;
                        }
                    // Определить, существует ли вершина, в которую есть дуга из вершины u, но нет из v. Вывести такую вершину
                    case "8":
                        {
                            Console.Write("\tВершина u: ");
                            string u = Console.ReadLine();
                            Console.Write("\tВершина v: ");
                            string v = Console.ReadLine();

                            foreach (var item in g.Task18(u, v))
                            {
                                Console.Write($"{item} ");
                            }
                            Console.WriteLine();
                            break;
                        }
                    // Построить орграф, являющийся обращением данного орграфа (каждая дуга перевёрнута)
                    case "9":
                        {
                            if (!g.directed)
                            {
                                Console.WriteLine("Граф должен быть ориентированным.");
                                break;
                            }
                            MenuGraph(g.Reversed());
                            break;
                        }
                    // Найти сильно связные компоненты орграфа
                    case "10":
                        {
                            if (!g.directed)
                            {
                                Console.WriteLine("Граф должен быть ориентированным.");
                                break;
                            }
                            else
                            {
                                List<List<string>> components = g.Kosaraju();

                                Console.WriteLine("Сильное связанные компоненты орграфа");
                                foreach (var comp in components)
                                {
                                    Console.Write("{");
                                    foreach (var ver in comp)
                                    {
                                        Console.Write($"{ver} ");
                                    }
                                    Console.WriteLine("}");
                                }
                            }
                            
                            break;
                        }
                    // Вывести длины кратчайших(по числу рёбер) путей от всех вершин до u.
                    case "11":
                        {
                            Console.Write("u: ");

                            foreach (var item in g.TaskII_30(Console.ReadLine()).OrderBy(v => v.Key))
                            {

                                Console.WriteLine($"{item.Key}: {item.Value}");
                            }    
                            break;
                        }
                    // Найти каркас минимального веса
                    case "12":
                        {
                            if (g.Kosaraju().Count != 1)
                            {
                                Console.WriteLine("Леса минимального веса: ");
                            }
                            else
                            {
                                Console.WriteLine("Дерево минимального веса:");
                            }
                            foreach (var item in g.Kruskal())
                            {
                                Console.WriteLine($"\t{item}");
                            }
                            Console.WriteLine();
                            break;
                        }
                    // Вывести все кратчайшие пути до вершины u.
                    case "13":
                        {
                            Console.Write("u:");
                            foreach (var item in g.TaskIVa_16(Console.ReadLine()))
                            {
                                Console.WriteLine($"\t{item}");
                            }
                            Console.WriteLine();
                            break;
                        }
                    // * Найти k кратчайших путей между вершинами u и v
                    case "14":
                        {
                            Console.Write("u: ");
                            string u = Console.ReadLine();
                            Console.Write("v: ");
                            string v = Console.ReadLine();
                            Console.Write("k: ");
                            int k = int.Parse(Console.ReadLine());
                            g.ShortPathes(u, v, k);
                            foreach (var item in g.ShortPathes(u, v, k))
                            {
                                Console.WriteLine($"{item.Key}: {item.Value}");
                            }
                            break;
                        }
                    //Вывести цикл отрицательного веса, если он есть
                    case "15":
                        {
                            Console.WriteLine(g.TaskIVc_18());
                            break;
                        }
                    default:
                        break;
                }
            }
        }


        static void Main(string[] args)
        {
            MenuCreate();
        }
    }
}
