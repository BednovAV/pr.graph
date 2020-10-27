using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel;

namespace pr.graph
{
    public class Graph
    {
        private class Link // ребро(дуга)
        {
            public string connectedVertex;
            public int weight;

            public Link(string vertex, int weight = 0)
            {
                this.connectedVertex = vertex;//смежная вершина
                this.weight = weight; // вес ребра(дуги)

            }
        }

        // словарь вершин, ключ - название вершины, значение - список смежных вершин 
        private Dictionary<string, HashSet<Link>> vertices;
        // флаг отвечающий за ориентированность графа
        public readonly bool directed;
        // флаг отвечающий за взвешенность графа
        public readonly bool weighted;



        //конструктор пустого графа
        public Graph()
        {
            vertices = new Dictionary<string, HashSet<Link>>();
            this.directed = false;
            this.weighted = false;
        }

        public Graph(bool directed, bool weighted)
        {
            vertices = new Dictionary<string, HashSet<Link>>();
            this.directed = directed;
            this.weighted = weighted;
        }

        //конструктор, заполняющий данные графа из файла
        public Graph(string inputName)
        {
            vertices = new Dictionary<string, HashSet<Link>>();

            using (StreamReader input = new StreamReader(inputName))
            {
                string[] str; // переменная для считывания 

                // считывание параметра ориентированности графа
                str = input.ReadLine().Split();
                directed = (str[0] == "True") ? true : false;
                weighted = (str[1] == "True") ? true : false;

                // считывание и заполнение списка вершин
                char[] sep = new char[]{ ' '};
                str = input.ReadLine().Split(sep , StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in str)
                {
                    AddVertex(item);
                }

                // считывание и заполнение списка ребер(дуг)
                string pair;
                while ((pair = input.ReadLine()) != "" && pair != null)
                {
                    str = pair.Split();
                    if (weighted)
                    {
                        AddLink(str[0], str[1], int.Parse(str[2]));
                    }
                    else
                    {
                        AddLink(str[0], str[1]);
                    }
                }
            }
        }

        // конструктор-копия
        public Graph(Graph g)
        {
            // создания пустого словаря вершин
            vertices = new Dictionary<string, HashSet<Link>>();

            // копирование параметров графа
            directed = g.directed;
            weighted = g.weighted;

            // копирование вершин
            foreach (var v in g.GetVertices())
            {
                AddVertex(v);
            }

            // копирование связей
            foreach (var e in g.GetLinks())
            {
                string[] pair = e.Split();
                AddLink(pair[0], pair[1], int.Parse(pair[2]));
            }
        }


        // метод добавляет вершину в граф и возвращает true если все прошло успешно
        public bool AddVertex(string name)
        {
            if (vertices.ContainsKey(name))
            {
                return false;
            }
            else
            {
                vertices.Add(name, new HashSet<Link>());
                return true;
            }
        }

        // метод добавляющий связь в граф
        public void AddLink(string v1, string v2, int weight = 1)
        {
            // если в графе отсутствует какая-то из вершин, то она будет добавлена
            if (!vertices.ContainsKey(v1))
            {
                this.AddVertex(v1);
            }
            if (!vertices.ContainsKey(v2))
            {
                this.AddVertex(v2);
            }

            // если в графе уже была добавляемая связь, то она будет удалена и добавлена заново
            vertices[v1].RemoveWhere(e => e.connectedVertex == v2);
            vertices[v1].Add(new Link(v2, weight));
            
            // если граф неориентированный, то связь будет двусторонней
            if (!directed)
            {
                vertices[v2].RemoveWhere(e => e.connectedVertex == v1);
                vertices[v2].Add(new Link(v1, weight));
            }
        }


        // метод удаляет вершину в графе, и возвращает true, если прошло успешно
        public bool RemoveVertex(string vertex)
        {
            foreach (var v in vertices)
            {
                v.Value.RemoveWhere(e => e.connectedVertex == vertex);
            }

            return vertices.Remove(vertex);
        }


        public bool RemoveLink(string v1, string v2)
        {// метод удаляет связь в графе и, если вce прошло успешно, возвращает true
            if (directed)
            {
                return RemoveArrow(v1, v2);
            }
            else
            {
                return RemoveEdge(v1, v2);
            }
        }

