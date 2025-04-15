namespace QuickGraph.Graphviz.Dot
{
    using System.Collections;
    using System.Drawing;

    public class GraphvizEdgeLabel
    {
        private double angle = -25;
        private double distance = 1;
        private bool @float = true;
        private Font font = null;
        private Color fontColor = Color.Black;
        private string value = null;

        public void AddParameters(IDictionary dic)
        {
            if (Value != null)
            {
                dic["label"] = Value;
                if (Angle != -25)
                {
                    dic["labelangle"] = Angle;
                }
                if (Distance != 1)
                {
                    dic["labeldistance"] = Distance;
                }
                if (!Float)
                {
                    dic["labelfloat"] = Float;
                }
                if (Font != null)
                {
                    dic["labelfontname"] = Font.Name;
                    dic["labelfontsize"] = Font.SizeInPoints;
                }
            }
        }

        public double Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        public double Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        public bool Float
        {
            get
            {
                return @float;
            }
            set
            {
                @float = value;
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

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}

