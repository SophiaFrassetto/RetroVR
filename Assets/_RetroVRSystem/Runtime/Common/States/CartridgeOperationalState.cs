using UnityEngine;

namespace retrovr.system
{
    /// <summary>
    /// Represents the logical/usage lifecycle of a cartridge, 
    /// separate from its physical placement in the VR world.
    /// </summary>
    public enum CartridgeOperationalState
    {
        /// <summary>Cartridge is idle and not associated with a console.</summary>
        Idle = 0,

        /// <summary>Cartridge is inserted but not yet mounted to the emulator.</summary>
        Inserted = 1,

        /// <summary>Cartridge has been mounted and its ROM is in use.</summary>
        Mounted = 2,

        /// <summary>Cartridge has been ejected and is no longer in use.</summary>
        Ejected = 3,

        /// <summary>An error occurred while reading or loading the ROM.</summary>
        Error = 100
    }
}
