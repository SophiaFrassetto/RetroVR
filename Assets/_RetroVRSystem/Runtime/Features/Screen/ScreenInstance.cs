using UnityEngine;

namespace retrovr.system
{
    public class ScreenInstance : MonoBehaviour
    {
        #region Fields
        [Header("Screen Configuration")]
        [SerializeField] public string screenName;
        [SerializeField] public Renderer screenRenderer;
        [SerializeField] public Collider screenCollider;
        #endregion

        #region Screen Power Management
        #endregion

        #region Screen Volume Management
        #endregion
    }
}
