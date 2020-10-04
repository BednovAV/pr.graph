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
                this.weight = weight;

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
                directed = (str[0] == "true") ? true : false;
                weighted = (str[1] == "true") ? true : false;
                
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

        public Graph(Graph g)
        {
            vertices = new Dictionary<string, List<Link>>();
            directed = g.directed;
            weighted = g.weighted;

            foreach (var v in g.GetVertices())
            {
                AddVertex(v);
            }

            foreach (var e in g.GetLinks())
            {
                string[] pair = e.Split();
                AddLink(pair[0], pair[1], int.Parse(pair[2]));
            }
        }

        public void AddVertex(string name)
        {
            if (vertices.ContainsKey(name))
            {
                throw new Exception("Такая вершина уже существует");
            }
            else
            {
                vertices.Add(name, new List<Link>());
            }
        }

        public void AddLink(string v1, string v2, int weight = 0)
        {
            if (directed)
            {
                AddArrow(v1, v2, weight);
            }
            else
            {
                AddEdge(v1, v2, weight);
            }
        }

        private void AddEdge(string v1, string v2, int weight = 0)
        {
           if (!vertices.ContainsKey(v1))
            {
                this.AddVertex(v1);
            }

            if (!vertices.ContainsKey(v2))
            {
                this.AddVertex(v2);
            }

            vertices[v1].RemoveAll(e => e.connectedVertex == v2);
            vertices[v1].Add(new Link(v2, weight));

            vertices[v2].RemoveAll(e => e.connectedVertex == v1);
            vertices[v2].Add(new Link(v1, weight));
        }

        private void AddArrow(string v1, string v2, int weight = 0)
        {
            if (!vertices.ContainsKey(v1))
            {
                this.AddVertex(v1);
            }

            if (!vertices.ContainsKey(v2))
            {
                this.AddVertex(v2);
            }

            vertices[v1].RemoveAll(e => e.connectedVertex == v2);
            vertices[v1].Add(new Link(v2, weight));
        }

        public void RemoveVertex(string vertex)
        {
            vertices.Remove(vertex);

            foreach (var v in vertices)
            {
                v.Value.RemoveAll(e => e.connectedVertex == vertex);
            }
        }

        public void RemoveLink(string v1, string v2)
        {
            if (directed)
            {
                RemoveArrow(v1, v2);
            }
            else
            {
                RemoveEdge(v1, v2);
            }
        }

        private void RemoveEdge(string v1, string v2)
        {
            vertices[v1].RemoveAll(e => e.connectedVertex == v2);
            vertices[v2].RemoveAll(e => e.connectedVertex == v1);
        }

        private void RemoveArrow(string v1, string v2)
        {
            vertices[v1].RemoveAll(e => e.connectedVertex == v2);
        }

        public List<string> GetVertices()
        {
            List<string> ver = new List<string>(vertices.Keys);

            return ver;
        }

        public List<string> GetLinks()
        {
            List<string> lines = new List<string>();

            if (weighted)
            {
                foreach (var v in vertices)
                {
                    foreach (var e in v.Value)
                    {
                        lines.Add(string.Format("{0} {1} {2}", v.Key, e.connectedVertex, e.weight));
                    }
                }
            }
            else
            {
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
                output.WriteLine("{0} {1}", directed, weighted);

                List<string> v = this.GetVertices();
                foreach (var item in v)
                {
                    output.Write("{0} ", item);
                }
                output.WriteLine();

                List<string> e = this.GetLinks();
                foreach (var item in e)
                {
                    output.WriteLine(item);
                }
            }
        }

        
    }
}
