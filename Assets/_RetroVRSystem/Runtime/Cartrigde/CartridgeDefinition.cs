using UnityEngine;

namespace retrovr.system
{
    [CreateAssetMenu(fileName = "NewCartridge", menuName = "RetroVR/Cartridge")]
    public class CartridgeDefinition : ScriptableObject
    {
        public string romName;
        public string romsDirectory;
        public string savesDirectory;
        public string overrideCoreName;
        public string extension;

    }
}
