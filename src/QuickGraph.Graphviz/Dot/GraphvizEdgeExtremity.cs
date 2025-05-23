namespace QuickGraph.Graphviz.Dot
{
    using System.Collections;
    using System.Diagnostics.Contracts;

    public class GraphvizEdgeExtremity
    {
        private bool isClipped;
        private readonly bool isHead;
        private string label;
        private string logical;
        private string same;
        private string tooltip;
        private string url;

        public GraphvizEdgeExtremity(bool isHead)
        {
            this.isHead = isHead;
            url = null;
            isClipped = true;
            label = null;
            tooltip = null;
            logical = null;
            same = null;
        }

        public void AddParameters(IDictionary dic)
        {
            Contract.Requires(dic != null);
            
            string text = null;
            if (IsHead)
            {
                text = "head";
            }
            else
            {
                text = "tail";
            }
            if (Url != null)
            {
                dic.Add(text + "URL", Url);
            }
            if (!IsClipped)
            {
                dic.Add(text + "clip", IsClipped);
            }
            if (Label != null)
            {
                dic.Add(text + "label", Label);
            }
            if (ToolTip != null)
            {
                dic.Add(text + "tooltip", ToolTip);
            }
            if (Logical != null)
            {
                dic.Add("l" + text, Logical);
            }
            if (Same != null)
            {
                dic.Add("same" + text, Same);
            }
        }

        public bool IsClipped
        {
            get => isClipped;
            set => isClipped = value;
        }

        public bool IsHead => isHead;

        public string Label
        {
            get => label;
            set => label = value;
        }

        public string Logical
        {
            get => logical;
            set => logical = value;
        }

        public string Same
        {
            get => same;
            set => same = value;
        }

        public string ToolTip
        {
            get => tooltip;
            set => tooltip = value;
        }

        public string Url
        {
            get => url;
            set => url = value;
        }
    }
}

