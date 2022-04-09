using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DynamiteDexter
{
    public class Graph
    {
        private struct PathItem
        {
            public int parent;
            public int weight;

            public PathItem(int parent, int weight)
            {
                this.parent = parent;
                this.weight = weight;
            }
        }

        private class Vertex
        {
            public readonly Point Position;
            public bool InTree { get; set; }

            public Vertex(Point position)
            {
                Position = position;
                InTree = false;
            }
        }

        private const int MAX = Game1.GRID_SIZE_X * Game1.GRID_SIZE_Y;

        private Vertex[] vertices;
        private int[,] adjacency;
        private int numVertices;
        private int numInTree;
        private PathItem[] shortestPath;
        private int currentVertex;
        private int startToCurrent;

        public int Size { get { return numVertices; } }

        public Graph()
        {
            vertices = new Vertex[MAX];
            adjacency = new int[MAX, MAX];
            numVertices = 0;
            numInTree = 0;
            for (int i = 0; i < MAX; i++)
                for (int j = 0; j < MAX; j++)
                    adjacency[i, j] = int.MaxValue;
            shortestPath = new PathItem[MAX];
        }

        public void AddVertex(Point position) { vertices[numVertices++] = new Vertex(position); }

        public void AddEdge(int start, int end)
        {
            adjacency[start, end] = 1;
            adjacency[end, start] = 1;
        }

        public Point GetVertexData(int i) { return vertices[i].Position; }

        public void Path(int startTree)
        {
            currentVertex = 0;
            startToCurrent = 0;

            vertices[startTree].InTree = true;
            numInTree = 1;

            for (int i = 0; i < numVertices; i++)
                shortestPath[i] = new PathItem(startTree, adjacency[startTree, i]);

            while (numInTree < numVertices)
            {
                int indexMin = GetMinimum();

                if (shortestPath[indexMin].weight == int.MaxValue) //If minimum weight is infinite...
                {
                    Debug.WriteLine(
                        "Warning - Unreachable vertex in graph. " +
                        "(" + vertices[shortestPath[indexMin].parent].Position +
                        " -/-> " + vertices[indexMin].Position +
                        ")");
                    break;
                }
                else
                {
                    currentVertex = indexMin;
                    startToCurrent = shortestPath[indexMin].weight;
                }

                vertices[currentVertex].InTree = true;
                numInTree++;
                AdjustShortestPath();
            }

            //DisplayPaths();

            numInTree = 0;

            for (int i = 0; i < numVertices; i++)
                vertices[i].InTree = false;
        }

        private int GetMinimum()
        {
            int minWeight = int.MaxValue;
            int indexMin = 0;

            for (int i = 0; i < numVertices; i++)

                if (!vertices[i].InTree && shortestPath[i].weight < minWeight)
                {
                    minWeight = shortestPath[i].weight;
                    indexMin = i;
                }

            return indexMin;
        }

        private void AdjustShortestPath()
        {
            int column = 0;

            while (column < numVertices)
            {
                int currentToFringe = adjacency[currentVertex, column];

                if (vertices[column].InTree || currentToFringe == int.MaxValue)
                {
                    column++;
                    continue;
                }

                int startToFringe = startToCurrent + currentToFringe;
                int shortestPathWeight = shortestPath[column].weight;

                if (startToFringe < shortestPathWeight)
                    shortestPath[column] = new PathItem(currentVertex, startToFringe);

                column++;
            }
        }

        public Stack<Point> ShortestPathTo(int end)
        {
            Stack<Point> path = new Stack<Point>();
            AppendPathList(path, end);

            return path;
        }

        private void AppendPathList(Stack<Point> path, int i)
        {
            /*Debug.WriteLine(
                "Current vertex: " + i + "\n" +
                "Parent vertex: " + shortestPath[i].parent + "\n" +
                "Cost: " + shortestPath[i].weight + "\n" +
                "Position: " + vertices[i].Position);*/

            if (shortestPath[i].parent != i)
            {
                path.Push(vertices[i].Position);
                AppendPathList(path, shortestPath[i].parent);
            }
        }

        public void DisplayPaths()
        {
            string s = "";

            for (int i = 0; i < numVertices; i++)
            {
                s += vertices[i].Position
                    + " = "
                    + (shortestPath[i].weight == int.MaxValue ? "inf" : shortestPath[i].weight.ToString())
                    + "("
                    + vertices[shortestPath[i].parent].Position.ToString()
                    + ")\n";
            }

            Debug.WriteLine(s);
        }
    }
}
