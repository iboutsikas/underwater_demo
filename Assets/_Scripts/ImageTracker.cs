using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracker : MonoBehaviour
{
    public TextMeshProUGUI DebugText;
    public GameObject TestMesh;
    private ARTrackedImageManager imageManager;

    Dictionary<string, GameObject> spawns;

    private void Awake() {
        imageManager = FindObjectOfType<ARTrackedImageManager>();
        spawns = new Dictionary<string, GameObject>();
    }

    private void OnEnable() {
        imageManager.trackedImagesChanged += On_ImageChange;
    }



    private void OnDisable() {
        imageManager.trackedImagesChanged -= On_ImageChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void On_ImageChange(ARTrackedImagesChangedEventArgs args) {
        DebugText.text = "";
        
        if (args.added.Count > 0) {
            DebugText.text += "Added: \n";
        }

        foreach(var image in args.added) {
            image.transform.localScale = new Vector3(0.01f, 1.0f, 0.01f);

            var name = image.referenceImage.name;
            GameObject spawn;
            if (!spawns.TryGetValue(name, out spawn)) {
                DebugText.text += name;
                DebugText.text += '\n';
                spawn = Instantiate(TestMesh, Vector3.zero, Quaternion.identity);
                spawn.SetActive(true);
                spawns.Add(name, spawn);
            }

            spawn.transform.position = image.transform.position;

        }

        if (args.removed.Count > 0) {
            DebugText.text += "Removed: \n";
        }
        
        foreach (var image in args.removed) {

            var name = image.referenceImage.name;

            GameObject spawn;
            if (spawns.TryGetValue(name, out spawn)) {
                spawn.SetActive(false);
            }
        }
        
        if (args.updated.Count > 0) {
            DebugText.text += "Updated: \n";
        }

        foreach (var image in args.updated) {

            var name = image.referenceImage.name;
            DebugText.text += name;
            DebugText.text += '\n';

            GameObject spawn;
            if (spawns.TryGetValue(name, out spawn)) {
                if (image.trackingState == TrackingState.Limited) {
                    spawn.SetActive(false);
                }
                else {
                    // The image extents is only valid when the image is being tracked
                    image.transform.localScale = new Vector3(image.size.x, 1f, image.size.y);
                    spawn.SetActive(true);
                    spawn.transform.position = image.transform.position;
                }
            }
        }

    }
}
