namespace Core.FsmUtil
{
    /// <summary>
    /// Provides data necessary to find the FSM you want to edit. Use this overload when you want to edit and fsm on a gameObject only in a specific scene
    /// </summary>
    /// <param name="SceneName">The scene name where you want the edit to happen</param>
    /// <param name="GameObjectName">The name of the gameObject that the fsm resides in</param>
    /// <param name="FsmName">The name of the FSM you want to edit</param>
    /// <param name="StateName">The name of the state</param>
    public record FSMData(string SceneName, string GameObjectName, string FsmName, string StateName)
    {
        /// <summary>
        /// Provides data necessary to find the FSM you want to edit. Use this overload when you want to edit and fsm on a gameObject in all scenes
        /// </summary>
        /// <param name="GameObjectName">The name of the gameobject that the fsm resides in</param>
        /// <param name="FsmName">The name of the FSM you want to edit</param>
        /// <param name="StateName">The name of the state</param>
        public FSMData(string GameObjectName, string FsmName, string StateName) : this(null, GameObjectName, FsmName, StateName) { }
        /// <summary>
        /// Provides data necessary to find the FSM you want to edit. Use this overload when you want to edit and an fsm on all gameObjects and all scenes
        /// </summary>
        /// <param name="FsmName">The name of the FSM you want to edit</param>
        /// <param name="StateName">The name of the state</param>
        public FSMData(string FsmName, string StateName) : this(null, null, FsmName, StateName) { }
    }
}