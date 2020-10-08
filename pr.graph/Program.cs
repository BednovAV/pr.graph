using System;
using System.IO;
using System.Xml;
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
                    case "3":
                        using (StreamReader input = new StreamReader("gr.xml"))
                        {
                            var reader = new System.Xml.Serialization.XmlSerializer(typeof(Graph));
                            MenuGraph((Graph)reader.Deserialize(input));
                        }
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
                    case "7":
                        {
                            using (StreamWriter output = new StreamWriter("gr.xml"))
                            {
                                var writer = new System.Xml.Serialization.XmlSerializer(typeof(Graph));
                                writer.Serialize(output, g);
                            }
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
