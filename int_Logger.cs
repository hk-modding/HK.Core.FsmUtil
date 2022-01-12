using MDebug = Modding.Logger;
using UDebug = UnityEngine.Debug;

namespace Core.FsmUtil
{
    internal static class Logger
    {
        internal static void Log(string message)
        {
            UDebug.Log(message);
            MDebug.LogDebug(message);
        }
    }
}