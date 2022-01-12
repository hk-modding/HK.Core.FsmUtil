using HutongGames.PlayMaker;
using System;

namespace Core.FsmUtil.Actions
{
    /// <summary>
    ///     FsmStateAction that invokes methods.
    /// </summary>
    public class MethodAction : FsmStateAction
    {
        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public Action method;

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
            if (method != null) method.Invoke();
            Finish();
        }
    }
}