namespace QuickGraph.Graphviz.Dot
{
    using System;
    using System.IO;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    public class GraphvizLayerCollection : Collection<GraphvizLayer>
    {
        private string m_Separators = ":";

        public GraphvizLayerCollection()
        {}

        public GraphvizLayerCollection(GraphvizLayer[] items)
            :base(items)
        {}

        public GraphvizLayerCollection(GraphvizLayerCollection items)
            :base(items)
        {}

        public string ToDot()
        {
            if (Count == 0)
            {
                return null;
            }
            using (StringWriter writer = new StringWriter())
            {
                writer.Write("layers=\"");
                bool flag = false;
                foreach (GraphvizLayer layer in this)
                {
                    if (flag)
                    {
                        writer.Write(Separators);
                    }
                    else
                    {
                        flag = true;
                    }
                    writer.Write(layer.Name);
                }
                writer.WriteLine("\";");
                writer.WriteLine("layersep=\"{0}\"", Separators);
                return writer.ToString();
            }
        }

        public string Separators
        {
            get
            {
                return m_Separators;
            }
            set
            {
                Contract.Requires(!string.IsNullOrEmpty(value));

                m_Separators = value;
            }
        }
    }
}

