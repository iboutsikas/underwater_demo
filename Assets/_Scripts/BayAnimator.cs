using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

struct AnimatorCache
{
    public int BreathLayerIndex;
    public int RotationLayerIndex;
    public int RotationSpeedHash;

    public AnimatorCache(Animator animator)
    {
        BreathLayerIndex    = animator.GetLayerIndex("utb_BreathingLayer");
        RotationLayerIndex  = animator.GetLayerIndex("utb_RotationLayer");
        RotationSpeedHash   = Animator.StringToHash("utb_RotationSpeed");
    }
}

[ExecuteInEditMode]
public class BayAnimator : MonoBehaviour
{
    private Color currentColor = Color.white;
    private float currentTilingOFsset = 0.0f;
    private float currentAlpha = 1.0f;
    private float currentScaleFactor = 0.0f;
    private float currentRotationSpeed = 1.0f;

    private List<InstancedMaterialModifier> materialModifiers = new List<InstancedMaterialModifier>();
    private List<Animator> animators = new List<Animator>();
    private List<AnimatorCache> cachedAnimatorProperties = new List<AnimatorCache>();

#if UNITY_EDITOR
    public bool OverrideDataFeed = false;
    public float TemperatureOverride = 0.0f;
    [Range(0.0f, 21.0f)]
    public float OxygenOverride = 0.0f;
    [Range(0.0f, 14.0f)]
    public float PHOverride = 0.0f;
    [Range(0.0f, 100.0f)]
    public float ClorophyllOverride = 0.0f;
    [Range(0.0f, 32.0f)]
    public float SalinityOverride = 0.0f;
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
    [Range(0.0f, 21.0f)]
    public float OxygenUpperBound = 21.0f;

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

    public bool VisualizeSalinity = false;
    [Range(0.0f, 32.0f)]
    public float MinSalinity = 0.0f;
    public float MinRotationSpeed = 0.0f;
    [Range(0.0f, 32.0f)]
    public float MaxSalinity = 32.0f;
    public float MaxRotationSpeed = 5.0f;




    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += AddRequiredComponents;
#endif
    }

    private void AddRequiredComponents()
    {
        Component c;

        // If we are attached to an object with a mesh, we add a modifier to ourselves
        if (!TryGetComponent(typeof(InstancedMaterialModifier), out c) && GetComponent<MeshRenderer>() != null)
            gameObject.AddComponent<InstancedMaterialModifier>();

        // We add a modifier to all our children as well
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.TryGetComponent(typeof(InstancedMaterialModifier), out c))
            {
                if (child.gameObject.GetComponent<MeshRenderer>() != null)
                    child.gameObject.AddComponent<InstancedMaterialModifier>();
            }
        }
    }

    private void PopulateCaches()
    {
        if (animators.Count > 0)
            animators.Clear();

        if (cachedAnimatorProperties.Count > 0)
            cachedAnimatorProperties.Clear();

        if (materialModifiers.Count > 0)
            materialModifiers.Clear();

        var myInstancedColorer = GetComponent<InstancedMaterialModifier>();
        if (myInstancedColorer != null)
            materialModifiers.Add(myInstancedColorer);
        materialModifiers.AddRange(GetComponentsInChildren<InstancedMaterialModifier>());

        var myAnimator = GetComponent<Animator>();
        if (myAnimator != null)
        {
            animators.Add(myAnimator);
            cachedAnimatorProperties.Add(new AnimatorCache(myAnimator));
        }

        var childrenAnimators = GetComponentsInChildren<Animator>();
        foreach (var a in childrenAnimators)
        {
            animators.Add(a);
            cachedAnimatorProperties.Add(new AnimatorCache(a));
        }
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall -= AddRequiredComponents;
#endif
        Component c;
        // Remove the modifiers on our children
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.TryGetComponent(typeof(InstancedMaterialModifier), out c))
            {
                DestroyImmediate(c);
            }
        }        
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += OnEditorUpdate;
#endif
        AddRequiredComponents();
        PopulateCaches();
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
        {
            ProcessSelf();
            ApplyChanges();
        }
    }

    private void ProcessSelf()
    {
        // These macros are checked here so we can fake the effect while in edit mode. Not just in play
        BayData data = DataContainer.instance.CurrentSample;
        
        if (data == null)
            return;
               
        DoTemperature(data);
        DoOxygen(data);
        DoPH(data);
        DoChlorophyll(data);
        DoSalinity(data);
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
            currentScaleFactor = 0;
            return;
        }

        float oxygen = data.Oxygen;
        float time = Time.time;
#if UNITY_EDITOR
        time = (float)EditorApplication.timeSinceStartup;
        oxygen = OverrideDataFeed ? OxygenOverride : oxygen;
#endif
        //float scaleFactor = (float)Mathf.Abs(Mathf.Sin(time) * oxygen);
        currentScaleFactor = OxygenUpperBound == 0.0f ? 0.0f : oxygen / OxygenUpperBound;
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

    private void DoSalinity(BayData data)
    {
        if (!VisualizeSalinity)
        {
            currentRotationSpeed = 0;
            return;
        }

        float salinity = data.Salinity;

#if UNITY_EDITOR
        salinity = OverrideDataFeed ? SalinityOverride : salinity;
#endif

        float t = (salinity - MinSalinity) / (MaxSalinity - MinSalinity);
        currentRotationSpeed = Mathf.Lerp(MinSalinity, MaxSalinity, t);
    }


    void FixedUpdate()
    {
        ProcessSelf();
        ApplyChanges();
    }

    // Update is called once per frame
    void ApplyChanges()
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

        for (int i = 0; i < animators.Count; i++)
        {
            var animator = animators[i];
            var cache = cachedAnimatorProperties[i];
#if UNITY_EDITOR

            // Since we are calling this method in the inspector as well to
            // see updates, we need to not apply anything here. The reason being,
            // that animators cannot play in edit mode
            if (animator.GetCurrentAnimatorStateInfo(0).length >
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                continue;
            }
#endif
            if (cache.BreathLayerIndex != -1)
                animator.SetLayerWeight(cache.BreathLayerIndex, currentScaleFactor);
            if (cache.RotationLayerIndex != -1)
                animator.SetFloat(cache.RotationSpeedHash, currentRotationSpeed);
        }
    }
}
