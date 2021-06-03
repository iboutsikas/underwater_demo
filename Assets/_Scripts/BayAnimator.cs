using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(InstancedMaterialModifier))]
[ExecuteInEditMode]
public class BayAnimator : MonoBehaviour
{
    private Color currentColor = Color.white;
    private float currentTilingOFsset = 0.0f;
    private float currentAlpha = 1.0f;

    private List<InstancedMaterialModifier> materialModifiers = new List<InstancedMaterialModifier>();

#if UNITY_EDITOR
    public bool OverrideDataFeed = false;
    public float TemperatureOverride = 0.0f;
    public float OxygenOverride = 0.0f;
    [Range(0.0f, 14.0f)]
    public float PHOverride = 0.0f;
    [Range(0.0f, 100.0f)]
    public float ClorophyllOverride = 0.0f;
#endif

    public bool VisualizeTemperature = false;
    public Color WarmColor = Color.red;
    [Range(0, 100)]
    public float WarmTemperature = 75.0f;
    public Color ColdColor = Color.blue;
    [Range(0, 100)]
    public float ColdTemperature = 10.0f;
    [Range(0.0f, 1.0f)]
    public float TemperatureBias = 0.0f;

    public bool VisualizeOxygen = false;

    public bool VisualizePH = false;
    [Range(0.0f, 2.0f)]
    public float HighOffset = 2.0f;
    [Range(0.0f, 2.0f)]
    public float LowOffset = 0.0f;

    public bool VisualizeChrolophyll = false;
    [Range(0.0f, 1.0f)]
    public float MinAlpha = 0.0f;
    public float MinChlf = 1.0f;
    [Range(0.0f, 1.0f)]
    public float MaxAlpha = 1.0f;
    public float MaxChlf = 100.0f;
    


    private void OnValidate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Component c;
            if (!child.TryGetComponent(typeof(InstancedMaterialModifier), out c))
            {
                child.gameObject.AddComponent<InstancedMaterialModifier>();
            }
        }
    }


    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += OnEditorUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= OnEditorUpdate;
#endif
    }

    // FixedUpdate is not called on the editor. So we need this workaround so we can see
    // the effects during design time
    void OnEditorUpdate()
    {
        if (!EditorApplication.isPlaying)
            ProcessSelf();
    }

    void Start()
    { 
        materialModifiers.AddRange(GetComponentsInChildren<InstancedMaterialModifier>());
        var myInstancedColorer = GetComponent<InstancedMaterialModifier>();
        if (myInstancedColorer != null)
            materialModifiers.Insert(0, myInstancedColorer);
    }

    private void ProcessSelf()
    {
        // These macros are checked here so we can fake the effect while in edit mode. Not just in play
        BayData _data = DataContainer.instance.CurrentSample;
        
        if (_data == null)
            return;
               
        DoTemperature(_data);
        DoOxygen(_data);
        DoPH(_data);
        DoChlorophyll(_data);
    }
    
    private void DoTemperature(BayData data)
    {
        if (!VisualizeTemperature)
            return;

        float temperature = data.Temperature;
        float time = Time.time;

#if UNITY_EDITOR
        temperature = OverrideDataFeed ? TemperatureOverride : temperature;
        time = (float)EditorApplication.timeSinceStartup;
#endif
        float t = (temperature - ColdTemperature) / (WarmTemperature - ColdTemperature);
        t += (Mathf.Sin(time) * TemperatureBias);
        currentColor = Color.Lerp(ColdColor, WarmColor, t);
    }

    private void DoOxygen(BayData data)
    {
        if (!VisualizeOxygen)
        {
            //transform.localScale = _originalScale;
            return;
        }

        float oxygen = data.Oxygen;
        float time = Time.time;
#if UNITY_EDITOR
        time = (float)EditorApplication.timeSinceStartup;
        oxygen = OverrideDataFeed ? OxygenOverride : oxygen;
#endif
        float scaleFactor = (float)Mathf.Abs(Mathf.Sin(time * oxygen));
        transform.localScale = Vector3.one + new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    private void DoPH(BayData data)
    {
        if (!VisualizePH)
            return;

        float ph = data.PH;

#if UNITY_EDITOR
        ph = OverrideDataFeed ? PHOverride : ph;
#endif

        float t = ph / 14;
        currentTilingOFsset = Mathf.Lerp(LowOffset, HighOffset, t);
    }

    private void DoChlorophyll(BayData data)
    {
        if (!VisualizeChrolophyll)
            return;

        float chlf = data.Chlorophyll;

#if UNITY_EDITOR
        chlf = OverrideDataFeed ? ClorophyllOverride : chlf;
#endif
        float t = (chlf - MinChlf) / (MaxChlf - MinChlf);
        currentAlpha = Mathf.Lerp(MinAlpha, MaxAlpha, t);
    }

    private void FixedUpdate()
    {
        ProcessSelf();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var c in materialModifiers)
        {
            if (VisualizeTemperature)
            {
                c.EnableEmission();
                c.EmissionColor = currentColor;
            }
            else
                c.DisableEmission();
            
            if (VisualizePH)
            {
                c.EnableOffset();
                c.InstanceOffset = new Vector2(0, currentTilingOFsset);
            }
            else
            {
                c.DisableOffset();
            }

            if (VisualizeChrolophyll)
            {
                c.EnableAlpha();
                c.InstanceAlpha = currentAlpha;
            }
            else
            {
                c.DisableAlpha();
            }
        }
    }

    private void OnDestroy()
    {
    }
}