        private bool RemoveEdge(string v1, string v2)
        {
            return vertices[v1].RemoveWhere(e => e.connectedVertex == v2) == 1
                && vertices[v2].RemoveWhere(e => e.connectedVertex == v1) == 1;
        }

        private bool RemoveArrow(string v1, string v2)
        {
            return vertices[v1].RemoveWhere(e => e.connectedVertex == v2) == 1;
        }


        // метод возвращает список вершин графа
        public List<string> GetVertices()
        {
            List<string> ver = new List<string>(vertices.Keys);

            return ver;
        }

        // метод возвращает список смежности графа
        public List<string> GetLinks()
        {
            List<string> lines = new List<string>();


            if (weighted)
            {// если граф взвешенный в каждом элементе списка будет выводится три параметра
                foreach (var v in vertices)
                {
                    foreach (var e in v.Value)
                    {
                        lines.Add(string.Format("{0} {1} {2}", v.Key, e.connectedVertex, e.weight));
                    }
                }
            }
            else
            {// иначе две(без веса)
                foreach (var v in vertices)
                {
                    foreach (var e in v.Value)
                    {
                        lines.Add(string.Format("{0} {1}", v.Key, e.connectedVertex));
                    }
                }
            }

            return lines;
        }

        public List<string> LinkList()
        {
            List<string> result = new List<string>();

            foreach (var v in vertices)
            {
                string element = string.Format("{0}: ", v.Key);
                foreach (var e in v.Value)
                {
                    if (weighted)
                    {
                        element += string.Format("{0}, {1}; ", e.connectedVertex, e.weight);
                    }
                    else
                    {
                        element += string.Format("{0}; ", e.connectedVertex);
                    }
                }

                result.Add(element);
            }

            return result;
        }

        public bool ContainsVertex(string v)
        {
            return vertices.ContainsKey(v);
        }

        public void Save(string name)
        {
            using (StreamWriter output = new StreamWriter(name))
            {
                // вывод параметров графа
                output.WriteLine("{0} {1}", directed, weighted);

                // вывод списка вершин графа
                List<string> v = this.GetVertices();
                foreach (var item in v)
                {
                    output.Write("{0} ", item);
                }
                output.WriteLine();

                // вывод списка смежности графа
                List<string> e = this.GetLinks();
                foreach (var item in e)
                {
                    output.WriteLine(item);
                }
            }
        }

        // Возвращает вершинны смежные с данной
        public IEnumerable<string> Adjacent(string v)
        {
            List<string> result = new List<string>();

            foreach (var item in vertices[v])
            {
                result.Add(item.connectedVertex);
            }

            return result;
        }

        // Ia.15. Вывести все вершины графа, не смежные с данной.
        public IEnumerable<string> NotAdjacent(string v)
        {
            return vertices.Keys.Except(Adjacent(v));
        }

        // Ia.18. Определить, существует ли вершина, в которую есть дуга из вершины u, но нет из v. Вывести такую вершину.
        public IEnumerable<string> Task18(string u, string v)
        {
            return Adjacent(u).Except(Adjacent(v));
        }

        // Ib.4. Построить орграф, являющийся обращением данного орграфа (каждая дуга перевёрнута).
        public Graph Reversed()
        {
            if (!directed)
            {
                throw new Exception("Данная операция не поддерживается для неориентировннного графа");
            }

            Graph result = new Graph(directed, weighted);

            // все вершины исходного графа копируются в реверсивный
            foreach (var item in vertices.Keys)
            {
                result.AddVertex(item);
            }

            // обращение каждой дуги исходного графа и добавление в реверсивный
            foreach (var item in GetLinks())
            {
                string[] arrow = item.Split();
                if (weighted)
                {
                    result.AddLink(arrow[1], arrow[0], int.Parse(arrow[2]));
                }
                else
                {
                    result.AddLink(arrow[1], arrow[0]);
                }
            }
            return result;
        }


        // II.16. Найти сильно связные компоненты орграфа.

        // первый обход в глубину для алгоритма Косарайю для подсчета времени выхода из вершин
        private void Dfs1(string v, Dictionary<string, bool> visited, Dictionary<string, int> tout, ref int t)
        {
            visited[v] = true;

            foreach (var item in vertices[v])
            {
                if(!visited[item.connectedVertex])
                {
                    t++;
                    Dfs1(item.connectedVertex, visited, tout, ref t);
                }
            }
            tout[v] = t;
            t++;
        }

