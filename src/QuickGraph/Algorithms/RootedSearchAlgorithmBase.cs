using System;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms.Services;

namespace QuickGraph.Algorithms
{
    public abstract class RootedSearchAlgorithmBase<TVertex, TGraph>
        : RootedAlgorithmBase<TVertex, TGraph>
    {
        private TVertex _goalVertex;
        private bool hasGoalVertex;

        protected RootedSearchAlgorithmBase(
            IAlgorithmComponent host,
            TGraph visitedGraph)
            :base(host, visitedGraph)
        {}

        public bool TryGetGoalVertex(out TVertex goalVertex)
        {
            if (hasGoalVertex)
            {
                goalVertex = _goalVertex;
                return true;
            }
            else
            {
                goalVertex = default(TVertex);
                return false;
            }
        }

        public void SetGoalVertex(TVertex goalVertex)
        {
            Contract.Requires(goalVertex != null);

            bool changed = !Equals(_goalVertex, goalVertex);
            _goalVertex = goalVertex;
            if (changed)
                OnGoalVertexChanged(EventArgs.Empty);
            hasGoalVertex = true;
        }

        public void ClearGoalVertex()
        {
            _goalVertex = default(TVertex);
            hasGoalVertex = false;
        }

        public event EventHandler GoalReached;
        protected virtual void OnGoalReached()
        {
            var eh = GoalReached;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        public event EventHandler GoalVertexChanged;
        protected virtual void OnGoalVertexChanged(EventArgs e)
        {
            Contract.Requires(e != null);

            var eh = GoalVertexChanged;
            if (eh != null)
                eh(this, e);
        }

        public void Compute(TVertex root, TVertex goal)
        {
            Contract.Requires(root != null);
            Contract.Requires(goal != null);

            SetGoalVertex(goal);
            Compute(root);
        }
    }
}
