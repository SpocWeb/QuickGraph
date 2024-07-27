using System;
using QuickGraph.Graphviz.Dot;
using System.Diagnostics.Contracts;

namespace QuickGraph.Graphviz
{
    [ContractClass(typeof(IDotEngineContract))]
    public interface IDotEngine
    {
        string Run(
            GraphvizImageType imageType,
            string dot,
            string outputFileName);
    }

    [ContractClassFor(typeof(IDotEngine))]
    abstract class IDotEngineContract
        : IDotEngine
    {
        #region IDotEngine Members
        string IDotEngine.Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(dot));
            Contract.Requires(!string.IsNullOrEmpty(outputFileName));
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return null;
        }
        #endregion
    }
}
