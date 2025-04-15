namespace QuickGraph.Graphviz.Dot
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Collections.Generic;
    using System.Globalization;

    public static class GraphvizVertexX
    {
        public static string GenerateDotForVertex(this Dictionary<string, object> pairs)
        {
            bool flag = false;
            var writer = new StringWriter();
            foreach (var entry in pairs)
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
                    writer.Write("{0}=\"{1}\"", entry.Key, entry.Value.ToString());
                    continue;
                }
                if (entry.Value is float floatValue)
                {
                    writer.Write("{0}={1}", entry.Key, floatValue.ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (entry.Value is double doubleValue)
                {
                    writer.Write("{0}={1}", entry.Key, doubleValue.ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (entry.Value is GraphvizVertexShape vertexShape)
                {
                    writer.Write("{0}={1}", entry.Key, vertexShape.ToString().ToLower());
                    continue;
                }
                if (entry.Value is GraphvizVertexStyle vertexStyle)
                {
                    writer.Write("{0}={1}", entry.Key, vertexStyle.ToString().ToLower());
                    continue;
                }
                if (entry.Value is Color color)
                {
                    writer.Write("{0}=\"#{1}{2}{3}{4}\"", entry.Key, color.R.ToString("x2").ToUpper(), color.G.ToString("x2").ToUpper(), color.B.ToString("x2").ToUpper(), color.A.ToString("x2").ToUpper());
                    continue;
                }
                if (entry.Value is GraphvizRecord graphvizRecord)
                {
                    writer.WriteLine("{0}=\"{1}\"", entry.Key, graphvizRecord.ToDot());
                    continue;
                }
                writer.Write(" {0}={1}", entry.Key, entry.Value.ToString().ToLower());
            }
            return writer.ToString();
        }
    }

    public class GraphvizVertex
    {
        private string bottomLabel = null;
        private string comment = null;
        private double distorsion = 0;
        private Color fillColor = Color.White;
        private bool fixedSize = false;
        private Font font = null;
        private Color fontColor = Color.Black;
        private string group = null;
        private string label = null;
        private GraphvizLayer layer = null;
        private double orientation = 0;
        private int peripheries = -1;
        private GraphvizRecord record = new GraphvizRecord();
        private bool regular = false;
        private GraphvizVertexShape shape = GraphvizVertexShape.Unspecified;
        private int sides = 4;
        private SizeF size = new SizeF(0f, 0f);
        private double skew = 0;
        private Color strokeColor = Color.Black;
        private GraphvizVertexStyle style = GraphvizVertexStyle.Unspecified;
        private string toolTip = null;
        private string topLabel = null;
        private string url = null;
        private double z = -1;
        private Point? position;

        public string ToDot()
        {
            var pairs = new Dictionary<string, object>();
            if (Font != null)
            {
                pairs["fontname"] = Font.Name;
                pairs["fontsize"] = Font.SizeInPoints;
            }
            if (FontColor != Color.Black)
            {
                pairs["fontcolor"] = FontColor;
            }
            if (Shape != GraphvizVertexShape.Unspecified)
            {
                pairs["shape"] = Shape;
            }
            if (Style != GraphvizVertexStyle.Unspecified)
            {
                pairs["style"] = Style;
            }
            if (Shape == GraphvizVertexShape.Record)
            {
                pairs["label"] = Record;
            }
            else if (Label != null)
            {
                pairs["label"] = Label;
            }
            if (FixedSize)
            {
                pairs["fixedsize"] = true;
                if (Size.Height > 0f)
                {
                    pairs["height"] = Size.Height;
                }
                if (Size.Width > 0f)
                {
                    pairs["width"] = Size.Width;
                }
            }
            if (StrokeColor != Color.Black)
            {
                pairs["color"] = StrokeColor;
            }
            if (FillColor != Color.White)
            {
                pairs["fillcolor"] = FillColor;
            }
            if (Regular)
            {
                pairs["regular"] = Regular;
            }
            if (Url != null)
            {
                pairs["URL"] = Url;
            }
            if (ToolTip != null)
            {
                pairs["tooltip"] = ToolTip;
            }
            if (Comment != null)
            {
                pairs["comment"] = Comment;
            }
            if (Group != null)
            {
                pairs["group"] = Group;
            }
            if (Layer != null)
            {
                pairs["layer"] = Layer.Name;
            }
            if (Orientation > 0)
            {
                pairs["orientation"] = Orientation;
            }
            if (Peripheries >= 0)
            {
                pairs["peripheries"] = Peripheries;
            }
            if (Z > 0)
            {
                pairs["z"] = Z;
            }
            if (position.HasValue)
            {
                var p = position.Value;
                pairs["pos"] = string.Format("{0},{1}!", p.X, p.Y);
            }
            if (((Style == GraphvizVertexStyle.Diagonals) || (Shape == GraphvizVertexShape.MCircle)) || ((Shape == GraphvizVertexShape.MDiamond) || (Shape == GraphvizVertexShape.MSquare)))
            {
                if (TopLabel != null)
                {
                    pairs["toplabel"] = TopLabel;
                }
                if (BottomLabel != null)
                {
                    pairs["bottomlable"] = BottomLabel;
                }
            }
            if (Shape == GraphvizVertexShape.Polygon)
            {
                if (Sides != 0)
                {
                    pairs["sides"] = Sides;
                }
                if (Skew != 0)
                {
                    pairs["skew"] = Skew;
                }
                if (Distorsion != 0)
                {
                    pairs["distorsion"] = Distorsion;
                }
            }

            return pairs.GenerateDotForVertex();
        }

        public override string ToString()
        {
            return ToDot();
        }

        public Point? Position
        {
            get => position;
            set => position = value;
        }

        public string BottomLabel
        {
            get => bottomLabel;
            set => bottomLabel = value;
        }

        public string Comment
        {
            get => comment;
            set => comment = value;
        }

        public double Distorsion
        {
            get => distorsion;
            set => distorsion = value;
        }

        public Color FillColor
        {
            get => fillColor;
            set => fillColor = value;
        }

        public bool FixedSize
        {
            get => fixedSize;
            set => fixedSize = value;
        }

        public Font Font
        {
            get => font;
            set => font = value;
        }

        public Color FontColor
        {
            get => fontColor;
            set => fontColor = value;
        }

        public string Group
        {
            get => group;
            set => group = value;
        }

        public string Label
        {
            get => label;
            set => label = value;
        }

        public GraphvizLayer Layer
        {
            get => layer;
            set => layer = value;
        }

        public double Orientation
        {
            get => orientation;
            set => orientation = value;
        }

        public int Peripheries
        {
            get => peripheries;
            set => peripheries = value;
        }

        public GraphvizRecord Record
        {
            get => record;
            set => record = value;
        }

        public bool Regular
        {
            get => regular;
            set => regular = value;
        }

        public GraphvizVertexShape Shape
        {
            get => shape;
            set => shape = value;
        }

        public int Sides
        {
            get => sides;
            set => sides = value;
        }

        public SizeF Size
        {
            get => size;
            set => size = value;
        }

        public double Skew
        {
            get => skew;
            set => skew = value;
        }

        public Color StrokeColor
        {
            get => strokeColor;
            set => strokeColor = value;
        }

        public GraphvizVertexStyle Style
        {
            get => style;
            set => style = value;
        }

        public string ToolTip
        {
            get => toolTip;
            set => toolTip = value;
        }

        public string TopLabel
        {
            get => topLabel;
            set => topLabel = value;
        }

        public string Url
        {
            get => url;
            set => url = value;
        }

        public double Z
        {
            get => z;
            set => z = value;
        }
    }
}

