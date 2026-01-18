using System;
using System.Runtime.InteropServices;

namespace RetroLib.LibretroHost.Native
{
    public sealed class LibretroLoader : IDisposable
    {
        private IntPtr handle;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);
#endif

        public void Load(string path)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            handle = LoadLibrary(path);
            if (handle == IntPtr.Zero)
                throw new Exception($"Failed to load core: {path}");
#else
            // Android / Quest: .so já carregado pelo sistema
            handle = IntPtr.Zero;
#endif
        }

        public T LoadFunction<T>(string name) where T : Delegate
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            IntPtr fn = GetProcAddress(handle, name);
            if (fn == IntPtr.Zero)
                throw new MissingMethodException($"Libretro symbol not found: {name}");

            return Marshal.GetDelegateForFunctionPointer<T>(fn);
#else
            // Android: resolve via DllImport("__Internal")
            return (T)(object)Marshal.GetDelegateForFunctionPointer(
                GetInternalFunctionPointer(name),
                typeof(T)
            );
#endif
        }

#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
        private static IntPtr GetInternalFunctionPointer(string name)
        {
            throw new PlatformNotSupportedException(
                "Dynamic symbol resolution is platform-specific. " +
                "Android implementation will bind via LibretroApi."
            );
        }
#endif

        public void Unload()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (handle != IntPtr.Zero)
            {
                FreeLibrary(handle);
                handle = IntPtr.Zero;
            }
#endif
        }

        public void Dispose() => Unload();
    }
}
