namespace Core.FsmUtil
{
    internal static class Logger
    {
        internal static void LogFine(string message)
        {
            Modding.Logger.LogFine(message);
            UnityEngine.Debug.Log(message);
        }
        internal static void LogDebug(string message)
        {
            Modding.Logger.LogDebug(message);
            UnityEngine.Debug.Log(message);
        }
        internal static void Log(string message)
        {
            Modding.Logger.Log(message);
            UnityEngine.Debug.Log(message);
        }
        internal static void LogWarn(string message)
        {
            Modding.Logger.LogWarn(message);
            UnityEngine.Debug.LogWarning(message);
        }
        internal static void LogError(string message)
        {
            Modding.Logger.LogError(message);
            UnityEngine.Debug.LogError(message);
        }
    }
}