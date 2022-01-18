using Core.FsmUtil.delegates;

namespace Core.FsmUtil
{
    namespace delegates
    {
        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Awake()` is called.  
        ///     In this hook, nothing in the FSM will be initialized yet.
        /// </summary>
        public delegate void PmFsmBeforeAwake(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Awake()` is called.  
        ///     In this hook, the PlayMakerGlobals will be initialized as well as the FSM template, if present, and the FSM itself.
        /// </summary>
        public delegate void PmFsmAfterAwake(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Start()` is called.  
        ///     In this hook, the FSM didn't execute any state actions yet, nor is the current state set, if it didn't already start.  
        ///     This is likely the hook for editing FSMs that you want.
        /// </summary>
        public delegate void PmFsmBeforeStart(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Start()` is called.  
        ///     In this hook, the FSM switched to its first state and started executing it.
        /// </summary>
        public delegate void PmFsmAfterStart(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.OnEnable()` is called.  
        ///     In this hook, the FSM is not added to the static `fsmList` yet, nor did the FSM restart if the `RestartOnEnable` flag is set.
        /// </summary>
        public delegate void PmFsmBeforeOnEnable(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.OnEnable()` is called.  
        ///     In this hook, the FSM is added to the static `fsmList` and restarted if the `RestartOnEnable` flag is set.
        /// </summary>
        public delegate void PmFsmAfterOnEnable(PlayMakerFSM fsm);
    }

    /// <summary>
    ///     Class that handles special FSM hooks.
    /// </summary>
    public static class Hooks
    {
        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Awake()` is called.
        /// </summary>
        /// <see cref="PmFsmBeforeAwake"/>
        /// <remarks>N/A</remarks>
        public static event PmFsmBeforeAwake PmFsmBeforeAwakeHook;

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Awake()` is called.
        /// </summary>
        /// <see cref="PmFsmAfterAwake"/>
        /// <remarks>N/A</remarks>
        public static event PmFsmAfterAwake PmFsmAfterAwakeHook;

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Start()` is called.
        /// </summary>
        /// <see cref="PmFsmBeforeStart"/>
        /// <remarks>N/A</remarks>
        public static event PmFsmBeforeStart PmFsmBeforeStartHook;

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Start()` is called.
        /// </summary>
        /// <see cref="PmFsmAfterStart"/>
        /// <remarks>N/A</remarks>
        public static event PmFsmAfterStart PmFsmAfterStartHook;

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.OnEnable()` is called.
        /// </summary>
        /// <see cref="PmFsmBeforeOnEnable"/>
        /// <remarks>N/A</remarks>
        public static event PmFsmBeforeOnEnable PmFsmBeforeOnEnableHook;

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.OnEnable()` is called.
        /// </summary>
        /// <see cref="PmFsmAfterOnEnable"/>
        /// <remarks>N/A</remarks>
        public static event PmFsmAfterOnEnable PmFsmAfterOnEnableHook;

        static Hooks()
        {
            On.PlayMakerFSM.Awake += (orig, self) =>
            {
                if (PmFsmBeforeAwakeHook != null)
                {
                    PmFsmBeforeAwake[] delegates = PmFsmBeforeAwakeHook.GetInvocationList() as PmFsmBeforeAwake[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
                orig(self);
                if (PmFsmAfterAwakeHook != null)
                {
                    PmFsmAfterAwake[] delegates = PmFsmAfterAwakeHook.GetInvocationList() as PmFsmAfterAwake[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
            };
            On.PlayMakerFSM.Start += (orig, self) =>
            {
                if (PmFsmBeforeStartHook != null)
                {
                    PmFsmBeforeStart[] delegates = PmFsmBeforeStartHook.GetInvocationList() as PmFsmBeforeStart[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
                orig(self);
                if (PmFsmAfterStartHook != null)
                {
                    PmFsmAfterStart[] delegates = PmFsmAfterStartHook.GetInvocationList() as PmFsmAfterStart[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
            };
            On.PlayMakerFSM.OnEnable += (orig, self) =>
            {
                if (PmFsmBeforeOnEnableHook != null)
                {
                    PmFsmBeforeOnEnable[] delegates = PmFsmBeforeOnEnableHook.GetInvocationList() as PmFsmBeforeOnEnable[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
                orig(self);
                if (PmFsmAfterOnEnableHook != null)
                {
                    PmFsmAfterOnEnable[] delegates = PmFsmAfterOnEnableHook.GetInvocationList() as PmFsmAfterOnEnable[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
            };
        }
    }
}
