using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Core.FsmUtil.delegates;
using HutongGames.PlayMaker;
using MonoMod.RuntimeDetour;

namespace Core.FsmUtil
{
    namespace delegates
    {
        /// <summary>
        ///     Delegate that receives a PlayMakerFSM.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate void FsmModificationHandler(PlayMakerFSM fsm);
    }

    /// <summary>
    ///     Class that handles special FSM hooks.
    /// </summary>
    public static class Hooks
    {
        static Hooks()
        {
            _ = new Hook(typeof(Fsm).GetMethod("EnterState", BindingFlags.Instance | BindingFlags.NonPublic), EnterState);
            _ = new Hook(typeof(Fsm).GetMethod("DoTransition", BindingFlags.Instance | BindingFlags.NonPublic), DoTransition);
            _ = new Hook(typeof(Fsm).GetMethod("ExitState", BindingFlags.Instance | BindingFlags.NonPublic), ExitState);
        }

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
                    InternalLogger.LogError($"[Core][FsmUtil][Hooks] - {ex}");
                }
            }
        }

        private static Dictionary<FSMData, Action<PlayMakerFSM>> StateEnterData = new Dictionary<FSMData, Action<PlayMakerFSM>>();
        private static Dictionary<FSMData, Action<PlayMakerFSM>> StateExitData = new Dictionary<FSMData, Action<PlayMakerFSM>>();
        private static Dictionary<FSMData, Action<PlayMakerFSM, string>> StateEnteredFromTransitionData = new Dictionary<FSMData, Action<PlayMakerFSM, string>>();
        private static Dictionary<FSMData, Action<PlayMakerFSM, string>> StateExitedViaTransitionData = new Dictionary<FSMData, Action<PlayMakerFSM, string>>();

        /// <summary>
        /// Hook that gets called when a state is entered by any means (a transition, a global transition, or from Fsm.SetState)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateEnter">The action that will be invoked when the state is entered, the parameter passed into the action is the fsm</param>
        public static void HookStateEntered(FSMData data, Action<PlayMakerFSM> onStateEnter)
        {
            if (!StateEnterData.ContainsKey(data))
            {
                StateEnterData.Add(data, onStateEnter);
            }
            else
            {
                StateEnterData[data] += onStateEnter;
            }
        }
        /// <summary>
        /// Hook that gets called when a state is exited by any means (a transition, a global transition, or from Fsm.SetState)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateExit">The action that will be invoked when the state is exited, the parameter passed into the action is the fsm</param>
        public static void HookStateExited(FSMData data, Action<PlayMakerFSM> onStateExit)
        {
            if (!StateExitData.ContainsKey(data))
            {
                StateExitData.Add(data, onStateExit);
            }
            else
            {
                StateExitData[data] += onStateExit;
            }
        }
        /// <summary>
        /// Unhook your action from the hook that gets called when a state is entered by any means (a transition, a global transition, or from Fsm.SetState)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateEnter">The action that will be removed</param>
        public static void UnHookStateEntered(FSMData data, Action<PlayMakerFSM> onStateEnter)
        {
            if (StateEnterData.ContainsKey(data))
            {
                StateEnterData[data] -= onStateEnter;
            }
        }
        /// <summary>
        /// Unhook your action from the hook that gets called when a state is exited by any means (a transition, a global transition, or from Fsm.SetState)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateExit">The action that will be removed</param>
        public static void UnHookStateExited(FSMData data, Action<PlayMakerFSM> onStateExit)
        {
            if (StateExitData.ContainsKey(data))
            {
                StateExitData[data] -= onStateExit;
            }
        }
        /// <summary>
        /// Hook that gets called when a state is entered by a transition (could be global or local). The transition from which it happened is passed into the action.
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateEnteredFromTransition">The action that will be invoked when the state is entered, the parameter passed into the action is the fsm and the transition from which the state enter happened</param>
        public static void HookStateEnteredFromTransition(FSMData data, Action<PlayMakerFSM, string> onStateEnteredFromTransition)
        {
            if (!StateEnteredFromTransitionData.ContainsKey(data))
            {
                StateEnteredFromTransitionData.Add(data, onStateEnteredFromTransition);
            }
            else
            {
                StateEnteredFromTransitionData[data] += onStateEnteredFromTransition;
            }
        }
        /// <summary>
        /// Hook that gets called when a state is exited via a transition (could be global or local). The transition from which it happened is passed into the action.
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateExitViaTransition">The action that will be invoked when the state is exited, the parameter passed into the action is the fsm and the transition from which the state exit happened</param>
        public static void HookStateExitedViaTransition(FSMData data, Action<PlayMakerFSM, string> onStateExitViaTransition)
        {
            if (!StateExitedViaTransitionData.ContainsKey(data))
            {
                StateExitedViaTransitionData.Add(data, onStateExitViaTransition);
            }
            else
            {
                StateExitedViaTransitionData[data] += onStateExitViaTransition;
            }
        }
        /// <summary>
        /// Unhook your action from the hook that gets called when a state is entered by a transition (could be global or local)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateEnteredFromTransition">The action that will be removed</param>
        public static void UnHookStateEnteredFromTransition(FSMData data, Action<PlayMakerFSM, string> onStateEnteredFromTransition)
        {
            if (StateEnteredFromTransitionData.ContainsKey(data))
            {
                StateEnteredFromTransitionData[data] -= onStateEnteredFromTransition;
            }
        }
        /// <summary>
        /// Unhook your action from the hook that gets called when a state is exited via a transition (could be global or local)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateExitViaTransition">The action that will be removed</param>
        public static void UnHookStateExitedViaTransition(FSMData data, Action<PlayMakerFSM, string> onStateExitViaTransition)
        {
            if (StateExitedViaTransitionData.ContainsKey(data))
            {
                StateExitedViaTransitionData[data] -= onStateExitViaTransition;
            }
        }

        /// <summary>
        /// Creates a hook that gets called when a state is entered by any means (a transition, a global transition, or from Fsm.SetState)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateEnter">The action that will be invoked when the state is entered, the parameter passed into the action is the fsm</param>
        /// <returns>Handle of the hook</returns>
        public static FSMHookHandle<Action<PlayMakerFSM>> CreateStateEnteredHook(FSMData data, Action<PlayMakerFSM> onStateEnter) =>
            new(StateEnterData, data, onStateEnter);
        /// <summary>
        /// Creates a hook that gets called when a state is exited by any means (a transition, a global transition, or from Fsm.SetState)
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateExit">The action that will be invoked when the state is exited, the parameter passed into the action is the fsm</param>
        /// <returns>Handle of the hook</returns>
        public static FSMHookHandle<Action<PlayMakerFSM>> CreateStateExitedHook(FSMData data, Action<PlayMakerFSM> onStateExit) =>
            new(StateExitData, data, onStateExit);
        /// <summary>
        /// Creates a hook that gets called when a state is entered by a transition (could be global or local). The transition from which it happened is passed into the action.
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateEnteredFromTransition">The action that will be invoked when the state is entered, the parameter passed into the action is the fsm and the transition from which the state enter happened</param>
        /// <returns>Handle of the hook</returns>
        public static FSMHookHandle<Action<PlayMakerFSM, string>> CreateStateEnteredViaTransitionHook(FSMData data, Action<PlayMakerFSM, string> onStateEnteredFromTransition) =>
            new(StateEnteredFromTransitionData, data, onStateEnteredFromTransition);
        /// <summary>
        /// Creates a hook that gets called when a state is exited via a transition (could be global or local). The transition from which it happened is passed into the action.
        /// </summary>
        /// <param name="data">The data necessary to find the fsm to be edited</param>
        /// <param name="onStateExitViaTransition">The action that will be invoked when the state is exited, the parameter passed into the action is the fsm and the transition from which the state exit happened</param>
        /// <returns>Handle of the hook</returns>
        public static FSMHookHandle<Action<PlayMakerFSM, string>> CreateStateExitedViaTransitionHook(FSMData data, Action<PlayMakerFSM, string> onStateExitViaTransition) =>
            new(StateExitedViaTransitionData, data, onStateExitViaTransition);

        private static string GetSceneName(Fsm self)
        {
            string tmpSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            tmpSceneName = Regex.Replace(tmpSceneName, "_boss_defeated$", "");
            tmpSceneName = Regex.Replace(tmpSceneName, "_boss$", "");
            tmpSceneName = Regex.Replace(tmpSceneName, "_preload$", "");
            return tmpSceneName;
        }

        private static void EnterState(Action<Fsm, FsmState> orig, Fsm self, FsmState state)
        {
            string sceneName = GetSceneName(self);
            string gameObject = self.GameObjectName;
            string fsmName = self.Name;
            string stateName = state.Name;

            if (StateEnterData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, stateName), out var onStateEnter_1))
            {
                onStateEnter_1.TryInvokeActions(self.FsmComponent);
            }
            if (StateEnterData.TryGetValue(new FSMData(gameObject, fsmName, stateName), out var onStateEnter_2))
            {
                onStateEnter_2.TryInvokeActions(self.FsmComponent);
            }
            if (StateEnterData.TryGetValue(new FSMData(fsmName, stateName), out var onStateEnter_3))
            {
                onStateEnter_3.TryInvokeActions(self.FsmComponent);
            }

            orig(self, state);
        }

        private static bool DoTransition(Func<Fsm, FsmTransition, bool, bool> orig, Fsm self, FsmTransition transition, bool isGlobal)
        {
            // a check in the normal code
            if (transition.ToFsmState == null)
            {
                return orig(self, transition, isGlobal);
            }

            string sceneName = GetSceneName(self);
            string gameObject = self.GameObjectName;
            string fsmName = self.Name;

            if (StateExitedViaTransitionData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, self.ActiveStateName), out var onStateExitedViaTransition_1))
            {
                onStateExitedViaTransition_1.TryInvokeActions(self.FsmComponent, transition.EventName);
            }
            if (StateExitedViaTransitionData.TryGetValue(new FSMData(gameObject, fsmName, self.ActiveStateName), out var onStateExitedViaTransition_2))
            {
                onStateExitedViaTransition_2.TryInvokeActions(self.FsmComponent, transition.EventName);
            }
            if (StateExitedViaTransitionData.TryGetValue(new FSMData(fsmName, self.ActiveStateName), out var onStateExitedViaTransition_3))
            {
                onStateExitedViaTransition_3.TryInvokeActions(self.FsmComponent, transition.EventName);
            }

            if (StateEnteredFromTransitionData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, transition.ToState), out var onStateEnteredFromTransition_1))
            {
                onStateEnteredFromTransition_1.TryInvokeActions(self.FsmComponent, transition.EventName);
            }
            if (StateEnteredFromTransitionData.TryGetValue(new FSMData(gameObject, fsmName, transition.ToState), out var onStateEnteredFromTransition_2))
            {
                onStateEnteredFromTransition_2.TryInvokeActions(self.FsmComponent, transition.EventName);
            }
            if (StateEnteredFromTransitionData.TryGetValue(new FSMData(fsmName, transition.ToState), out var onStateEnteredFromTransition_3))
            {
                onStateEnteredFromTransition_3.TryInvokeActions(self.FsmComponent, transition.EventName);
            }

            return orig(self, transition, isGlobal);
        }

        private static void ExitState(Action<Fsm, FsmState> orig, Fsm self, FsmState state)
        {
            string sceneName = GetSceneName(self);
            string gameObject = self.GameObjectName;
            string fsmName = self.Name;
            string stateName = state.Name;

            if (StateExitData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, stateName), out var onStateExit_1))
            {
                onStateExit_1.TryInvokeActions(self.FsmComponent);
            }
            if (StateExitData.TryGetValue(new FSMData(gameObject, fsmName, stateName), out var onStateExit_2))
            {
                onStateExit_2.TryInvokeActions(self.FsmComponent);
            }
            if (StateExitData.TryGetValue(new FSMData(fsmName, stateName), out var onStateExit_3))
            {
                onStateExit_3.TryInvokeActions(self.FsmComponent);
            }

            orig(self, state);
        }

        private static void TryInvokeActions(this Action<PlayMakerFSM> action, PlayMakerFSM fsm)
        {
            if (action != null)
            {
                Delegate[] invocationList = action.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    try
                    {
                        (invocationList[i] as Action<PlayMakerFSM>).Invoke(fsm);
                    }
                    catch (Exception ex)
                    {
                        InternalLogger.LogError($"[Core][FsmUtil][Hooks] - {ex}");
                    }
                }
            }
        }

        private static void TryInvokeActions(this Action<PlayMakerFSM, string> action, PlayMakerFSM fsm, string transition)
        {
            if (action != null)
            {
                Delegate[] invocationList = action.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    try
                    {
                        (invocationList[i] as Action<PlayMakerFSM, string>).Invoke(fsm, transition);
                    }
                    catch (Exception ex)
                    {
                        InternalLogger.LogError($"[Core][FsmUtil][Hooks] - {ex}");
                    }
                }
            }
        }
    }
}
