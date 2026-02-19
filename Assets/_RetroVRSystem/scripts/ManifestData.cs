using UnityEngine;

namespace retrovr.system
{
    public class ManifestData
    {
        public string Name { get; set; }
        public string Core { get; set; }
        public string System { get; set; }
        public RomEntry[] Games { get; set; }
    }
}
