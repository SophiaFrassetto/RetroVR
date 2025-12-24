using UnityEngine;

namespace retrovr.system
{
    [CreateAssetMenu(fileName = "NewCartridge", menuName = "RetroVR/Cartridge")]
    public class CartridgeDefinition : ScriptableObject
    {
        public string romName;
        public string romSubfolder;
        public string overrideCoreName;
        public string extension;

    }
}
