// Copyright 2021 by Hextant Studios. https://HextantStudios.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/
using UnityEngine;

// A ScriptableObject-based singleton for use at runtime (in-game).
// Note: OnEnable() / OnDisable() should be used to register with any global events
// to properly support domain reloads.
public abstract class ScriptableObjectSingleton<T> : ScriptableObject
    where T : ScriptableObjectSingleton<T>
{
    // The singleton instance. (Not thread safe but fine for ScriptableObjects.)
    public static T instance => _instance != null ? _instance : Initialize();
    static T _instance;

    // Finds or creates the singleton instance and stores it in _instance. This can
    // be called from a derived type to ensure creation of the singleton using the 
    // [RuntimeInitializeOnLoadMethod] attribute on a static method.
    protected static T Initialize()
    {
        // Prevent runtime instances from being created outside of Play Mode or
        // re-created during OnDestroy() handlers when exiting Play Mode.
        //if (!Application.isPlaying) return null;

        // If the instance is already valid, return it. Needed if called from a
        // derived class that wishes to ensure the instance is initialized.
        if (_instance != null) return _instance;

        // Find the existing instance (across domain reloads) or create a new one.
        var instances = Resources.FindObjectsOfTypeAll<T>();
        return instances.Length > 0 ? _instance = instances[0] :
            CreateInstance<T>();
    }

    // Called once during creation of this instance. Derived classes should call
    // this base method first if overridden.
    protected virtual void Awake()
    {
        // Verify there is only a single instance; catches accidental creation
        // from other CreateInstance() calls.
        Debug.Assert(_instance == null);

        // Ensure _instance is assigned here to prevent possible double-creation
        // should the instance property be called by a derived class handler.
        _instance = (T)this;

        // HideAndDontSave prevents Resources.UnloadUnusedAssets() from destroying
        // the singleton instance if called or when new scenes are loaded.
        _instance.hideFlags = HideFlags.HideAndDontSave;
    }

    // Called when the singleton is destroyed after exiting play mode.
    protected virtual void OnDestroy() => _instance = null;

    // Called when the singleton is created *or* after a domain reload in the editor.
    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    // Called when entering or exiting play mode.
    void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
    {
        // Note that EnteredEditMode is used because ExitingPlayMode occurs *before*
        // MonoBehavior.OnDetroy() which is likely too early.
        if (stateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            UnityEditor.EditorApplication.playModeStateChanged -=
                OnPlayModeStateChanged;
            DestroyImmediate(this);
        }
    }
#endif
}