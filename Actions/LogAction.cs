using HutongGames.PlayMaker;
using UnityEngine;

namespace Core.FsmUtil.Actions
{
    /// <summary>
    ///     FsmStateAction that logs the value of an FsmString and gives context of which GameObject with which Fsm in which State produces the log.
    ///     Output:
    ///     `[{GameObject path}]:[{FSM name}]:[{FSM State name}] - {message}`
    /// </summary>
    public class LogAction : SimpleLogAction
    {
        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (text.Value != null) Log($"{text.Value}");
            Finish();
        }

        private new void Log(string message)
        {
            string path = Fsm.GameObjectName;
            Transform t = Fsm.GameObject.transform.parent;
            while (t != null)
            {
                path = $"{t.gameObject.name}/{path}";
                t = t.parent;
            }
            Logger.Log($"[{path}]:[{Fsm.Name}]:[{State.Name}] - {message}");
        }
    }
}