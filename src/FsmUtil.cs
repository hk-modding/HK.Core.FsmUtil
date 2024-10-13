using System;
using Core.FsmUtil.Actions;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.FsmUtil;

/// <summary>
///     Utils specifically for PlayMakerFSMs.
/// </summary>
public static class FsmUtil
{
    #region Get a FSM

    /// <summary>
    ///     Locates a PlayMakerFSM by name and preprocesses it.
    /// </summary>
    /// <param name="go">The GameObject to search on</param>
    /// <param name="fsmName">The name of the FSM</param>
    /// <returns>The found FSM, null if not found</returns>
    [PublicAPI]
    private static PlayMakerFSM GetFsmPreprocessed(this GameObject go, string fsmName)
    {
        PlayMakerFSM[] goFsmArr = go.GetComponents<PlayMakerFSM>();
        int goFsmArrCount = goFsmArr.Length;
        int fsmNameCount = fsmName.Length;
        int i;
        for (i = 0; i < goFsmArrCount; i++)
        {
            // length check first because unity's framework472 is trash and doesn't do it itself
            if (goFsmArr[i].FsmName.Length == fsmNameCount && goFsmArr[i].FsmName == fsmName)
            {
                goFsmArr[i].Preprocess();
                return goFsmArr[i];
            }
        }
        return null;
    }

    #endregion
    
    #region Get

    private static TVal GetItemFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
    {
        int origArrayCount = origArray.Length;
        int i;
        for (i = 0; i < origArrayCount; i++)
        {
            if (isItemCheck(origArray[i]))
            {
                return origArray[i];
            }
        }
        return null;
    }
    
    private static TVal[] GetItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
    {
        int origArrayCount = origArray.Length;
        int i;
        int foundItems = 0;
        for (i = 0; i < origArrayCount; i++)
        {
            if (isItemCheck(origArray[i]))
            {
                foundItems++;
            }
        }
        if (foundItems == origArrayCount)
        {
            return origArray;
        }
        else if (foundItems == 0)
        {
            return Array.Empty<TVal>();
        }
        TVal[] retActions = new TVal[foundItems];
        int foundProgress = 0;
        for (i = 0; foundProgress < foundItems; i++)
        {
            var tmpAction = origArray[i];
            if (isItemCheck(tmpAction))
            {
                retActions[foundProgress] = tmpAction;
                foundProgress++;
            }
        }
        return retActions;
    }
    
    /// <summary>
    ///     Gets a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>The found state, null if none are found</returns>
    [PublicAPI]
    public static FsmState GetState(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName);

    /// <inheritdoc cref="GetState(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmState GetFsmState(this PlayMakerFSM fsm, string stateName)
    {
        int stateNameCount = stateName.Length;
        // length check first because unity's framework472 is trash and doesn't do it itself
        return GetItemFromArray(fsm.FsmStates, x => x.Name.Length == stateNameCount && x.Name == stateName);
    }

    /// <summary>
    ///     Gets a transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the from state</param>
    /// <param name="eventName">The name of the event</param>
    /// <returns>The found transition, null if none are found</returns>
    [PublicAPI]
    public static FsmTransition GetTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmState(stateName).GetFsmTransition(eventName);

    /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static FsmTransition GetFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmState(stateName).GetFsmTransition(eventName);

    /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
    /// <param name="state">The state</param>
    /// <param name="eventName">The name of the event</param>
    [PublicAPI]
    public static FsmTransition GetTransition(this FsmState state, string eventName) => state.GetFsmTransition(eventName);

    /// <inheritdoc cref="GetTransition(FsmState, string)"/>
    [PublicAPI]
    public static FsmTransition GetFsmTransition(this FsmState state, string eventName)
    {
        int eventNameCount = eventName.Length;
        // length check first because unity's framework472 is trash and doesn't do it itself
        return GetItemFromArray(state.Transitions, x => x.EventName.Length == eventNameCount && x.EventName == eventName);
    }

    /// <summary>
    ///     Gets an action in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of the action that is wanted</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <param name="index">The index of the action</param>
    /// <returns>The action, null if it can't be found</returns>
    [PublicAPI]
    public static TAction GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetFsmState(stateName).GetFsmAction<TAction>(index);

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static TAction GetFsmAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetFsmState(stateName).GetFsmAction<TAction>(index);

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    /// <param name="state">The state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static TAction GetAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction => state.GetFsmAction<TAction>(index);

    /// <inheritdoc cref="GetAction{TAction}(FsmState, int)"/>
    [PublicAPI]
    public static TAction GetFsmAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction
    {
        return state.Actions[index] as TAction;
    }

