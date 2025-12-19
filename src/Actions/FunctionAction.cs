using System;
using HutongGames.PlayMaker;

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
        public Action<TArg> Method;

        /// <summary>
        ///     The argument.
        /// </summary>
        public TArg Arg;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            Method = null;
            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (Method != null && Arg != null)
            {
                Method.Invoke(Arg);
            }

            if ((!(Arg is Action tmpAction)) || (tmpAction != Finish))
            {
                Finish();
            }
        }
    }
}
