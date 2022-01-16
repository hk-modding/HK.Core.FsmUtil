namespace Core.FsmUtil
{
    namespace delegates
    {
        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Awake()` is called.  
        ///     In this hook, nothing in the FSM will be initialized yet.
        /// </summary>
        public delegate void PMFsmBeforeAwake(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Awake()` is called.  
        ///     In this hook, the PlayMakerGlobals will be initialized as well as the FSM template, if present, and the FSM itself.
        /// </summary>
        public delegate void PMFsmAfterAwake(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Start()` is called.  
        ///     In this hook, the FSM didn't execute any state actions yet, nor is the current state set, if it didn't already start.  
        ///     This is likely the hook for editing FSMs that you want.
        /// </summary>
        public delegate void PMFsmBeforeStart(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Start()` is called.  
        ///     In this hook, the FSM switched to its first state and started executing it.
        /// </summary>
        public delegate void PMFsmAfterStart(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.OnEnable()` is called.  
        ///     In this hook, the FSM is not added to the static `fsmList` yet, nor did the FSM restart if the `RestartOnEnable` flag is set.
        /// </summary>
        public delegate void PMFsmBeforeOnEnable(PlayMakerFSM fsm);

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.OnEnable()` is called.  
        ///     In this hook, the FSM is added to the static `fsmList` and restarted if the `RestartOnEnable` flag is set.
        /// </summary>
        public delegate void PMFsmAfterOnEnable(PlayMakerFSM fsm);
    }

    /// <summary>
    ///     Class that handles special FSM hooks.
    /// </summary>
    public static class Hooks
    {
        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Awake()` is called.
        /// </summary>
        /// <see cref="delegates.PMFsmBeforeAwake"/>
        /// <remarks>N/A</remarks>
        public static event delegates.PMFsmBeforeAwake PMFsmBeforeAwakeHook;

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Awake()` is called.
        /// </summary>
        /// <see cref="delegates.PMFsmAfterAwake"/>
        /// <remarks>N/A</remarks>
        public static event delegates.PMFsmAfterAwake PMFsmAfterAwakeHook;

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.Start()` is called.
        /// </summary>
        /// <see cref="delegates.PMFsmBeforeStart"/>
        /// <remarks>N/A</remarks>
        public static event delegates.PMFsmBeforeStart PMFsmBeforeStartHook;

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.Start()` is called.
        /// </summary>
        /// <see cref="delegates.PMFsmAfterStart"/>
        /// <remarks>N/A</remarks>
        public static event delegates.PMFsmAfterStart PMFsmAfterStartHook;

        /// <summary>
        ///     Hook that gets called before `PlayMakerFSM.OnEnable()` is called.
        /// </summary>
        /// <see cref="delegates.PMFsmBeforeOnEnable"/>
        /// <remarks>N/A</remarks>
        public static event delegates.PMFsmBeforeOnEnable PMFsmBeforeOnEnableHook;

        /// <summary>
        ///     Hook that gets called after `PlayMakerFSM.OnEnable()` is called.
        /// </summary>
        /// <see cref="delegates.PMFsmAfterOnEnable"/>
        /// <remarks>N/A</remarks>
        public static event delegates.PMFsmAfterOnEnable PMFsmAfterOnEnableHook;

        static Hooks()
        {
            On.PlayMakerFSM.Awake += (orig, self) =>
            {
                if (PMFsmBeforeAwakeHook != null)
                {
                    delegates.PMFsmBeforeAwake[] delegates = PMFsmBeforeAwakeHook.GetInvocationList() as delegates.PMFsmBeforeAwake[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
                orig(self);
                if (PMFsmAfterAwakeHook != null)
                {
                    delegates.PMFsmAfterAwake[] delegates = PMFsmAfterAwakeHook.GetInvocationList() as delegates.PMFsmAfterAwake[];
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
                if (PMFsmBeforeStartHook != null)
                {
                    delegates.PMFsmBeforeStart[] delegates = PMFsmBeforeStartHook.GetInvocationList() as delegates.PMFsmBeforeStart[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
                orig(self);
                if (PMFsmAfterStartHook != null)
                {
                    delegates.PMFsmAfterStart[] delegates = PMFsmAfterStartHook.GetInvocationList() as delegates.PMFsmAfterStart[];
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
                if (PMFsmBeforeOnEnableHook != null)
                {
                    delegates.PMFsmBeforeOnEnable[] delegates = PMFsmBeforeOnEnableHook.GetInvocationList() as delegates.PMFsmBeforeOnEnable[];
                    int delegatesCount = delegates.Length;
                    int i;
                    for (i = 0; i < delegatesCount; i++)
                    {
                        delegates[i].Invoke(self);
                    }
                }
                orig(self);
                if (PMFsmAfterOnEnableHook != null)
                {
                    delegates.PMFsmAfterOnEnable[] delegates = PMFsmAfterOnEnableHook.GetInvocationList() as delegates.PMFsmAfterOnEnable[];
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