    /// <summary>
    ///     Gets an action in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of the action that is wanted</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>An array of actions</returns>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetFsmState(stateName).GetFsmActionsOfType<TAction>();

    /// <inheritdoc cref="GetActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static TAction[] GetFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetFsmState(stateName).GetFsmActionsOfType<TAction>();

    /// <inheritdoc cref="GetActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The state</param>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction => state.GetFsmActionsOfType<TAction>();

    /// <inheritdoc cref="GetActionsOfType{TAction}(FsmState)"/>
    [PublicAPI]
    public static TAction[] GetFsmActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction
    {
        return GetItemsFromArray(state.Actions, x => x is TAction) as TAction[];
    }

    #endregion Get

    #region Add

    private static TVal[] AddItemToArray<TVal>(TVal[] origArray, TVal value)
    {
        TVal[] newArray = new TVal[origArray.Length + 1];
        origArray.CopyTo(newArray, 0);
        newArray[origArray.Length] = value;
        return newArray;
    }
    
    /// <summary>
    ///     Adds a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>The created state</returns>
    [PublicAPI]
    public static FsmState AddState(this PlayMakerFSM fsm, string stateName) => fsm.AddFsmState(new FsmState(fsm.Fsm) { Name = stateName });

    /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmState AddFsmState(this PlayMakerFSM fsm, string stateName) => fsm.AddFsmState(new FsmState(fsm.Fsm) { Name = stateName });

    /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
    /// <param name="fsm">The fsm</param>
    /// <param name="state">The state</param>
    [PublicAPI]
    public static FsmState AddState(this PlayMakerFSM fsm, FsmState state) => fsm.AddFsmState(state);

    /// <inheritdoc cref="AddState(PlayMakerFSM, FsmState)"/>
    [PublicAPI]
    public static FsmState AddFsmState(this PlayMakerFSM fsm, FsmState state)
    {
        FsmState[] origStates = fsm.FsmStates;
        FsmState[] states = AddItemToArray(origStates, state);
        fsm.Fsm.States = states;
        return states[origStates.Length];
    }

    /// <summary>
    ///     Copies a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="fromState">The name of the state to copy</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The new state</returns>
    [PublicAPI]
    public static FsmState CopyState(this PlayMakerFSM fsm, string fromState, string toState) => fsm.CopyFsmState(fromState, toState);

    /// <inheritdoc cref="CopyState(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static FsmState CopyFsmState(this PlayMakerFSM fsm, string fromState, string toState)
    {
        FsmState from = fsm.GetFsmState(fromState);
        // save the actions before we create a new state from this, as the copy constructor will create the new actions from the saved action data from the state we put in, and that is only updated if we call .SaveActions()
        from.SaveActions();
        FsmState copy = new FsmState(from)
        {
            Name = toState
        };
        FsmTransition[] transitions = copy.Transitions;
        int transitionsCount = transitions.Length;
        int i;
        for (i = 0; i < transitionsCount; i++)
        {
            // This is because playmaker is bad, it has to be done extra
            transitions[i].ToFsmState = fsm.GetFsmState(transitions[i].ToState);
        }
        fsm.AddFsmState(copy);
        return copy;
    }

