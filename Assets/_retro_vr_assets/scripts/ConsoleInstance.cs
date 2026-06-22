using System;
using SK.Libretro.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LibretroInstance))]
public class Console : MonoBehaviour
{

    // LibretroInstance
    private LibretroInstanceVariable  _libretroInstance;
    [SerializeField] private string coreName;
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Collider screenCollider;

    [SerializeField] private string romName;
    private bool initialized = false;
    private bool running = false;

    void Awake()
    {
        // start new instance of libretro
        if (_libretroInstance == null)
        {
            _libretroInstance = ScriptableObject.CreateInstance<LibretroInstanceVariable>();
        }

        _libretroInstance.Current = GetComponent<LibretroInstance>();

        Initialize();
    }

    void Update()
    {
               // press E to set input focus to the console new and old unity input system
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Input focus set to console.");
            _libretroInstance.Current.InputEnabled = true;
            _libretroInstance.SetInputEnabled(true);
        }
    }

    void Initialize()
    {
        if (_libretroInstance == null)
        {
            Debug.LogError("LibretroInstance component not found. Cannot initialize console.");
            return;
        }

        if (initialized)
        {
            Debug.LogWarning("Console is already initialized. Skipping initialization.");
            return;
        }

        _libretroInstance.Current.Renderer = screenRenderer;
        _libretroInstance.Current.Collider = screenCollider;
        Debug.Log($"Initializing console with core: {coreName} and ROM: {romName} in path: {GamePaths.RomsFolderPath}");
        _libretroInstance.Current.Initialize(coreName, GamePaths.RomsFolderPath, romName);
        initialized = true;
    }

    void DeInitialize()
    {
        _libretroInstance.Current.Renderer = null;
        _libretroInstance.Current.Collider = null;
        _libretroInstance.Current.DeInitialize();
        initialized = false;
    }

    public void StartRom()
    {
        if (!initialized || _libretroInstance == null)
        {
            Debug.LogError("Console not initialized. Cannot start ROM.");
            return;
        }

        _libretroInstance.Current.StartContent();
        running = true;
    }

    public void StopRom()
    {
        if (!initialized || _libretroInstance == null)
        {
            Debug.LogError("Console not initialized. Cannot stop ROM.");
            return;
        }

        if (!running)
        {
            Debug.LogWarning("ROM is not running. Nothing to stop.");
            return;
        }

        _libretroInstance.StopContent();
        running = false;
    }


}
