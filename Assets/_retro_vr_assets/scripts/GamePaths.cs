using System.IO;
using UnityEngine;

public static class GamePaths
{
    public static string RootFolder
    {
        get
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            return Path.GetDirectoryName(Application.dataPath);
#else
            return Application.persistentDataPath;
#endif
        }
    }

    public static string RomsFolderPath =>
        EnsureFolder(Path.Combine(RootFolder, "Roms"));

    public static string ConfigFolderPath =>
        EnsureFolder(Path.Combine(RootFolder, "Config"));

    private static string EnsureFolder(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path + Path.DirectorySeparatorChar;
    }
}
