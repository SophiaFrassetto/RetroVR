using UnityEngine;
using TMPro;

namespace retrovr.system
{
    public class CartridgeInstance : MonoBehaviour
    {
        #region Fields
        [Header("Cartridge Configuration")]
        [SerializeField] public CartridgeDefinition cartridgeDefinition;
        [SerializeField] private TMP_Text displayRomName;
        #endregion

        #region execution
        private void Awake()
        {
            SetDisplayName(cartridgeDefinition.romName);
        }
        #endregion

        #region Cartridge Management
        public void SetDisplayName(string name)
        {
            if (displayRomName != null)
            {
                displayRomName.text = name;
            }
            else
            {
                Log.Warn("Display text component is not set.");
            }
        }
        #endregion
    }

}
