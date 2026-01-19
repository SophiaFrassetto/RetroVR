using System;
using UnityEngine;
using TMPro;
using retrovr.system.physics;

namespace retrovr.system
{
    public class CartridgeInstance : MonoBehaviour
    {
        #region Fields
        [Header("Cartridge Configuration")]
        [SerializeField] public CartridgeDefinition cartridgeDefinition;
        [SerializeField] private TMP_Text displayRomName;

        [Header("State")]
        [SerializeField] private CartridgeOperationalState operationalState = CartridgeOperationalState.Idle;
        [SerializeField] private CartridgePhysicalState physicalState = CartridgePhysicalState.Loose;

        public event Action<CartridgeOperationalState> OnOperationalStateChanged;
        public event Action<CartridgePhysicalState> OnPhysicalStateChanged;

        public CartridgeOperationalState CurrentOperationalState => operationalState;
        public CartridgePhysicalState CurrentPhysicalState => physicalState;
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

        #region State Management
        public void SetOperationalState(CartridgeOperationalState newState)
        {
            if (operationalState == newState) return;
            operationalState = newState;
            Log.Info($"[CartridgeInstance] OperationalState -> {operationalState}");
            OnOperationalStateChanged?.Invoke(newState);
        }

        public void SetPhysicalState(CartridgePhysicalState newState)
        {
            if (physicalState == newState) return;
            physicalState = newState;

            var physical = GetComponent<XRPhysicalObject>();

            switch (newState)
            {
                case CartridgePhysicalState.Held:
                    physical?.OnGrabbed();
                    break;

                case CartridgePhysicalState.Loose:
                    physical?.OnReleased();
                    break;

                case CartridgePhysicalState.Inserted:
                    physical?.ForceKinematic();
                    break;
            }

            OnPhysicalStateChanged?.Invoke(newState);
        }
        #endregion
    }

}
