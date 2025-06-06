// <copyright file="EdgeFactory.cs" company="MSIT">Copyright © MSIT 2007</copyright>

namespace QuickGraph
{
    public static partial class EdgeFactory
    {
        public static Edge<int> Create(int source, int target)
        {
            Edge<int> edge = new Edge<int>(source, target);
            return edge;
        }
    }
}
