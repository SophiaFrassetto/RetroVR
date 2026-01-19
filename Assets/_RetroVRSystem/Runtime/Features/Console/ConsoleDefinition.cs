using System.Collections.Generic;
using UnityEngine;

namespace retrovr.system
{
    [CreateAssetMenu(fileName = "NewConsole", menuName = "RetroVR/Console")]
    public class ConsoleDefinition : ScriptableObject
    {
        public string consoleName;
        public string coreName;
        public List<string> supportedExtensions;
        public bool isPortable = false;
    }
}
