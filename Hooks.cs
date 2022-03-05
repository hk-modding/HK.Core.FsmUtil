using System;
using System.ComponentModel;
using Core.FsmUtil.delegates;

namespace Core.FsmUtil
{
    namespace delegates
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate void FsmModificationHandler(PlayMakerFSM fsm);
    }

    /// <summary>
    ///     Class that handles special FSM hooks.
    /// </summary>
    public static class Hooks
    {
        private static Modding.ILogger logger = new Modding.SimpleLogger("Core.FsmUtil.Hooks");

        private static FsmModificationHandler _pmFsmBeforeStartHook;
        private static void InvokeBeforeStartHook(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            InvokeAll(_pmFsmBeforeStartHook, self);
            orig(self);
        }
        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Start()` is called.
        /// </summary>
        public static event FsmModificationHandler PmFsmBeforeStartHook
        {
            add
            {
                if (_pmFsmBeforeStartHook == null)
                {
                    On.PlayMakerFSM.Start += InvokeBeforeStartHook;
                }
                _pmFsmBeforeStartHook += value;
            }
            remove
            {
                _pmFsmBeforeStartHook -= value;
                if (_pmFsmBeforeStartHook == null)
                {
                    On.PlayMakerFSM.Start -= InvokeBeforeStartHook;
                }
            }
        }

        private static FsmModificationHandler _pmFsmAfterOnEnableHook;
        private static void InvokeAfterOnEnableHook(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            InvokeAll(_pmFsmAfterOnEnableHook, self);
        }
        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.OnEnable()` is called.
        /// </summary>
        public static event FsmModificationHandler PmFsmAfterOnEnableHook
        {
            add
            {
                if (_pmFsmAfterOnEnableHook == null)
                {
                    On.PlayMakerFSM.OnEnable += InvokeAfterOnEnableHook;
                }
                _pmFsmAfterOnEnableHook += value;
            }
            remove
            {
                _pmFsmAfterOnEnableHook -= value;
                if (_pmFsmAfterOnEnableHook == null)
                {
                    On.PlayMakerFSM.OnEnable -= InvokeAfterOnEnableHook;
                }
            }
        }

        private static void InvokeAll(FsmModificationHandler handler, PlayMakerFSM fsm)
        {
            if (handler == null)
            {
                return;
            }

            Delegate[] invocationList = handler.GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            {
                try
                {
                    (invocationList[i] as FsmModificationHandler).Invoke(fsm);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);
                    int_Logger.Log(ex.ToString());
                }
            }
        }
    }
}
