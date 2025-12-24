using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace retrovr.system.debug
{
    public class WorldDebugLog : MonoBehaviour
    {
        public Text logText;
        public ScrollRect scrollRect;
        public int maxLines = 200;

        private readonly Queue<string> lines = new Queue<string>();

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string message, string stackTrace, LogType type)
        {
            string prefix = type switch
            {
                LogType.Warning => "<color=yellow>[WARN]</color> ",
                LogType.Error or LogType.Exception => "<color=red>[ERROR]</color> ",
                _ => ""
            };

            string line = prefix + message;

            lines.Enqueue(line);

            while (lines.Count > maxLines)
                lines.Dequeue();

            logText.text = string.Join("\n", lines);

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