    /// <summary>
    ///     Adds a transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The event of the transition</returns>
    [PublicAPI]
    public static FsmEvent AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).AddFsmTransition(eventName, toState);

    /// <inheritdoc cref="AddTransition(FsmState, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).AddFsmTransition(eventName, toState);

    /// <inheritdoc cref="AddTransition(FsmState, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddTransition(this FsmState state, string eventName, string toState) => state.AddFsmTransition(eventName, toState);

    /// <inheritdoc cref="AddTransition(FsmState, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddFsmTransition(this FsmState state, string eventName, string toState)
    {
        var ret = FsmEvent.GetFsmEvent(eventName);
        FsmTransition[] origTransitions = state.Transitions;
        FsmTransition[] transitions = AddItemToArray(origTransitions, new FsmTransition
        {
            ToState = toState,
            ToFsmState = state.Fsm.GetState(toState),
            FsmEvent = ret
        });
        state.Transitions = transitions;
        return ret;
    }

    /// <summary>
    ///     Adds a global transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="globalEventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The event of the transition</returns>
    [PublicAPI]
    public static FsmEvent AddGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => fsm.AddFsmGlobalTransitions(globalEventName, toState);

    /// <inheritdoc cref="AddGlobalTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddFsmGlobalTransitions(this PlayMakerFSM fsm, string globalEventName, string toState)
    {
        var ret = new FsmEvent(globalEventName) { IsGlobal = true };
        FsmTransition[] origTransitions = fsm.FsmGlobalTransitions;
        FsmTransition[] transitions = AddItemToArray(origTransitions, new FsmTransition
        {
            ToState = toState,
            ToFsmState = fsm.GetFsmState(toState),
            FsmEvent = ret
        });
        fsm.Fsm.GlobalTransitions = transitions;
        return ret;
    }

    /// <summary>
    ///     Adds an action in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="action">The action</param>
    [PublicAPI]
    public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetFsmState(stateName).AddFsmAction(action);

    /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
    [PublicAPI]
    public static void AddFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetFsmState(stateName).AddFsmAction(action);

    /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="action">The action</param>
    [PublicAPI]
    public static void AddAction(this FsmState state, FsmStateAction action) => state.AddFsmAction(action);

    /// <inheritdoc cref="AddAction(FsmState, FsmStateAction)"/>
    [PublicAPI]
    public static void AddFsmAction(this FsmState state, FsmStateAction action)
    {
        FsmStateAction[] actions = AddItemToArray(state.Actions, action);
        state.Actions = actions;
        action.Init(state);
    }

    /// <summary>
    ///     Adds a method in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    [PublicAPI]
    public static void AddMethod(this PlayMakerFSM fsm, string stateName, Action method) => fsm.GetFsmState(stateName).AddFsmMethod(method);

    /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action)"/>
    [PublicAPI]
    public static void AddFsmMethod(this PlayMakerFSM fsm, string stateName, Action method) => fsm.GetFsmState(stateName).AddFsmMethod(method);

    /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    [PublicAPI]
    public static void AddMethod(this FsmState state, Action method) => state.AddFsmMethod(method);

    /// <inheritdoc cref="AddMethod(FsmState, Action)"/>
    [PublicAPI]
    public static void AddFsmMethod(this FsmState state, Action method)
    {
        state.AddFsmAction(new MethodAction { Method = method });
    }

    /// <summary>
    ///     Adds a method with a parameter in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TArg">The type of the parameter of the function</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    [PublicAPI]
    public static void AddMethod<TArg>(this PlayMakerFSM fsm, string stateName, Action<TArg> method, TArg arg) => fsm.GetFsmState(stateName).AddFsmMethod(method, arg);

    /// <inheritdoc cref="AddMethod{TArg}(PlayMakerFSM, string, Action{TArg}, TArg)"/>
    [PublicAPI]
    public static void AddFsmMethod<TArg>(this PlayMakerFSM fsm, string stateName, Action<TArg> method, TArg arg) => fsm.GetFsmState(stateName).AddFsmMethod(method, arg);

    /// <inheritdoc cref="AddMethod{TArg}(PlayMakerFSM, string, Action{TArg}, TArg)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    [PublicAPI]
    public static void AddMethod<TArg>(this FsmState state, Action<TArg> method, TArg arg) => state.AddFsmMethod(method, arg);

    /// <inheritdoc cref="AddMethod{TArg}(FsmState, Action{TArg}, TArg)"/>
    [PublicAPI]
    public static void AddFsmMethod<TArg>(this FsmState state, Action<TArg> method, TArg arg)
    {
        state.AddFsmAction(new FunctionAction<TArg> { Method = method, Arg = arg });
    }

    #endregion Add

    #region Insert

    private static TVal[] InsertItemIntoArray<TVal>(TVal[] origArray, TVal value, int index)
    {
        int origArrayCount = origArray.Length;
        if (index < 0 || index > (origArrayCount + 1))
        {
            throw new ArgumentOutOfRangeException($"Index {index} was out of range for array with length {origArrayCount}!");
        }
        TVal[] actions = new TVal[origArrayCount + 1];
        int i;
        for (i = 0; i < index; i++)
        {
            actions[i] = origArray[i];
        }
        actions[index] = value;
        for (i = index; i < origArrayCount; i++)
        {
            actions[i + 1] = origArray[i];
        }
        return actions;
    }
    
    /// <summary>
    ///     Inserts an action in a PlayMakerFSM.  
    ///     Trying to insert an action out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetFsmState(stateName).InsertFsmAction(action, index);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetFsmState(stateName).InsertFsmAction(action, index);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertAction(this FsmState state, FsmStateAction action, int index) => state.InsertFsmAction(action, index);

    /// <inheritdoc cref="InsertAction(FsmState, FsmStateAction, int)"/>
    [PublicAPI]
    public static void InsertFsmAction(this FsmState state, FsmStateAction action, int index)
    {
        FsmStateAction[] actions = InsertItemIntoArray(state.Actions, action, index);
        state.Actions = actions;
        action.Init(state);
    }

    /// <summary>
    ///     Inserts a method in a PlayMakerFSM.
    ///     Trying to insert a method out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertMethod(this PlayMakerFSM fsm, string stateName, Action method, int index) => fsm.GetFsmState(stateName).InsertFsmMethod(method, index);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action, int)"/>
    [PublicAPI]
    public static void InsertFsmMethod(this PlayMakerFSM fsm, string stateName, Action method, int index) => fsm.GetFsmState(stateName).InsertFsmMethod(method, index);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertMethod(this FsmState state, Action method, int index) => state.InsertFsmMethod(method, index);

    /// <inheritdoc cref="InsertMethod(FsmState, Action, int)"/>
    [PublicAPI]
    public static void InsertFsmMethod(this FsmState state, Action method, int index) => state.InsertFsmAction(new MethodAction { Method = method }, index);

    /// <summary>
    ///     Inserts a method with a parameter in a PlayMakerFSM.
    ///     Trying to insert a method out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <typeparam name="TArg">The type of the parameter of the function</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertMethod<TArg>(this PlayMakerFSM fsm, string stateName, Action<TArg> method, TArg arg, int index) => fsm.GetFsmState(stateName).InsertFsmMethod(method, arg, index);

    /// <inheritdoc cref="InsertMethod{TArg}(PlayMakerFSM, string, Action{TArg}, TArg, int)"/>
    [PublicAPI]
    public static void InsertFsmMethod<TArg>(this PlayMakerFSM fsm, string stateName, Action<TArg> method, TArg arg, int index) => fsm.GetFsmState(stateName).InsertFsmMethod(method, arg, index);

    /// <inheritdoc cref="InsertMethod{TArg}(PlayMakerFSM, string, Action{TArg}, TArg, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertMethod<TArg>(this FsmState state, Action<TArg> method, TArg arg, int index) => state.InsertFsmMethod(method, arg, index);

    /// <inheritdoc cref="InsertMethod{TArg}(FsmState, Action{TArg}, TArg, int)"/>
    [PublicAPI]
    public static void InsertFsmMethod<TArg>(this FsmState state, Action<TArg> method, TArg arg, int index) => state.InsertFsmAction(new FunctionAction<TArg> { Method = method, Arg = arg }, index);

    #endregion Insert

    #region Replace
    
    /// <summary>
    ///     Replaces an action in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is replaced</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetFsmState(stateName).ReplaceFsmAction(action, index);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void ReplaceFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetFsmState(stateName).ReplaceFsmAction(action, index);

    /// <summary>
    ///     Replaces an action in a PlayMakerFSM state.
    /// </summary>
    /// <param name="state">The state in which the action is replaced</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static void ReplaceAction(this FsmState state, FsmStateAction action, int index) => state.ReplaceFsmAction(action, index);

    /// <inheritdoc cref="ReplaceAction(FsmState, FsmStateAction, int)"/>
    [PublicAPI]
    public static void ReplaceFsmAction(this FsmState state, FsmStateAction action, int index)
    {
        state.Actions[index] = action;
        action.Init(state);
    }

    /// <summary>
    ///     Replaces all actions in a PlayMakerFSM state.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the actions are to be replaced</param>
    /// <param name="actions">The new actions of the state</param>
    [PublicAPI] public static void ReplaceAllActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => fsm.GetFsmState(stateName).ReplaceAllFsmActions(actions);

    /// <inheritdoc cref="ReplaceAllActions(PlayMakerFSM, string, FsmStateAction[])"/>
    [PublicAPI]
    public static void ReplaceAllFsmActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => fsm.GetFsmState(stateName).ReplaceAllFsmActions(actions);

    /// <summary>
    ///     Replaces all actions in a PlayMakerFSM state.
    /// </summary>
    /// <param name="state">The fsm state</param>
    /// <param name="actions">The action</param>
    [PublicAPI]
    public static void ReplaceAllActions(this FsmState state, params FsmStateAction[] actions) => state.ReplaceAllFsmActions(actions);

    /// <inheritdoc cref="ReplaceAllActions(FsmState, FsmStateAction[])"/>
    [PublicAPI]
    public static void ReplaceAllFsmActions(this FsmState state, params FsmStateAction[] actions)
    {
        state.Actions = actions;
        int i;
        for (i = 0; i < actions.Length; i++)
        {
            actions[i].Init(state);
        }
    }

    #endregion Replace

    #region Change

    /// <summary>
    ///     Changes a transition endpoint in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The event of the transition</param>
    /// <param name="toState">The new endpoint of the transition</param>
    /// <returns>bool that indicates whether the change was successful</returns>
    [PublicAPI]
    public static bool ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.ChangeFsmTransition(stateName, eventName, toState);

    /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
    [PublicAPI]
    public static bool ChangeFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).ChangeFsmTransition(eventName, toState);

    /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="eventName">The event of the transition</param>
    /// <param name="toState">The new endpoint of the transition</param>
    [PublicAPI]
    public static bool ChangeTransition(this FsmState state, string eventName, string toState) => state.ChangeFsmTransition(eventName, toState);

    /// <inheritdoc cref="ChangeTransition(FsmState, string, string)"/>
    [PublicAPI]
    public static bool ChangeFsmTransition(this FsmState state, string eventName, string toState)
    {
        var transition = state.GetFsmTransition(eventName);
        if (transition == null)
        {
            return false;
        }
        transition.ToState = toState;
        transition.ToFsmState = state.Fsm.GetState(toState);
        return true;
    }

    #endregion Change

    #region Remove

    private static TVal[] RemoveItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> shouldBeRemovedCallback)
    {
        int origArrayCount = origArray.Length;
        int i;
        int amountOfRemoved = 0;
        for (i = 0; i < origArrayCount; i++)
        {
            if (shouldBeRemovedCallback(origArray[i]))
            {
                amountOfRemoved++;
            }
        }
        if (amountOfRemoved == 0)
        {
            return origArray;
        }
        TVal[] newArray = new TVal[origArray.Length - amountOfRemoved];
        for (i = origArrayCount - 1; i >= 0; i--)
        {
            TVal tmpValue = origArray[i];
            if (shouldBeRemovedCallback(tmpValue))
            {
                amountOfRemoved--;
                continue;
            }
            newArray[i - amountOfRemoved] = tmpValue;
        }
        return newArray;
    }

    /// <summary>
    ///     Removes a state in a PlayMakerFSM.  
    ///     Trying to remove a state that doesn't exist will result in the states not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove</param>
    [PublicAPI]
    public static void RemoveState(this PlayMakerFSM fsm, string stateName) => fsm.RemoveFsmState(stateName);

    /// <inheritdoc cref="RemoveState(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveFsmState(this PlayMakerFSM fsm, string stateName)
    {
        int stateNameCount = stateName.Length;
        // length check first because unity's framework472 is trash and doesn't do it itself
        fsm.Fsm.States = RemoveItemsFromArray(fsm.FsmStates, x => x.Name.Length == stateNameCount && x.Name == stateName);
    }

    /// <summary>
    ///     Removes a transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmState(stateName).RemoveFsmTransition(eventName);

    /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static void RemoveFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmState(stateName).RemoveFsmTransition(eventName);

    /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="eventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransition(this FsmState state, string eventName) => state.RemoveFsmTransition(eventName);

    /// <inheritdoc cref="RemoveTransition(FsmState, string)"/>
    [PublicAPI]
    public static void RemoveFsmTransition(this FsmState state, string eventName)
    {
        int eventNameCount = eventName.Length;
        // length check first because unity's framework472 is trash and doesn't do it itself
        state.Transitions = RemoveItemsFromArray(state.Transitions, x => x.EventName.Length == eventNameCount && x.EventName == eventName);
    }

    /// <summary>
    ///     Removes all transitions to a specified transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="toState">The target of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string toState) => fsm.RemoveFsmTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveFsmTransitionsTo(this PlayMakerFSM fsm, string toState)
    {
        FsmState[] origStates = fsm.FsmStates;
        int origStatesCount = origStates.Length;
        int i;
        for (i = 0; i < origStatesCount; i++)
        {
            origStates[i].RemoveFsmTransitionsTo(toState);
        }
    }

    /// <summary>
    ///     Removes all transitions from a state to another specified state in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="toState">The target of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState) => fsm.GetFsmState(stateName).RemoveFsmTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static void RemoveFsmTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState) => fsm.GetFsmState(stateName).RemoveFsmTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="toState">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this FsmState state, string toState) => state.RemoveFsmTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(FsmState, string)"/>
    [PublicAPI]
    public static void RemoveFsmTransitionsTo(this FsmState state, string toState)
    {
        int toStateCount = toState.Length;
        // length check first because unity's framework472 is trash and doesn't do it itself
        state.Transitions = RemoveItemsFromArray(state.Transitions, x => x.ToState.Length == toStateCount && x.ToState == toState);
    }

    /// <summary>
    ///     Removes all transitions from a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    [PublicAPI]
    public static void RemoveTransitions(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName).RemoveFsmTransitions();

    /// <inheritdoc cref="RemoveTransitions(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveFsmTransitions(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName).RemoveFsmTransitions();

    /// <inheritdoc cref="RemoveTransitions(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveTransitions(this FsmState state) => state.RemoveFsmTransitions();

    /// <inheritdoc cref="RemoveTransitions(FsmState)"/>
    [PublicAPI]
    public static void RemoveFsmTransitions(this FsmState state)
    {
        state.Transitions = Array.Empty<FsmTransition>();
    }

    /// <summary>
    ///     Removes an action in a PlayMakerFSM.  
    ///     Trying to remove an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool RemoveAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetFsmState(stateName).RemoveFsmAction(index);

    /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static bool RemoveFsmAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetFsmState(stateName).RemoveFsmAction(index);

    /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool RemoveAction(this FsmState state, int index) => state.RemoveFsmAction(index);

    /// <inheritdoc cref="RemoveAction(FsmState, int)"/>
    [PublicAPI]
    public static bool RemoveFsmAction(this FsmState state, int index)
    {
        FsmStateAction[] origActions = state.Actions;
        if (index < 0 || index >= origActions.Length)
        {
            return false;
        }
        FsmStateAction[] newActions = new FsmStateAction[origActions.Length - 1];
        int newActionsCount = newActions.Length;
        int i;
        for (i = 0; i < index; i++)
        {
            newActions[i] = origActions[i];
        }
        for (i = index; i < newActionsCount; i++)
        {
            newActions[i] = origActions[i + 1];
        }

        state.Actions = newActions;
        return true;
    }

    /// <summary>
    ///     Removes all actions of a given type in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm) => fsm.RemoveFsmActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM)"/>
    [PublicAPI]
    public static void RemoveFsmActionsOfType<TAction>(this PlayMakerFSM fsm)
    {
        FsmState[] origStates = fsm.FsmStates;
        int origStatesCount = origStates.Length;
        int i;
        for (i = 0; i < origStatesCount; i++)
        {
            origStates[i].RemoveFsmActionsOfType<TAction>();
        }
    }


    /// <summary>
    ///     Removes all actions of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName).RemoveFsmActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName).RemoveFsmActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this FsmState state) => state.RemoveFsmActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(FsmState)"/>
    [PublicAPI]
    public static void RemoveFsmActionsOfType<TAction>(this FsmState state)
    {
        state.Actions = RemoveItemsFromArray(state.Actions, x => x is TAction);
    }

    #endregion Remove

    #region Disable
    /// <summary>
    ///     Disables an action in a PlayMakerFSM.  
    ///     Trying to disable an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool DisableAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetFsmState(stateName).DisableFsmAction(index);

    /// <inheritdoc cref="DisableAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static bool DisableFsmAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetFsmState(stateName).DisableFsmAction(index);

    /// <inheritdoc cref="DisableAction(PlayMakerFSM, string, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool DisableAction(this FsmState state, int index) => state.DisableFsmAction(index);

    /// <inheritdoc cref="DisableAction(FsmState, int)"/>
    [PublicAPI]
    public static bool DisableFsmAction(this FsmState state, int index)
    {
        FsmStateAction[] origActions = state.Actions;
        if (index < 0 || index >= origActions.Length)
        {
            return false;
        }
        origActions[index].Enabled = false;
        return true;
    }

    /// <summary>
    ///     Disables all actions of a given type in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of actions to disable</typeparam>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm) => fsm.DisableFsmActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM)"/>
    [PublicAPI]
    public static void DisableFsmActionsOfType<TAction>(this PlayMakerFSM fsm)
    {
        FsmState[] origStates = fsm.FsmStates;
        int origStatesCount = origStates.Length;
        int i;
        for (i = 0; i < origStatesCount; i++)
        {
            origStates[i].DisableFsmActionsOfType<TAction>();
        }
    }

    /// <summary>
    ///     Disables all actions of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to disable</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to disable the actions from</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName).DisableFsmActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void DisableFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName).DisableFsmActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this FsmState state) => state.DisableFsmActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(FsmState)"/>
    [PublicAPI]
    public static void DisableFsmActionsOfType<TAction>(this FsmState state)
    {
        FsmStateAction[] origArray = state.Actions;
        int origArrayCount = origArray.Length;
        int i;
        for (i = 0; i < origArrayCount; i++)
        {
            if (origArray[i] is TAction)
            {
                origArray[i].Enabled = false;
            }
        }
    }

    #endregion Disable

    #region FSM Variables

    private static TVar[] MakeNewVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
    {
        TVar[] newArray = new TVar[orig.Length + 1];
        orig.CopyTo(newArray, 0);
        newArray[orig.Length] = new TVar { Name = name };
        return newArray;
    }

    /// <summary>
    ///     Adds a fsm variable in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the new variable</param>
    /// <returns>The newly created variable</returns>
    [PublicAPI]
    public static FsmFloat AddFloatVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmFloatVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmFloat AddFsmFloatVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.FloatVariables, name);
        fsm.FsmVariables.FloatVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt AddIntVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmIntVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt AddFsmIntVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.IntVariables, name);
        fsm.FsmVariables.IntVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool AddBoolVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmBoolVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool AddFsmBoolVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.BoolVariables, name);
        fsm.FsmVariables.BoolVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString AddStringVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmStringVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString AddFsmStringVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.StringVariables, name);
        fsm.FsmVariables.StringVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 AddVector2Variable(this PlayMakerFSM fsm, string name) => fsm.AddFsmVector2Variable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 AddFsmVector2Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.Vector2Variables, name);
        fsm.FsmVariables.Vector2Variables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 AddVector3Variable(this PlayMakerFSM fsm, string name) => fsm.AddFsmVector3Variable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 AddFsmVector3Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.Vector3Variables, name);
        fsm.FsmVariables.Vector3Variables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor AddColorVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmColorVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor AddFsmColorVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.ColorVariables, name);
        fsm.FsmVariables.ColorVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect AddRectVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmRectVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect AddFsmRectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.RectVariables, name);
        fsm.FsmVariables.RectVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion AddQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmQuaternionVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion AddFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.QuaternionVariables, name);
        fsm.FsmVariables.QuaternionVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject AddGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmGameObjectVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject AddFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.GameObjectVariables, name);
        fsm.FsmVariables.GameObjectVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    private static TVar FindInVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
    {
        int count = orig.Length;
        int nameLength = name.Length;
        int i;
        for (i = 0; i < count; i++)
        {
            // length check first because unity's framework472 is trash and doesn't do it itself
            if (orig[i].Name.Length == nameLength && orig[i].Name == name)
            {
                return orig[i];
            }
        }
        return null;
    }

    /// <summary>
    ///     Finds a fsm variable in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the variable</param>
    /// <returns>The variable, null if not found</returns>
    [PublicAPI]
    public static FsmFloat FindFloatVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmFloatVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmFloat FindFsmFloatVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.FloatVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt FindIntVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmIntVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt FindFsmIntVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.IntVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool FindBoolVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmBoolVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool FindFsmBoolVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.BoolVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString FindStringVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmStringVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString FindFsmStringVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.StringVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 FindVector2Variable(this PlayMakerFSM fsm, string name) => fsm.FindFsmVector2Variable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 FindFsmVector2Variable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.Vector2Variables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 FindVector3Variable(this PlayMakerFSM fsm, string name) => fsm.FindFsmVector3Variable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 FindFsmVector3Variable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.Vector3Variables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor FindColorVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmColorVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor FindFsmColorVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.ColorVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect FindRectVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmRectVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect FindFsmRectVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.RectVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion FindQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmQuaternionVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion FindFsmQuaternionVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.QuaternionVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject FindGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmGameObjectVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject FindFsmGameObjectVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.GameObjectVariables, name);

    /// <summary>
    ///     Gets a fsm variable in a PlayMakerFSM. Creates a new one if none with the name are present.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the variable</param>
    /// <returns>The variable</returns>
    [PublicAPI]
    public static FsmFloat GetFloatVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmFloatVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmFloat GetFsmFloatVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmFloatVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmFloatVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt GetIntVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmIntVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt GetFsmIntVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmIntVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmIntVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool GetBoolVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmBoolVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool GetFsmBoolVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmBoolVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmBoolVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString GetStringVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmStringVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString GetFsmStringVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmStringVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmStringVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 GetVector2Variable(this PlayMakerFSM fsm, string name) => fsm.GetFsmVector2Variable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 GetFsmVector2Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmVector2Variable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmVector2Variable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 GetVector3Variable(this PlayMakerFSM fsm, string name) => fsm.GetFsmVector3Variable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 GetFsmVector3Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmVector3Variable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmVector3Variable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor GetColorVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmColorVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor GetFsmColorVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmColorVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmColorVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect GetRectVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmRectVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect GetFsmRectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmRectVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmRectVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion GetQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmQuaternionVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion GetFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmQuaternionVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmQuaternionVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    public static FsmGameObject GetGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmGameObjectVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject GetFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFsmGameObjectVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFsmGameObjectVariable(name);
    }

    #endregion FSM Variables

    #region Log

    /// <summary>
    ///     Adds actions to a PlayMakerFSM so it gives a log message before and after every single normal action.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="additionalLogging">Flag if, additionally, every log should also log the state of all fsm variables</param>
    [PublicAPI]
    public static void MakeLog(this PlayMakerFSM fsm, bool additionalLogging = false)
    {
        int i;
        int j;
        foreach (var s in fsm.FsmStates)
        {
            for (i = s.Actions.Length - 1; i >= 0; i--)
            {
                fsm.InsertFsmAction(s.Name, new LogAction { Text = $"{i}" }, i);
                if (additionalLogging)
                {
                    fsm.InsertFsmMethod(s.Name, () =>
                    {
                        var fsmVars = fsm.FsmVariables;
                        var origFloatVariables = fsmVars.FloatVariables;
                        var origIntVariables = fsmVars.IntVariables;
                        var origBoolVariables = fsmVars.BoolVariables;
                        var origStringVariables = fsmVars.StringVariables;
                        var origVector2Variables = fsmVars.Vector2Variables;
                        var origVector3Variables = fsmVars.Vector3Variables;
                        var origColorVariables = fsmVars.ColorVariables;
                        var origRectVariables = fsmVars.RectVariables;
                        var origQuaternionVariables = fsmVars.QuaternionVariables;
                        var origGameObjectVariables = fsmVars.GameObjectVariables;
                        for (j = 0; j < origFloatVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[FloatVariables] - '{origFloatVariables[j].Name}': '{origFloatVariables[j].Value}'");
                        for (j = 0; j < origIntVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[IntVariables] - '{origIntVariables[j].Name}': '{origIntVariables[j].Value}'");
                        for (j = 0; j < origBoolVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[BoolVariables] - '{origBoolVariables[j].Name}': '{origBoolVariables[j].Value}'");
                        for (j = 0; j < origStringVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[StringVariables] - '{origStringVariables[j].Name}': '{origStringVariables[j].Value}'");
                        for (j = 0; j < origVector2Variables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[Vector2Variables] - '{origVector2Variables[j].Name}': '({origVector2Variables[j].Value.x}, {origVector2Variables[j].Value.y})'");
                        for (j = 0; j < origVector3Variables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[Vector3Variables] - '{origVector3Variables[j].Name}': '({origVector3Variables[j].Value.x}, {origVector3Variables[j].Value.y}, {origVector3Variables[j].Value.z})'");
                        for (j = 0; j < origColorVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[ColorVariables] - '{origColorVariables[j].Name}': '{origColorVariables[j].Value}'");
                        for (j = 0; j < origRectVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[RectVariables] - '{origRectVariables[j].Name}': '{origRectVariables[j].Value}'");
                        for (j = 0; j < origQuaternionVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[QuaternionVariables] - '{origQuaternionVariables[j].Name}': '{origQuaternionVariables[j].Value}'");
                        for (j = 0; j < origGameObjectVariables.Length; j++)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[GameObjectVariables] - '{origGameObjectVariables[j].Name}': '{origGameObjectVariables[j].Value}'");
                    }, i + 1);
                }
            }
        }
    }

    /// <summary>
    ///     Logs the fsm and its states and transitions.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void Log(this PlayMakerFSM fsm)
    {
        Log($"FSM \"{fsm.name}\"");
        Log($"{fsm.FsmStates.Length} States");
        foreach (var s in fsm.FsmStates)
        {
            Log($"\tState \"{s.Name}\"");
            foreach (var t in s.Transitions)
            {
                Log($"\t\t-> \"{t.ToState}\" via \"{t.EventName}\"");
            }
        }
        Log($"{fsm.FsmGlobalTransitions.Length} Global Transitions");
        foreach (var t in fsm.FsmGlobalTransitions)
        {
            Log($"\tGlobal Transition \"{t.EventName}\" to \"{t.ToState}\"");
        }
    }

    private static void Log(string msg)
    {
        InternalLogger.Log($"[Core]:[FsmUtil]:[FsmUtil] - {msg}");
    }

    #endregion Log
}