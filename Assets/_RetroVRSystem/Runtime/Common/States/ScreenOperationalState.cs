using UnityEngine;

namespace retrovr.system
{
    /// <summary>
    /// Represents the execution/visual state of the screen/TV.
    /// Decides what the user sees: no signal, booting, content, etc.
    /// </summary>
    public enum ScreenOperationalState
    {
        /// <summary>Screen is turned off.</summary>
        Off = 0,

        /// <summary>Screen is on but not emitting content.</summary>
        Standby = 1,

        /// <summary>Screen has power but no input from the console.</summary>
        NoSignal = 2,

        /// <summary>Screen is booting together with the console.</summary>
        Booting = 3,

        /// <summary>Screen is actively showing the emulated game.</summary>
        ShowingContent = 4,

        /// <summary>An error occurred (invalid input, core failure, etc).</summary>
        Error = 100
    }
}
