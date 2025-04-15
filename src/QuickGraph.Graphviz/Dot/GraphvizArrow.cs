namespace QuickGraph.Graphviz.Dot
{
    using System.IO;

    public class GraphvizArrow
    {
        private GraphvizArrowClipping clipping;
        private GraphvizArrowFilling filling;
        private GraphvizArrowShape shape;

        public GraphvizArrow(GraphvizArrowShape shape)
        {
            this.shape = shape;
            clipping = GraphvizArrowClipping.None;
            filling = GraphvizArrowFilling.Close;
        }

        public GraphvizArrow(GraphvizArrowShape shape, GraphvizArrowClipping clip, GraphvizArrowFilling fill)
        {
            this.shape = shape;
            clipping = clip;
            filling = fill;
        }

        public string ToDot()
        {
            using (StringWriter writer = new StringWriter())
            {
                if (filling == GraphvizArrowFilling.Open)
                {
                    writer.Write('o');
                }
                switch (clipping)
                {
                    case GraphvizArrowClipping.Left:
                        writer.Write('l');
                        break;

                    case GraphvizArrowClipping.Right:
                        writer.Write('r');
                        break;
                }
                writer.Write(shape.ToString().ToLower());
                return writer.ToString();
            }
        }

        public override string ToString()
        {
            return ToDot();
        }

        public GraphvizArrowClipping Clipping
        {
            get => clipping;
            set => clipping = value;
        }

        public GraphvizArrowFilling Filling
        {
            get => filling;
            set => filling = value;
        }

        public GraphvizArrowShape Shape
        {
            get => shape;
            set => shape = value;
        }
    }
}

