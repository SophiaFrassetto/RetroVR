using UnityEngine;

namespace retrovr.system
{
    public class Log : MonoBehaviour
    {
        public enum Level { Info, Warning, Error, VR }

        public static bool enableInfo = true;
        public static bool enableWarnings = true;
        public static bool enableErrors = true;
        public static bool enableVRLogs = true;
        public static bool onlyInPlayMode = false;

        private static bool CanLog()
        {
            #if UNITY_EDITOR
            if (onlyInPlayMode && !Application.isPlaying)
                return false;
            #endif
            return true;
        }

        public static void Info(string msg)
        {
            if (enableInfo && CanLog())
                Debug.Log($"<color=green>[INFO]</color> {msg}");
        }

        public static void Warn(string msg)
        {
            if (enableWarnings && CanLog())
                Debug.LogWarning($"<color=yellow>[WARN]</color> {msg}");
        }

        public static void Error(string msg)
        {
            if (enableErrors && CanLog())
                Debug.LogError($"<color=red>[ERROR]</color> {msg}");
        }

        public static void VR(string msg)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableVRLogs && CanLog())
                Debug.Log($"<color=cyan>[VR]</color> {msg}");
            #endif
        }
    }
}
