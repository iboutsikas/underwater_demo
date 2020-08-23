using UnityEngine;

[RequireComponent(typeof(InstancedColorer))]
public class BayAnimator : MonoBehaviour
{
    private Color currentColor = Color.white;
    private InstancedColorer[] colorers;

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

    void Start()
    {
        colorers = GetComponentsInChildren<InstancedColorer>();
    }

    private void FixedUpdate()
    {
        var data = DataContainer.GetData();
        float temperature = data.Temperature;
        float oxygen = data.Oxygen;

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
            float scaleFactor = (float)Mathf.Abs(Mathf.Sin(Time.time * oxygen * 0.1f));
            transform.localScale = Vector3.one + new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var c in colorers)
        {
            c.InstanceColor = currentColor;
        }
    }
}
