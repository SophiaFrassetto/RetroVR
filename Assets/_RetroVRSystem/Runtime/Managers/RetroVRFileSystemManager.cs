using System.IO;
using UnityEngine;

public class RetroVRFileSystemManager : MonoBehaviour
{
    void Awake()
    {
        CreateBaseFolders();
    }

    void CreateBaseFolders()
    {
        string root = Application.persistentDataPath;

        // User-facing folders
        Create(Path.Combine(root, "roms"));
        Create(Path.Combine(root, "roms/snes"));
        Create(Path.Combine(root, "roms/gba"));

        // Libretro internal structure (optional, but explicit)
        string libretro = Path.Combine(root, "Libretro");
        Create(libretro);
        Create(Path.Combine(libretro, "cores"));
        Create(Path.Combine(libretro, "system"));
        Create(Path.Combine(libretro, "core_assets"));
        Create(Path.Combine(libretro, "core_options"));
        Create(Path.Combine(libretro, "saves"));
        Create(Path.Combine(libretro, "states"));
        Create(Path.Combine(libretro, "temp"));
    }

    void Create(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
