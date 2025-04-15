using System.Data;
using QuickGraph.Algorithms;
using System.Diagnostics.Contracts;

namespace QuickGraph.Data
{
    public sealed class DataSetGraphPopulatorAlgorithm :
        AlgorithmBase<IMutableVertexAndEdgeListGraph<DataTable, DataRelationEdge>>
    {
        private readonly DataSet dataSet;

        public DataSetGraphPopulatorAlgorithm(
            IMutableVertexAndEdgeListGraph<DataTable, DataRelationEdge> visitedGraph,
            DataSet dataSet
            )
            : base(visitedGraph)
        {
            Contract.Requires(dataSet != null);

            this.dataSet = dataSet;
        }

        public DataSet DataSet
        {
            get { return dataSet; }
        }

        protected override void InternalCompute()
        {
            foreach (DataTable table in DataSet.Tables)
                VisitedGraph.AddVertex(table);

            foreach (DataRelation relation in DataSet.Relations)
                VisitedGraph.AddEdge(new DataRelationEdge(relation));
        }
    }
}
