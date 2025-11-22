using UnityEngine;

namespace retrovr.system
{
    /// <summary>
    /// Represents the internal execution state of the console.
    /// Controls the Libretro lifecycle and power flow.
    /// </summary>
    public enum ConsoleOperationalState
    {
        /// <summary>Console is fully powered off.</summary>
        Off = 0,

        /// <summary>Console is powered but idle, with no cartridge in use.</summary>
        Standby = 1,

        /// <summary>A cartridge is inserted and recognized, but execution hasn't started.</summary>
        CartridgeInserted = 2,

        /// <summary>Libretro core and ROM are being initialized.</summary>
        Initializing = 3,

        /// <summary>The console is actively running the loaded game.</summary>
        Running = 4,

        /// <summary>The console is paused. (Optional future feature.)</summary>
        Paused = 5,

        /// <summary>Console is shutting down / stopping emulation.</summary>
        ShuttingDown = 6,

        /// <summary>An error occurred during initialization, loading, or execution.</summary>
        Error = 100
    }
}
