using UnityEngine;

namespace retrovr.system
{
    /// <summary>
    /// Represents the physical/positional status of the console within the VR world.
    /// This is separate from operational state and is used for interactions and gameplay logic.
    /// </summary>
    public enum ConsolePhysicalState
    {
        /// <summary>Console is loose in the world (free, not placed).</summary>
        Loose = 0,

        /// <summary>Console is currently being held by the player.</summary>
        Held = 1,

        /// <summary>Console is placed in the environment (on a table, floor, etc.).</summary>
        Placed = 2,

        /// <summary>Console is connected to a TV (via cable or linkage).</summary>
        ConnectedToScreen = 3,

        /// <summary>Console is connected to a power source.</summary>
        ConnectedToPower = 4,

        /// <summary>Console is physically damaged or invalid.</summary>
        Broken = 100
    }
}
