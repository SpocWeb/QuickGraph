﻿using System;
using QuickGraph.Algorithms.Services;
using System.Diagnostics.Contracts;

namespace QuickGraph.Algorithms
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class RootedAlgorithmBase<TVertex,TGraph> 
        : AlgorithmBase<TGraph>
    {
        private TVertex rootVertex;
        private bool hasRootVertex;

        protected RootedAlgorithmBase(
            IAlgorithmComponent host,
            TGraph visitedGraph)
            :base(host, visitedGraph)
        {}

        public bool TryGetRootVertex(out TVertex rootVertex)
        {
            if (hasRootVertex)
            {
                rootVertex = this.rootVertex;
                return true;
            }
            else
            {
                rootVertex = default(TVertex);
                return false;
            }
        }

        public void SetRootVertex(TVertex rootVertex)
        {
            Contract.Requires(rootVertex != null);

            bool changed = !Equals(this.rootVertex, rootVertex);
            this.rootVertex = rootVertex;
            if (changed)
                OnRootVertexChanged(EventArgs.Empty);
            hasRootVertex = true;
        }

        public void ClearRootVertex()
        {
            rootVertex = default(TVertex);
            hasRootVertex = false;
        }

        public event EventHandler RootVertexChanged;
        protected virtual void OnRootVertexChanged(EventArgs e)
        {
            Contract.Requires(e != null);

            var eh = RootVertexChanged;
            if (eh != null)
                eh(this, e);
        }

        public void Compute(TVertex rootVertex)
        {
            Contract.Requires(rootVertex != null);

            SetRootVertex(rootVertex);
            Compute();
        }
    }
}
