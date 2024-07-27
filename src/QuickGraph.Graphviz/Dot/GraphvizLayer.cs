namespace QuickGraph.Graphviz.Dot
{
    using System;
    using System.Diagnostics.Contracts;

    public class GraphvizLayer
    {
        private string name;

        public GraphvizLayer(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                Contract.Requires(!string.IsNullOrEmpty(value));
                this.name = value;
            }
        }
    }
}

