namespace QuickGraph.Graphviz.Dot
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    public static class GraphvizGraphX
    {
        public static string GenerateDotForGraph(this Hashtable pairs)
        {
            List<string> entries = new List<string>(pairs.Count);
            foreach (DictionaryEntry entry in pairs)
            {
                if (entry.Value is string)
                {
                    entries.Add(string.Format("{0}=\"{1}\"", entry.Key.ToString(), entry.Value.ToString()));
                    continue;
                }
                if (entry.Value is float floatValue)
                {
                    entries.Add(string.Format("{0}={1}", entry.Key.ToString(), floatValue.ToString(CultureInfo.InvariantCulture)));
                    continue;
                }
                if (entry.Value is double doubleValue)
                {
                    entries.Add(string.Format("{0}={1}", entry.Key.ToString(), doubleValue.ToString(CultureInfo.InvariantCulture)));
                    continue;
                }
                if (entry.Value is Color color)
                {
                    entries.Add(string.Format("{0}=\"#{1}{2}{3}{4}\"", new object[] { entry.Key.ToString(), color.R.ToString("x2").ToUpper(), color.G.ToString("x2").ToUpper(), color.B.ToString("x2").ToUpper(), color.A.ToString("x2").ToUpper() }));
                    continue;
                }
                if ((entry.Value is GraphvizRankDirection) || (entry.Value is GraphvizPageDirection))
                {
                    entries.Add(string.Format("{0}={1}", entry.Key.ToString(), entry.Value.ToString()));
                    continue;
                }
                entries.Add(string.Format(" {0}={1}", entry.Key.ToString(), entry.Value.ToString().ToLower()));
            }
            string result = string.Join(";", entries);
            result = entries.Count > 1 ? result + ";" : result;

            return result;
        }
    }

    public class GraphvizGraph
    {
        private string name = "G";
        private Color backgroundColor = Color.White;
        private GraphvizClusterMode clusterRank = GraphvizClusterMode.Local;
        private string comment = null;
        private Font font = null;
        private Color fontColor = Color.Black;
        private bool isCentered = false;
        private bool isCompounded = false;
        private bool isConcentrated = false;
        private bool isLandscape = false;
        private bool isNormalized = false;
        private bool isReMinCross = false;
        private string label = null;
        private GraphvizLabelJustification labelJustification = GraphvizLabelJustification.C;
        private GraphvizLabelLocation labelLocation = GraphvizLabelLocation.B;
        private readonly GraphvizLayerCollection layers = new GraphvizLayerCollection();
        private double mcLimit = 1;
        private double nodeSeparation = 0.25;
        private int nsLimit = -1;
        private int nsLimit1 = -1;
        private GraphvizOutputMode outputOrder = GraphvizOutputMode.BreadthFirst;
        private GraphvizPageDirection pageDirection = GraphvizPageDirection.BL;
        private SizeF pageSize = new SizeF(0, 0);
        private double quantum = 0;
        private GraphvizRankDirection rankDirection = GraphvizRankDirection.TB;
        private double rankSeparation = 0.5;
        private GraphvizRatioMode ratio = GraphvizRatioMode.Auto;
        private double resolution = 0.96;
        private int rotate = 0;
        private int samplePoints = 8;
        private int searchSize = 30;
        private SizeF size = new SizeF(0, 0);
        private string styleSheet = null;
        private string url = null;

        public string ToDot()
        {
            Hashtable pairs = new Hashtable();
            if (Url != null)
            {
                pairs["URL"] = Url;
            }
            if (BackgroundColor != Color.White)
            {
                pairs["bgcolor"] = BackgroundColor;
            }
            if (IsCentered)
            {
                pairs["center"] = true;
            }
            if (ClusterRank != GraphvizClusterMode.Local)
            {
                pairs["clusterrank"] = ClusterRank.ToString().ToLower();
            }
            if (Comment != null)
            {
                pairs["comment"] = Comment;
            }
            if (IsCompounded)
            {
                pairs["compound"] = IsCompounded;
            }
            if (IsConcentrated)
            {
                pairs["concentrated"] = IsConcentrated;
            }
            if (Font != null)
            {
                pairs["fontname"] = Font.Name;
                pairs["fontsize"] = Font.SizeInPoints;
            }
            if (FontColor != Color.Black)
            {
                pairs["fontcolor"] = FontColor;
            }
            if (Label != null)
            {
                pairs["label"] = Label;
            }
            if (LabelJustification != GraphvizLabelJustification.C)
            {
                pairs["labeljust"] = LabelJustification.ToString().ToLower();
            }
            if (LabelLocation != GraphvizLabelLocation.B)
            {
                pairs["labelloc"] = LabelLocation.ToString().ToLower();
            }
            if (Layers.Count != 0)
            {
                pairs["layers"] = Layers.ToDot();
            }
            if (McLimit != 1)
            {
                pairs["mclimit"] = McLimit;
            }
            if (NodeSeparation != 0.25)
            {
                pairs["nodesep"] = NodeSeparation;
            }
            if (IsNormalized)
            {
                pairs["normalize"] = IsNormalized;
            }
            if (NsLimit > 0)
            {
                pairs["nslimit"] = NsLimit;
            }
            if (NsLimit1 > 0)
            {
                pairs["nslimit1"] = NsLimit1;
            }
            if (OutputOrder != GraphvizOutputMode.BreadthFirst)
            {
                pairs["outputorder"] = OutputOrder.ToString().ToLower();
            }
            if (!PageSize.IsEmpty)
            {
                pairs["page"] = string.Format("{0},{1}", PageSize.Width, PageSize.Height);
            }
            if (PageDirection != GraphvizPageDirection.BL)
            {
                pairs["pagedir"] = PageDirection.ToString().ToLower();
            }
            if (Quantum > 0)
            {
                pairs["quantum"] = Quantum;
            }
            if (RankSeparation != 0.5)
            {
                pairs["ranksep"] = RankSeparation;
            }
            if (Ratio != GraphvizRatioMode.Auto)
            {
                pairs["ratio"] = Ratio.ToString().ToLower();
            }
            if (IsReMinCross)
            {
                pairs["remincross"] = IsReMinCross;
            }
            if (Resolution != 0.96)
            {
                pairs["resolution"] = Resolution;
            }
            if (Rotate != 0)
            {
                pairs["rotate"] = Rotate;
            }
            else if (IsLandscape)
            {
                pairs["orientation"] = "[1L]*";
            }
            if (SamplePoints != 8)
            {
                pairs["samplepoints"] = SamplePoints;
            }
            if (SearchSize != 30)
            {
                pairs["searchsize"] = SearchSize;
            }
            if (!Size.IsEmpty)
            {
                pairs["size"] = string.Format("{0},{1}", Size.Width, Size.Height);
            }
            if (StyleSheet != null)
            {
                pairs["stylesheet"] = StyleSheet;
            }
            if (RankDirection != GraphvizRankDirection.TB)
            {
                pairs["rankdir"] = RankDirection;
            }
            return pairs.GenerateDotForGraph();
        }

        public override string ToString()
        {
            return ToDot();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        public GraphvizClusterMode ClusterRank
        {
            get
            {
                return clusterRank;
            }
            set
            {
                clusterRank = value;
            }
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

        public bool IsCentered
        {
            get
            {
                return isCentered;
            }
            set
            {
                isCentered = value;
            }
        }

        public bool IsCompounded
        {
            get
            {
                return isCompounded;
            }
            set
            {
                isCompounded = value;
            }
        }

        public bool IsConcentrated
        {
            get
            {
                return isConcentrated;
            }
            set
            {
                isConcentrated = value;
            }
        }

        public bool IsLandscape
        {
            get
            {
                return isLandscape;
            }
            set
            {
                isLandscape = value;
            }
        }

        public bool IsNormalized
        {
            get
            {
                return isNormalized;
            }
            set
            {
                isNormalized = value;
            }
        }

        public bool IsReMinCross
        {
            get
            {
                return isReMinCross;
            }
            set
            {
                isReMinCross = value;
            }
        }

        public string Label
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

        public GraphvizLabelJustification LabelJustification
        {
            get
            {
                return labelJustification;
            }
            set
            {
                labelJustification = value;
            }
        }

        public GraphvizLabelLocation LabelLocation
        {
            get
            {
                return labelLocation;
            }
            set
            {
                labelLocation = value;
            }
        }

        public GraphvizLayerCollection Layers
        {
            get
            {
                return layers;
            }
        }

        public double McLimit
        {
            get
            {
                return mcLimit;
            }
            set
            {
                mcLimit = value;
            }
        }

        public double NodeSeparation
        {
            get
            {
                return nodeSeparation;
            }
            set
            {
                nodeSeparation = value;
            }
        }

        public int NsLimit
        {
            get
            {
                return nsLimit;
            }
            set
            {
                nsLimit = value;
            }
        }

        public int NsLimit1
        {
            get
            {
                return nsLimit1;
            }
            set
            {
                nsLimit1 = value;
            }
        }

        public GraphvizOutputMode OutputOrder
        {
            get
            {
                return outputOrder;
            }
            set
            {
                outputOrder = value;
            }
        }

        public GraphvizPageDirection PageDirection
        {
            get
            {
                return pageDirection;
            }
            set
            {
                pageDirection = value;
            }
        }

        public SizeF PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

        public double Quantum
        {
            get
            {
                return quantum;
            }
            set
            {
                quantum = value;
            }
        }

        public GraphvizRankDirection RankDirection
        {
            get
            {
                return rankDirection;
            }
            set
            {
                rankDirection = value;
            }
        }

        public double RankSeparation
        {
            get
            {
                return rankSeparation;
            }
            set
            {
                rankSeparation = value;
            }
        }

        public GraphvizRatioMode Ratio
        {
            get
            {
                return ratio;
            }
            set
            {
                ratio = value;
            }
        }

        public double Resolution
        {
            get
            {
                return resolution;
            }
            set
            {
                resolution = value;
            }
        }

        public int Rotate
        {
            get
            {
                return rotate;
            }
            set
            {
                rotate = value;
            }
        }

        public int SamplePoints
        {
            get
            {
                return samplePoints;
            }
            set
            {
                samplePoints = value;
            }
        }

        public int SearchSize
        {
            get
            {
                return searchSize;
            }
            set
            {
                searchSize = value;
            }
        }

        public SizeF Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public string StyleSheet
        {
            get
            {
                return styleSheet;
            }
            set
            {
                styleSheet = value;
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
    }
}

