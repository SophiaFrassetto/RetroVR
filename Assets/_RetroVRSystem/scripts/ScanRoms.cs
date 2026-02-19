using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RetroVR.System.Assets._RetroVRSystem.Scripts
{
    public class Wrapper : MonoBehaviour
    {
        private string mainDirectory;
        private string romsDirectory;

        void Awake()
        {
            mainDirectory = Application.persistentDataPath + "/Libretro";
            romsDirectory = mainDirectory + "/roms";
        }

        void Start()
        {
            ScanRoms();
        }

        // Force scan ROMs regardless of manifest existence
        public void ForceScanRoms()
        {
            Debug.Log("[RetroVR] Force scanning ROMs...");
            ScanRoms(true);
        }

        private void ScanRoms(bool forceScan = false)
        {
            Debug.Log($"[RetroVR] Starting ROM scan... (Force: {forceScan})");

            if (!Directory.Exists(romsDirectory))
            {
                Debug.LogError($"[RetroVR] ROMs directory not found: {romsDirectory}");
                return;
            }

            var systemDirectorys = Directory.GetDirectories(romsDirectory);
            Debug.Log($"[RetroVR] Found {systemDirectorys.Length} system directorys");

            foreach (var systemDirectory in systemDirectorys)
            {
                string systemName = Path.GetFileName(systemDirectory);
                Debug.Log($"[RetroVR] Processing system: {systemName}");

                // TODO - adapt with supported system for filter with system extensions or define defaults.
                var romFiles = Directory.GetFiles(systemDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.EndsWith(".json")).ToArray(); // Exclude JSON files

                Debug.Log($"[RetroVR] Found {romFiles.Length} ROM files in {systemName}");

                CreateOrUpdateManifest(systemDirectory, systemName, romFiles, forceScan);
            }

            Debug.Log("[RetroVR] ROM scan completed.");

        }

        private void CreateOrUpdateManifest(string systemPath, string systemName, string[] romFiles, bool forceScan)
        {
            // TODO - use manifestData to create manifest.json with systemPath, systemName and list of roms
            // TODO - validade changes for dont reescan unecessary
            return;
        }

    }
}
