namespace QuickGraph.Graphviz.Dot
{
    using System.Collections;
    using System.Drawing;
    using System.Globalization;
    using System.IO;

    public static class GraphvizEdgeX
    {
        public static string GenerateDotForEdge(this Hashtable pairs)
        {
            bool flag = false;
            StringWriter writer = new StringWriter();
            foreach (DictionaryEntry entry in pairs)
            {
                if (flag)
                {
                    writer.Write(", ");
                }
                else
                {
                    flag = true;
                }
                if (entry.Value is string)
                {
                    writer.Write("{0}=\"{1}\"", entry.Key.ToString(), entry.Value.ToString());
                    continue;
                }
                if (entry.Value is float floatValue)
                {
                    writer.Write("{0}={1}", entry.Key.ToString(), floatValue.ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (entry.Value is double doubleValue)
                {
                    writer.Write("{0}={1}", entry.Key.ToString(), doubleValue.ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (entry.Value is GraphvizEdgeDirection direction)
                {
                    writer.Write("{0}={1}", entry.Key.ToString(), direction.ToString().ToLower());
                    continue;
                }
                if (entry.Value is GraphvizEdgeStyle style)
                {
                    writer.Write("{0}={1}", entry.Key.ToString(), style.ToString().ToLower());
                    continue;
                }
                if (entry.Value is Color color)
                {
                    writer.Write("{0}=\"#{1}{2}{3}{4}\"", new object[] { entry.Key.ToString(), color.R.ToString("x2").ToUpper(), color.G.ToString("x2").ToUpper(), color.B.ToString("x2").ToUpper(), color.A.ToString("x2").ToUpper() });
                    continue;
                }
                writer.Write(" {0}={1}", entry.Key.ToString(), entry.Value.ToString().ToLower());
            }
            return writer.ToString();
        }
    }

    public class GraphvizEdge
    {
        private string comment = null;
        private GraphvizEdgeDirection dir = GraphvizEdgeDirection.Forward;
        private Font font = null;
        private Color fontColor = Color.Black;
        private GraphvizEdgeExtremity head = new GraphvizEdgeExtremity(true);
        private GraphvizArrow headArrow = null;
        private bool isConstrained = true;
        private bool isDecorated = false;
        private GraphvizEdgeLabel label = new GraphvizEdgeLabel();
        private GraphvizLayer layer = null;
        private int minLength = 1;
        private Color strokeColor = Color.Black;
        private GraphvizEdgeStyle style = GraphvizEdgeStyle.Unspecified;
        private GraphvizEdgeExtremity tail = new GraphvizEdgeExtremity(false);
        private GraphvizArrow tailArrow = null;
        private string tooltip = null;
        private string url = null;
        private double weight = 1;
        private int length = 1;

        public string ToDot()
        {
            Hashtable dic = new Hashtable();
            if (Comment != null)
            {
                dic["comment"] = Comment;
            }
            if (Dir != GraphvizEdgeDirection.Forward)
            {
                dic["dir"] = Dir.ToString().ToLower();
            }
            if (Font != null)
            {
                dic["fontname"] = Font.Name;
                dic["fontsize"] = Font.SizeInPoints;
            }
            if (FontColor != Color.Black)
            {
                dic["fontcolor"] = FontColor;
            }
            Head.AddParameters(dic);
            if (HeadArrow != null)
            {
                dic["arrowhead"] = HeadArrow.ToDot();
            }
            if (!IsConstrained)
            {
                dic["constraint"] = IsConstrained;
            }
            if (IsDecorated)
            {
                dic["decorate"] = IsDecorated;
            }
            Label.AddParameters(dic);
            if (Layer != null)
            {
                dic["layer"] = Layer.Name;
            }
            if (MinLength != 1)
            {
                dic["minlen"] = MinLength;
            }
            if (StrokeColor != Color.Black)
            {
                dic["color"] = StrokeColor;
            }
            if (Style != GraphvizEdgeStyle.Unspecified)
            {
                dic["style"] = Style.ToString().ToLower();
            }
            Tail.AddParameters(dic);
            if (TailArrow != null)
            {
                dic["arrowtail"] = TailArrow.ToDot();
            }
            if (ToolTip != null)
            {
                dic["tooltip"] = ToolTip;
            }
            if (Url != null)
            {
                dic["URL"] = Url;
            }
            if (Weight != 1)
            {
                dic["weight"] = Weight;
            }
            if (HeadPort != null)
                dic["headport"] = HeadPort;
            if (TailPort != null)
                dic["tailport"] = TailPort;
            if (length != 1)
                dic["len"] = length;
            return dic.GenerateDotForEdge();
        }

        public override string ToString()
        {
            return ToDot();
        }

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }

        public GraphvizEdgeDirection Dir
        {
            get
            {
                return dir;
            }
            set
            {
                dir = value;
            }
        }

        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        public Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }
        }

        public GraphvizEdgeExtremity Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
            }
        }

        public GraphvizArrow HeadArrow
        {
            get
            {
                return headArrow;
            }
            set
            {
                headArrow = value;
            }
        }

        public bool IsConstrained
        {
            get
            {
                return isConstrained;
            }
            set
            {
                isConstrained = value;
            }
        }

        public bool IsDecorated
        {
            get
            {
                return isDecorated;
            }
            set
            {
                isDecorated = value;
            }
        }

        public GraphvizEdgeLabel Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
            }
        }

        public GraphvizLayer Layer
        {
            get
            {
                return layer;
            }
            set
            {
                layer = value;
            }
        }

        public int MinLength
        {
            get
            {
                return minLength;
            }
            set
            {
                minLength = value;
            }
        }

        public Color StrokeColor
        {
            get
            {
                return strokeColor;
            }
            set
            {
                strokeColor = value;
            }
        }

        public GraphvizEdgeStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

        public GraphvizEdgeExtremity Tail
        {
            get
            {
                return tail;
            }
            set
            {
                tail = value;
            }
        }

        public GraphvizArrow TailArrow
        {
            get
            {
                return tailArrow;
            }
            set
            {
                tailArrow = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return tooltip;
            }
            set
            {
                tooltip = value;
            }
        }

        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }

        public double Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        public string HeadPort { get; set; }
        public string TailPort {get;set;}

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
    }
}

