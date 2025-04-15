namespace QuickGraph.Graphviz.Dot
{
    using System.Text;

    public class GraphvizRecordCell
    {
        private readonly GraphvizRecordCellCollection cells = new GraphvizRecordCellCollection();
        private readonly GraphvizRecordEscaper escaper = new GraphvizRecordEscaper();
        private string port = null;
        private string text = null;

        public string ToDot()
        {
            StringBuilder builder = new StringBuilder();
            if (HasPort)
            {
                builder.AppendFormat("<{0}> ", Escaper.Escape(Port));
            }
            if (HasText)
            {
                builder.AppendFormat("{0}", Escaper.Escape(Text));
            }
            if (Cells.Count > 0)
            {
                builder.Append(" { ");
                bool flag = false;
                foreach (GraphvizRecordCell cell in Cells)
                {
                    if (flag)
                    {
                        builder.AppendFormat(" | {0}", cell.ToDot());
                        continue;
                    }
                    builder.Append(cell.ToDot());
                    flag = true;
                }
                builder.Append(" } ");
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            return ToDot();
        }

        public GraphvizRecordCellCollection Cells => cells;

        protected GraphvizRecordEscaper Escaper => escaper;

        public bool HasPort
        {
            get
            {
                if (Port != null)
                {
                    return (Port.Length > 0);
                }
                return false;
            }
        }

        public bool HasText
        {
            get
            {
                if (text != null)
                {
                    return (text.Length > 0);
                }
                return false;
            }
        }

        public string Port
        {
            get => port;
            set => port = value;
        }

        public string Text
        {
            get => text;
            set => text = value;
        }
    }
}

