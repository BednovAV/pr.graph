using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;

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
        private Dictionary<string, List<Link>> vertices;
        // флаг отвечающий за ориентированность графа
        public readonly bool directed;
        // флаг отвечающий за взвешенность графа
        public readonly bool weighted;

        //конструктор пустого графа
        public Graph(bool directed = false, bool weighted = false)
        {
            vertices = new Dictionary<string, List<Link>>();
            this.directed = directed;
            this.weighted = weighted;
        }

        //конструктор, заполняющий данные графа из файла
        public Graph(string inputName)
        {
            vertices = new Dictionary<string, List<Link>>();

            using (StreamReader input = new StreamReader(inputName))
            {
                string[] str; // переменная для считывания 

                // считывание параметра ориентированности графа
                str = input.ReadLine().Split();
                directed = (str[0] == "True") ? true : false;
                weighted = (str[1] == "True") ? true : false;
                
                // считывание и заполнение списка вершин
                str = input.ReadLine().Split();
                foreach (var item in str)
                {
                    AddVertex(item);
                }

                // считывание и заполнение списка ребер(дуг)
                string pair;
                while ((pair = input.ReadLine()) != null)
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
            vertices = new Dictionary<string, List<Link>>();

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
                vertices.Add(name, new List<Link>());
                return true;
            }
        }

        // метод добавляющий связь в граф
        public void AddLink(string v1, string v2, int weight = 0)
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
            vertices[v1].RemoveAll(e => e.connectedVertex == v2);
            vertices[v1].Add(new Link(v2, weight));
            
            // если граф неориентированный, то связь будет двусторонней
            if (!directed)
            {
                vertices[v2].RemoveAll(e => e.connectedVertex == v1);
                vertices[v2].Add(new Link(v1, weight));
            }
        }


        // метод удаляет вершину в графе, и возвращает true, если прошло успешно
        public bool RemoveVertex(string vertex)
        {
            foreach (var v in vertices)
            {
                v.Value.RemoveAll(e => e.connectedVertex == vertex);
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
            return vertices[v1].RemoveAll(e => e.connectedVertex == v2) == 1
                && vertices[v2].RemoveAll(e => e.connectedVertex == v1) == 1;
        }

        private bool RemoveArrow(string v1, string v2)
        {
            return vertices[v1].RemoveAll(e => e.connectedVertex == v2) == 1;
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

        
    }
}
