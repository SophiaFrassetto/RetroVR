using UnityEngine;

namespace retrovr.system
{
    /// <summary>
    /// Represents how the cartridge physically exists in the environment:
    /// loose, held, inserted into a slot, etc.
    /// </summary>
    public enum CartridgePhysicalState
    {
        /// <summary>Cartridge is free in the world.</summary>
        Loose = 0,

        /// <summary>Cartridge is being held by the player.</summary>
        Held = 1,

        /// <summary>Cartridge is physically inserted into a console slot.</summary>
        Inserted = 2,

        /// <summary>Cartridge is placed in an inventory area or shelf.</summary>
        InInventory = 3,

        /// <summary>Cartridge is physically damaged or invalid.</summary>
        Broken = 100
    }
}
