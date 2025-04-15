using System.Data;
using System.Diagnostics.Contracts;

namespace QuickGraph.Data
{
    public sealed class DataRelationEdge 
        : IEdge<DataTable>
    {
        private readonly DataRelation relation;
        public DataRelationEdge(DataRelation relation)
        {
            Contract.Requires(relation != null);

            this.relation = relation;
        }

        public DataRelation Relation => relation;

        public DataTable Source => relation.ParentTable;

        public DataTable Target => relation.ChildTable;
    }
}