        // второй обход в глубину для алгоритма Косарайю для заполнения списка компонент связанности 
        private void Dfs2(Graph gr, string v, Dictionary<string, bool> visited, List<string> component, HashSet<string> entries)
        {
            visited[v] = true;
            component.Add(v);
            entries.Add(v);
            foreach (var item in gr.vertices[v])
            {
                if (!visited[item.connectedVertex])
                {
                    Dfs2(gr, item.connectedVertex, visited, component, entries);
                }
            }
        }

        // Алгоритм Косарайю для поиска сильно связанных компонент орграфа
        public List<List<string>> Kosaraju()
        {
                   
            // создание и инициализация словаря посещенных вершин для обходов в глубину
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach (var item in vertices.Keys)
            {
                visited.Add(item, false);
            }

            // создание и инициализация словаря для времён выхода из вершин
            Dictionary<string, int> tout = new Dictionary<string, int>();
            foreach (var item in vertices.Keys)
            {
                tout.Add(item, 0);
            }

            // таймер для подсчета времени
            int t = 0;

            // серия обходов в глубину для заполнения словаря времен выхода 
            foreach (var item in vertices.Keys)
            {
                if (!visited[item])
                {
                    Dfs1(item, visited, tout, ref t);
                }
            }

            // обнуление словаря посещенных вершин
            foreach (var item in vertices.Keys)
            {
                visited[item] = false;
            }

            // множество вершин уже добавленных в компоненты связанности
            HashSet<string> entries = new HashSet<string>();

            // инвертированный орграф
            Graph gr = Reversed();

            // список сильно связанных компонент
            List<List<string>> result = new List<List<string>>();

            // цикл по отсортированному словарю времен выхода (по убыванию времени)
            foreach (var item in tout.OrderByDescending(e => e.Value))
            {
                // если вершина не принадлежит в какой-либо компоненте связанности, 
                // то создаем новую компоненту, запускаем обход в глубиину и 
                // добавляем эту компоненту в список сильно связанных компонент
                if(!entries.Contains(item.Key))
                {
                    List<string> component = new List<string>();
                    Dfs2(gr, item.Key, visited, component, entries);
                    result.Add(component);
                }
            }

            return result;
        }

        // II.30. Вывести длины кратчайших (по числу рёбер) путей от всех вершин до u.
        public Dictionary<string, int> TaskII_30(string u)
        {
            Graph gr = Reversed();
            return gr.BfsLengh(u);
        }

        private Dictionary<string, int> BfsLengh(string v)
        {

            // создание и инициализация словаря посещенных вершин для обходов в глубину
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach (var item in vertices.Keys)
            {
                visited.Add(item, false);
            }

            Dictionary<string, int> result = new Dictionary<string, int>();


            visited[v] = true;
            result[v] = 0;
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(v);
            while (queue.Count != 0)
            {
                v = queue.Dequeue();
                foreach (var ver in vertices[v])
                {
                    if (!visited[ver.connectedVertex])
                    {
                        queue.Enqueue(ver.connectedVertex);
                        result[ver.connectedVertex] = result[v] + 1;
                        visited[ver.connectedVertex] = true;
                    }
                }
            }

            return result;
        }

        // алгоритм Флойда-Уоршелла по нахождению кратчайших расстояний (по количеству ребер) в графе
        private Dictionary<string, Dictionary<string, int>> Floyd()
        {
            // создание и заполнение таблицы смежности графа
            Dictionary<string, Dictionary<string, int>> g = new Dictionary<string, Dictionary<string, int>>();
            foreach (var i in vertices.Keys)
            {
                g.Add(i, new Dictionary<string, int>());

                // заполнение несмежных вершин
                foreach (var j in NotAdjacent(i))
                {
                    g[i].Add(j, 1000);
                }

                //заполнение смежных вершин
                foreach (var j in vertices[i])
                {
                    g[i].Add(j.connectedVertex, 1);
                }              
            }

            // вычисление гратчайших расстояний
            foreach (var i in vertices.Keys)
            {
                foreach (var j in vertices.Keys)
                {
                    foreach (var k in vertices.Keys)
                    {
                        if (g[j][k] > g[j][i] + g[i][k])
                        {
                            g[j][k] = g[j][i] + g[i][k];
                        }
                    }
                }
            }




            return g;
        }
        

    }
}
