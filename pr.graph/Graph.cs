﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel;
using System.Diagnostics;

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

            // словарь для длин путей
            Dictionary<string, int> result = new Dictionary<string, int>();

            // очередь для обхода в ширину
            Queue<string> queue = new Queue<string>();

            // отмечаем начальную вершину как посещенную и добавляем ее в очередь
            visited[v] = true;
            result[v] = 0;
            queue.Enqueue(v);

            // просмотр очереди
            while (queue.Count != 0)
            {
                v = queue.Dequeue();
                // смотрим вершины смежные с текущей
                foreach (var ver in vertices[v])
                {
                    // если вершина не посещена, то добавляем ее в очередь и считаем путь к ней
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

        


        // III.Краскал Дан взвешенный неориентированный граф из N вершин и M ребер. Требуется найти в нем каркас минимального веса.
        public List<string> Kruskal()
        {

            // словарь ребер
            Dictionary<KeyValuePair<string, string>, int> edges = new Dictionary<KeyValuePair<string, string>, int>();
            foreach (var ver in vertices.Keys)
            {
                foreach (var edg in vertices[ver])
                {
                    // проверка на вхождение в словарь обратного ребра
                    KeyValuePair<string, string> pair = new KeyValuePair<string, string>(edg.connectedVertex, ver);
                    if (!edges.ContainsKey(pair))
                    {
                        edges.Add(new KeyValuePair<string, string>(ver, edg.connectedVertex), edg.weight);
                    }
                }
            }

            // создание и инициализация деревьев
            Dictionary<string, int> treeId = new Dictionary<string, int>();
            int id = 0;
            foreach (var ver in vertices.Keys)
            {
                treeId[ver] = id++;
            }

            // подграф для каркаса минимального веса
            Graph f = new Graph(directed, weighted);
            // идем по отсортированному в порядке возрастания веса словарю ребер
            foreach (var edge in edges.OrderBy(e => e.Value))
            {
                string start = edge.Key.Key;// начальная вершина
                string end = edge.Key.Value;// конечная

                // если вершины ребра из разных поддеревьев, то добавляем их в каркас
                if (treeId[start] != treeId[end])
                {
                    f.AddLink(start, end, edge.Value);

                    // обьединение поддеревьев
                    int oldId = treeId[end];
                    foreach (var ver in vertices.Keys)
                    {
                        if (treeId[ver] == oldId)
                        {
                            treeId[ver] = treeId[start];
                        }
                    }
                }
            }

            return f.LinkList();
        }



        // IV.a.16 Вывести все кратчайшие пути до вершины u.
        public List<string> TaskIVa_16(string u)
        {
            List<string> result = new List<string>();

            Dictionary<string, string> ways = Reversed().Dijkstr(u);

            foreach (var item in ways.Keys)
            {
                StringBuilder way = new StringBuilder(item);
                string now = item;
                while (ways[now] != u)
                {
                    way.Append($" {ways[now]}");
                    now = ways[now];
                }
                way.Append($" {u}");

                result.Add(way.ToString());
            }

            //foreach (var item in Reversed().Dijkstr(u))
            //{
            //    Console.WriteLine($"{item.Key} {item.Value}");
            //}

            return result;
        }

        public Dictionary<string, string> Dijkstr(string v)
        {
            // создание и инициализация словаря посещенных вершин для обходов в глубину
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach (var item in vertices.Keys)
            {
                visited.Add(item, false);
            }

            visited[v] = true; // помечаем вершину v как просмотренную
            //создаем матрицу с
            Dictionary<string, Dictionary<string, int>> c = new Dictionary<string, Dictionary<string, int>>();
            foreach (var i in vertices.Keys)
            {
                c.Add(i, new Dictionary<string, int>());

                // заполнение несмежных вершин
                foreach (var j in NotAdjacent(i))
                {
                    c[i].Add(j, int.MaxValue);
                }

                //заполнение смежных вершин
                foreach (var j in vertices[i])
                {
                    c[i].Add(j.connectedVertex, j.weight);
                }
            }

            //создаем матрицы d и p
            Dictionary<string, long> d = new Dictionary<string, long>();
            Dictionary<string, string> p = new Dictionary<string, string>();

            foreach (var u in vertices.Keys)
            {
                if (u != v)
                {
                    d[u] = c[v][u];
                    p[u] = v;
                }
            }

            foreach (var i in vertices.Keys)
            {
                // выбираем из множества V\S такую вершину w, что D[w] минимально
                long min = int.MaxValue;
                string w = "";
                foreach (var u in vertices.Keys)
                {
                    if (!visited[u] && min > d[u])
                    {
                        min = d[u];
                        w = u;
                    }
                }

                if (w == "") break;

                visited[w] = true; //помещаем w в множество S
                                   //для каждой вершины из множества V\S определяем кратчайший путь от
                                   // источника до этой вершины
                foreach (var u in vertices.Keys)
                {
                    long distance = d[w] + c[w][u];
                    if (!visited[u] && d[u] > distance)
                    {
                        d[u] = distance;
                        p[u] = w;
                    }
                }
            }

            //в качестве результата возвращаем словарь кратчайших путей для
            //заданного источника
            return p;
        }

        //IV.b.22* Найти k кратчайших путей между вершинами u и v.
        public List<KeyValuePair<string, int>> ShortPathes(string u, string v, int k)
        {
            // список кратчайший путей
            List<YenPath> pathes = new List<YenPath>();

            // применяем алгоритм Флойда к графу
            Dictionary<string, Dictionary<string, string>> path;
            Dictionary<string, Dictionary<string, int>> dist = Floyd(out path);
            pathes.Add(new YenPath(ExtractPath(path, u, v), dist[u][v], new List<KeyValuePair<string, string>>()));

            Yen(pathes, u, v, k);
            pathes = new List<YenPath>(pathes.Distinct());
            pathes.Sort();

            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            foreach (var item in pathes.GetRange(0, k))
            {
                result.Add(new KeyValuePair<string, int>(item.path, item.dist));
            }

            //foreach (var item in pathes)
            //{
            //    Console.Write($"{item.path}, {item.dist}, ");
            //    foreach (var m in item.missing)
            //    {
            //        Console.Write($"[{m.Key}, {m.Value}]");
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine($"{ExtractPath(path, a, b)} {dist[a][b]}");
            return result;
        }
        struct YenPath: IComparable<YenPath>, IEquatable<YenPath>
        {
            public string path;
            public int dist;
            public List<KeyValuePair<string, string>> missing;
            public YenPath(string path, int dist, List<KeyValuePair<string, string>> missing)
            {
                this.path = path;
                this.dist = dist;
                this.missing = missing;
            }

            public int CompareTo(YenPath other)
            {
                return dist.CompareTo(other.dist);
            }

            public bool Equals(YenPath other)
            {
                if (path == other.path)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void Yen(List<YenPath> pathes, string u, string v, int k)
        {
            while (pathes.Distinct().Count() < k)
            {
                int maxMiss = 0;
                foreach (var item in pathes)
                {
                    if(item.missing.Count > maxMiss)
                    {
                        maxMiss = item.missing.Count;
                    }
                }

                List<YenPath> nowPathes = pathes.FindAll(e => e.missing.Count == maxMiss);

                foreach (var item in nowPathes)
                {
                    string[] path = item.path.Split(' ');
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        List<KeyValuePair<string, string>> newMissing = new List<KeyValuePair<string, string>>(item.missing);
                        newMissing.Add(new KeyValuePair<string, string>(path[i], path[i + 1]));
                        AddWayToList(pathes, newMissing, u, v);
                    }
                }
            }
        }

        private void AddWayToList(List<YenPath> pathes, List<KeyValuePair<string, string>> missing, string u, string v)
        {
            Graph g = new Graph(this);
            foreach (var e in missing)
            {
                g.RemoveLink(e.Key, e.Value);
            }

            // применяем алгоритм Флойда к графу
            Dictionary<string, Dictionary<string, string>> path;
            Dictionary<string, Dictionary<string, int>> dist = g.Floyd(out path);
            if (path[u][v] != "no way")
            {
                YenPath yenPath = new YenPath(ExtractPath(path, u, v), dist[u][v], missing);
                //if (!pathes.Contains(yenPath))
                {
                    pathes.Add(yenPath);
                }
            }
        }

        private string ExtractPath(Dictionary<string, Dictionary<string, string>> pathes, string u, string v)
        {
            string path = u;
            string now = u;

            while(pathes[now][v] != v)
            {
                path += $" {pathes[now][v]}";
                now = pathes[now][v];
            }
            path += $" {v}";
            return path;
        }

        //IV.c.18 Вывести цикл отрицательного веса, если он есть
        public string TaskIVc_18()
        {
            // применяем алгоритм Флойда к графу
            Dictionary<string, Dictionary<string, string>> pathes;
            Dictionary<string, Dictionary<string, int>> dists = Floyd(out pathes);

            // если на главной диагонали получившейся матрицы есть отрицательные длины,
            // то значит граф содержит цикл отрицательного веса
            string negVer = "";
            foreach (var i in dists.Keys)
            {
                if (dists[i][i] < 0)
                {
                    negVer = i;
                    break;
                }
            }
            if (negVer == "") return "Циклы отрицательного веса отсутствуют";

            // находим цикл по матрице путей
            StringBuilder way = new StringBuilder(negVer);
            string now = negVer;
            while (pathes[now][negVer] != negVer)
            {
                way.Append($" {pathes[now][negVer]}");
                now = pathes[now][negVer];
                if (way.ToString().Contains($" {now} "))
                {
                    way = new StringBuilder(now);
                    negVer = now;
                }
            }
            way.Append($" {pathes[now][negVer]}");

            //foreach (var v1 in vertices.Keys)
            //{
            //    foreach (var v2 in vertices.Keys)
            //    {
            //        Console.WriteLine($"Dist[{v1}][{v2}]{dists[v1][v2]}");
            //        Console.WriteLine($"Path[{v1}][{v2}]{pathes[v1][v2]}");
            //    }
            //}

            return way.ToString();
        }

        // алгоритм Флойда-Уоршелла по нахождению кратчайших расстояний (по количеству ребер) в графе
        private Dictionary<string, Dictionary<string, int>> Floyd(out Dictionary<string, Dictionary<string, string>> pathes)
        {
            // создание и заполнение таблицы смежности графа
            Dictionary<string, Dictionary<string, int>> g = new Dictionary<string, Dictionary<string, int>>();
            pathes = new Dictionary<string, Dictionary<string, string>>();
            foreach (var i in vertices.Keys)
            {
                g.Add(i, new Dictionary<string, int>());
                pathes.Add(i, new Dictionary<string, string>());

                // заполнение несмежных вершин
                foreach (var j in NotAdjacent(i))
                {
                    g[i].Add(j, int.MaxValue / 2);
                    pathes[i].Add(j, "no way");
                }

                //заполнение смежных вершин
                foreach (var j in vertices[i])
                {
                    g[i].Add(j.connectedVertex, j.weight);
                    pathes[i].Add(j.connectedVertex, j.connectedVertex);
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
                            pathes[j][k] = pathes[j][i];
                        }
                    }
                }
            }

            return g;
        }

        bool Bfs(Dictionary<string, Dictionary<string, int>> matrix, string s, string t, Dictionary<string, string> parent)
        {
            // словарь посещенный вершин 
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            foreach (var item in vertices.Keys)
            {
                visited.Add(item, false);
            }
 
            Queue<string> q = new Queue<string>();
            q.Enqueue(s);
            visited[s] = true;
            parent[s] = "";
 
            while (q.Count != 0)
            {
                string u = q.Dequeue();

                foreach (var v in matrix[u].Keys)
                {
                    if (visited[v] == false && matrix[u][v] > 0)
                    {
                        q.Enqueue(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }

            // если мы достигли стока то метод вернет true
            return (visited[t] == true);
        }

        // Returns the maximum flow from s to t in the given graph 
        public int FordFulkerson(string s = "s", string t = "t")
        {
            string u, v;

            //создаем матрицу смежности
            Dictionary<string, Dictionary<string, int>> matrix = new Dictionary<string, Dictionary<string, int>>();
            foreach (var i in vertices.Keys)
            {
                matrix.Add(i, new Dictionary<string, int>());

                // заполнение несмежных вершин
                foreach (var j in NotAdjacent(i))
                {
                    matrix[i].Add(j, 0);
                }

                //заполнение смежных вершин
                foreach (var j in vertices[i])
                {
                    matrix[i].Add(j.connectedVertex, j.weight);
                }
            }

            // словарь для хранения путей найденный в bfs 
            Dictionary<string, string> parent = new Dictionary<string, string>();  

            int maxFlow = 0;  

            // Пока есть путь от источника до стока, увеличиваем поток
            while (Bfs(matrix, s, t, parent))
            {
                // находим минимальную пропускную способность на пути
                int path_flow = int.MaxValue;
                for (v = t; v != s; v = parent[v])
                {
                    u = parent[v];
                    path_flow = Math.Min(path_flow, matrix[u][v]);
                }

                // обновляем пропускные способности дуг и обратных дуг
                for (v = t; v != s; v = parent[v])
                {
                    u = parent[v];
                    matrix[u][v] -= path_flow;
                    matrix[v][u] += path_flow;
                }

                // прибавляем найденный поток к общему
                maxFlow += path_flow;
            }

            return maxFlow;
        }

    }
}
