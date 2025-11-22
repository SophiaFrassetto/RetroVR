using UnityEngine;

namespace retrovr.system
{
    /// <summary>
    /// Represents the physical state of the TV/screen in the VR world.
    /// </summary>
    public enum ScreenPhysicalState
    {
        /// <summary>Screen is loose in the world, free to be moved.</summary>
        Loose = 0,

        /// <summary>Screen is currently being held by the player.</summary>
        Held = 1,

        /// <summary>Screen is placed somewhere (table, floor, stand).</summary>
        Placed = 2,

        /// <summary>Screen is mounted (wall mount or similar).</summary>
        Mounted = 3,

        /// <summary>Screen is linked to a console (via cable or script binding).</summary>
        ConnectedToConsole = 4,

        /// <summary>Screen is physically damaged or invalid.</summary>
        Broken = 100
    }
}
