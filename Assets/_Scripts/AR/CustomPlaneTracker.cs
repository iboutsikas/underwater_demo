using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARSessionOrigin))]
public class CustomPlaneTracker : MonoBehaviour
{
    private ARSessionOrigin sessionOrigin;
    private ARPlaneManager planeManager;
    private bool instantiated = false;
    private GameObject spawn;

    public GameObject ScenePrefab;

    private void Awake()
    {
        sessionOrigin = GetComponent<ARSessionOrigin>();
        planeManager = GetComponent<ARPlaneManager>();

        planeManager.planesChanged += On_PlanesChanged;
    }

    private void On_PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count == 0)
            return;

        if (instantiated)
            return;

        ARPlane plane = args.added[0];

        spawn = Instantiate(ScenePrefab);

        spawn.transform.position = plane.transform.position;

        sessionOrigin.MakeContentAppearAt(spawn.transform, spawn.transform.rotation);
        instantiated = true;
        planeManager.planesChanged -= On_PlanesChanged;
        planeManager.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
