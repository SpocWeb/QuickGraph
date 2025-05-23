﻿using System;
using System.Diagnostics.Contracts;

namespace QuickGraph.Contracts
{
    [ContractClassFor(typeof(ICloneableEdge<,>))]
    abstract class ICloneableEdgeContract<TVertex, TEdge>
        : ICloneableEdge<TVertex,TEdge>
        where TEdge : IEdge<TVertex>
    {
        TEdge ICloneableEdge<TVertex, TEdge>.Clone(TVertex source, TVertex target)
        {
            Contract.Requires(source != null);
            Contract.Requires(target != null);
            Contract.Ensures(Contract.Result<TEdge>() != null);
            Contract.Ensures(Contract.Result<TEdge>().Source.Equals(source));
            Contract.Ensures(Contract.Result<TEdge>().Target.Equals(target));

            return default(TEdge);
        }

        #region IEdge<TVertex> Members

        TVertex IEdge<TVertex>.Source => throw new NotImplementedException();

        TVertex IEdge<TVertex>.Target => throw new NotImplementedException();

        #endregion
    }
}
