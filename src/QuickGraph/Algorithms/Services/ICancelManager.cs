﻿using System;
using System.Threading;

namespace QuickGraph.Algorithms.Services
{
    public interface ICancelManager :
        IService
    {
        /// <summary>
        /// Raised when the cancel method is called
        /// </summary>
        event EventHandler CancelRequested;

        /// <summary>
        /// Requests the component to cancel its computation
        /// </summary>
        void Cancel();

        /// <summary>
        /// Gets a value indicating if a cancellation request is pending.
        /// </summary>
        /// <returns></returns>
        bool IsCancelling { get; }

        /// <summary>
        /// Raised when the cancel state has been reseted
        /// </summary>
        event EventHandler CancelReseted;

        /// <summary>
        /// Resets the cancel state
        /// </summary>
        void ResetCancel();
    }

    class CancelManager :
        ICancelManager
    {
        private int cancelling;

        public event EventHandler CancelRequested;

        public void Cancel()
        {
            var value = Interlocked.Increment(ref cancelling);
            if (value == 0)
            {
                var eh = CancelRequested;
                if (eh != null)
                    eh(this, EventArgs.Empty);
            }
        }

        public bool IsCancelling => cancelling > 0;

        /// <summary>
        /// Raised when the cancel state has been reseted
        /// </summary>
        public event EventHandler CancelReseted;

        /// <summary>
        /// Resets the cancel state
        /// </summary>
        public void ResetCancel()
        {
            var value = Interlocked.Exchange(ref cancelling, 0);
            if (value != 0)
            {
                var eh = CancelReseted;
                if (eh != null)
                    eh(this, EventArgs.Empty);
            }
        }
    }
}
