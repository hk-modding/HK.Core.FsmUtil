using HutongGames.PlayMaker;

namespace Core.FsmUtil.Actions
{
    /// <summary>
    ///     FsmStateAction that logs the value of an FsmString.
    ///     Output:
    ///     `[Core]:[FsmUtil]:[LogAction] - {message}`
    /// </summary>
    public class SimpleLogAction : FsmStateAction
    {
        /// <summary>
        ///     The text to log.
        /// </summary>
        public FsmString text;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            text = string.Empty;
            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (text.Value != null) Log($"FSM Log: \"{text.Value}\"");
            Finish();
        }

        private new void Log(string message)
        {
            Logger.Log($"[Core]:[FsmUtil]:[LogAction] - {message}");
        }
    }
}