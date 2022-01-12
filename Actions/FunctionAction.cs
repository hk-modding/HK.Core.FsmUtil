using HutongGames.PlayMaker;
using System;

namespace Core.FsmUtil.Actions
{
    /// <summary>
    ///     FsmStateAction that invokes methods with an argument.
    ///     You will likely use this with a `HutongGames.PlayMaker.NamedVariable` as the generic argument
    /// </summary>
    public class FunctionAction<TArg> : FsmStateAction
    {
        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public Action<TArg> method;

        /// <summary>
        ///     The argument.
        /// </summary>
        public TArg arg;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            method = null;
            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (method != null) method.Invoke(arg);
            Finish();
        }
    }
}