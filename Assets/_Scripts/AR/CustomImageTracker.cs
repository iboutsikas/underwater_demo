using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[Serializable]
public class ImageData {
    [SerializeField, Tooltip("The source texture for the image. Must be marked as readable.")]
    Texture2D m_Texture;

    public Texture2D texture {
        get => m_Texture;
        set => m_Texture = value;
    }

    [SerializeField, Tooltip("The name for this image.")]
    string m_Name;

    public string name {
        get => m_Name;
        set => m_Name = value;
    }

    [SerializeField, Tooltip("The width, in meters, of the image in the real world.")]
    float m_Width;

    public float width {
        get => m_Width;
        set => m_Width = value;
    }

    [SerializeField, Tooltip("The prefab to spawn on this image, when it is detected")]
    GameObject m_Prefab;

    public GameObject prefab {
        get => m_Prefab;
        set => m_Prefab = value;
    }

    [HideInInspector]
    public AddReferenceImageJobState JobState;
}


[RequireComponent(typeof(ARTrackedImageManager))]
[RequireComponent(typeof(ARSessionOrigin))]
public class CustomImageTracker : MonoBehaviour {
    private ARSession arSession;
    private ARSessionOrigin sessionOrigin;
    private ARTrackedImageManager imageManager;
    private MutableRuntimeReferenceImageLibrary referenceImageLibrary;

    private Dictionary<string, GameObject> spawns = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> prefabsMap = new Dictionary<string, GameObject>();

    public ImageData[] Images;

    private void Awake() {
        sessionOrigin = GetComponent<ARSessionOrigin>();
        imageManager = GetComponent<ARTrackedImageManager>();

        if (imageManager == null)
            Debug.Log($"[{nameof(CustomImageTracker)}] Unable to get ARTrackedImageManager");

        if (!imageManager.descriptor.supportsMutableLibrary) {
            Debug.LogError($"[{nameof(CustomImageTracker)}] The manager does not support dynamic libraries");
            return;
        }

        // Either get the existing library, or make a new one if the current is not dynamic
        if (imageManager.referenceLibrary != null && imageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary) {
            referenceImageLibrary = imageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        }
        else {
            referenceImageLibrary = imageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
        }

        if (referenceImageLibrary == null) {
            Debug.LogError($"[{nameof(CustomImageTracker)}] Reference Image library is null");
            return;
        }

        imageManager.referenceLibrary = referenceImageLibrary;
        imageManager.requestedMaxNumberOfMovingImages = Images.Length;

        StartCoroutine(AddCoroutine());
    }

    IEnumerator AddCoroutine() {
        bool done = true;
        foreach (var image in Images) {
            if (image.JobState.status != AddReferenceImageJobStatus.Success) {
                done = false;
                break;
            }
        }

        if (done) {
            StopCoroutine("AddCoroutine");
            yield return null;
        }

        yield return new WaitForSeconds(2);

        try {
            foreach (var entry in Images) {

                Debug.Log($"[{nameof(CustomImageTracker)}] Adding texture to library: {entry.name}");
                prefabsMap[entry.name] = entry.prefab;

                entry.JobState = referenceImageLibrary.ScheduleAddImageWithValidationJob(entry.texture, entry.name, entry.width);

            }
        }
        catch (Exception e) {
            Debug.LogError($"[{nameof(CustomImageTracker)}] ScheduleAddImageJob threw exception: {e.Message}");
        }

    }

    StringBuilder m_StringBuilder = new StringBuilder();
    private void OnGUI() {
#if ENABLE_AR_DEBUG_OUTPUT
        var fontSize = 50;
        GUI.skin.button.fontSize = fontSize;
        GUI.skin.label.fontSize = fontSize;

        float margin = 100;

        GUILayout.BeginArea(new Rect(margin, margin, Screen.width - margin * 2, Screen.height - margin * 2));
        m_StringBuilder.Clear();
        foreach (var image in Images) {
            m_StringBuilder.AppendLine($"{image.name}: {image.JobState.status}");
        }
        GUILayout.Label(m_StringBuilder.ToString());
        GUILayout.EndArea();
#endif
    }

    void Start() {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable() {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    /// <summary>
    /// All the tracked image resizing is taken care of in the TackedImageInfoManager. 
    /// We do not need to deal with it here for now.
    /// </summary>
    /// <param name="args"></param>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args) {
        foreach (var image in args.added) {
            var name = image.referenceImage.name;
            //image.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
            GameObject spawn;
            if (!spawns.TryGetValue(name, out spawn)) {
                var prefab = prefabsMap[name];
                spawn = Instantiate(prefab);
                spawn.transform.SetPositionAndRotation(image.transform.position, image.transform.rotation);
                spawns[name] = spawn;
            }
        }

        foreach (var image in args.updated) {
            //image.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
            var imageName = image.referenceImage.name;
            var pos = image.transform.position;

            GameObject spawn = null;
            if (image.trackingState == TrackingState.Tracking) {
                if (spawns.TryGetValue(imageName, out spawn)) {
                    if (!spawn.activeSelf) {
                        spawn.SetActive(true);
                    }
                    spawn.transform.position = pos;
                    sessionOrigin.MakeContentAppearAt(spawn.transform, spawn.transform.position, spawn.transform.rotation);
                }
            }
            else if (image.trackingState == TrackingState.Limited) {
                if (spawns.TryGetValue(imageName, out spawn)) {
                    //Destroy(spawn);
                    spawn.SetActive(false);
                }
            }
        }
    }
}
