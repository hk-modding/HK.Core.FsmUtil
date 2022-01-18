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
        public FsmString Text;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            Text = string.Empty;
            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (Text.Value != null) Log($"FSM Log: \"{Text.Value}\"");
            Finish();
        }

        private new static void Log(string message)
        {
            Logger.Log($"[Core]:[FsmUtil]:[LogAction] - {message}");
        }
    }
}
