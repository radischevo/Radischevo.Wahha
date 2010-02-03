using System;
using System.Reflection;
using System.Threading;

namespace Radischevo.Wahha.Core
{
    public static class DelegateExtensions
    {
        #region Nested Types
        internal class TargetInfo
        {
            #region Instance Fields
            private Delegate _target;
            private object[] _arguments;
            #endregion

            #region Constructors
            internal TargetInfo(Delegate d, object[] args)
            {
                _target = d;
                _arguments = args;
            }
            #endregion

            #region Instance Properties
            public Delegate Target
            {
                get
                {
                    return _target;
                }
            }

            public object[] Arguments
            {
                get
                {
                    return _arguments;
                }
            }
            #endregion
        }
        #endregion

        #region Instance Fields
        private static WaitCallback _invokeShim = new WaitCallback(DynamicInvokeShim);
        #endregion

        #region Static Methods
        private static void DynamicInvokeShim(object obj)
        {
            try
            {
                TargetInfo ti = (TargetInfo)obj;
                ti.Target.DynamicInvoke(ti.Arguments);
            }
            catch (TargetInvocationException te)
            {
                throw te.InnerException;
            }
        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Dynamically invokes (using fire-and-forget strategy) 
        /// the method represented by the current delegate in the seperate thread.
        /// </summary>
        /// <param name="d">The delegate containing the method to invoke.</param>
        /// <param name="args">The argument list of a method.</param>
        public static void InvokeAndForget(this Delegate d, params object[] args)
        {
            ThreadPool.QueueUserWorkItem(_invokeShim, new TargetInfo(d, args));
        }
        #endregion
    }
}
