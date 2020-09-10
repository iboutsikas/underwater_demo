using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEditor;

[RequireComponent(typeof(InstancedColorer))]
[ExecuteInEditMode]
public class BayAnimator : MonoBehaviour
{
    private Color currentColor = Color.white;
    private List<InstancedColorer> colorers = new List<InstancedColorer>();
    private BayData _data;
    private IDisposable _subscription;

#if UNITY_EDITOR
    public bool OverrideDataFeed = false;
    public float TemperatureOverride = 0.0f;
    public float OxygenOverride = 0.0f;
    public float TurbidityOverride = 0.0f;
#endif

    public bool VisualizeTemperature = false;
    public Color WarmColor = Color.red;
    [Range(0, 100)]
    public float WarmTemperature = 75.0f;
    public Color ColdColor = Color.blue;
    [Range(0, 100)]
    public float ColdTemperature = 10.0f;

    public bool VisualizeOxygen = false;
    public bool AnimateTurbidity = false;

    private void OnValidate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Component c;
            if (!child.TryGetComponent(typeof(InstancedColorer), out c))
            {
                child.gameObject.AddComponent<InstancedColorer>();
            }
        }
    }

    void Start()
    { 
        colorers.AddRange(GetComponentsInChildren<InstancedColorer>());
        colorers.Insert(0, GetComponent<InstancedColorer>());

        _subscription = DataContainer.DataStream.Subscribe(data => _data = data);
    }

    private void ProcessSelf()
    {
        float temperature = _data.Temperature;
        float oxygen = _data.Oxygen;

#if UNITY_EDITOR
        if (OverrideDataFeed)
        {
            temperature = TemperatureOverride;
            oxygen = OxygenOverride;
        }
#endif
        if (VisualizeTemperature)
        {
            float t = (temperature - ColdTemperature) / (WarmTemperature - ColdTemperature);
            currentColor = Color.Lerp(ColdColor, WarmColor, t);
        }

        if (VisualizeOxygen)
        {
            float scaleFactor = (float)Mathf.Abs(Mathf.Sin(Time.time * oxygen));
            transform.localScale = Vector3.one + new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    private void FixedUpdate()
    {
        ProcessSelf();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            ProcessSelf();
        }
#endif
        //Debug.Log("Potato");

        foreach (var c in colorers)
        {
            c.InstanceColor = currentColor;
        }
    }

    private void OnDestroy()
    {
        _subscription?.Dispose();
    }
}
