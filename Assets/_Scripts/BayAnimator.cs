using UnityEngine;

public enum TurbulanceAxis
{ 
    Axis_X,
    Axis_Y,
    Axis_Z
};

[RequireComponent(typeof(InstancedColorer))]
public class BayAnimator : MonoBehaviour
{
    private Vector3 initialPosition;
    private Color currentColor = Color.white;
    private InstancedColorer colorer;

    [Header("Data Feed")]
    public bool UseLiveData = false;
    [Range(0, 15)]
    public int Day = 0;

    [Header("Temperature")]
    public bool AnimateTemperature = false;
    public float Temperature = 15.0f;
    public float ColdTemperature = 10.0f;
    public float WarmTemperature = 25.0f;
    public Color ColdColor = Color.blue;
    public Color WarmColor = Color.red;

    [Header("Oxygen")]
    public bool AnimateOxygen = false;
    public float BreathingSpeed = 1.0f;
    //public float ExpansionFactor = 1.0f;


    [Header("Turbulence")]
    public bool AnimateTurbulance = false;
    public TurbulanceAxis RotationAxis = TurbulanceAxis.Axis_Y;
    public float Turbulence = 0.84f;

    [Header("General")]
    public bool EnableBobbing = false;
    public float BobAmplitude = 0.6f;


    void Start()
    {
        initialPosition = transform.position;
        colorer = GetComponent<InstancedColorer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AnimateTemperature)
        {
            float temp = UseLiveData ? DataContainer.Data[Day].Temperature : Temperature;

            float t = (temp - ColdTemperature) / (WarmTemperature - ColdTemperature);

            currentColor = Color.Lerp(ColdColor, WarmColor, t);
            colorer.InstanceColor = currentColor;
        }

        if (AnimateOxygen)
        {
            float multiplier = UseLiveData ? DataContainer.Data[Day].Oxygen * 0.01f : BreathingSpeed;
            float scaleFactor = (float)Mathf.Abs(Mathf.Sin(Time.time * multiplier));
            transform.localScale = Vector3.one + new Vector3(scaleFactor, scaleFactor, scaleFactor);

        }

        if (EnableBobbing)
        {
            float y = BobAmplitude * (float)Mathf.Cos(Time.time);
            transform.position = new Vector3(initialPosition.x, initialPosition.y + y, initialPosition.z);
        }

        if (AnimateTurbulance)
        {
            Vector3 axis;
            switch (RotationAxis)
            {
                case TurbulanceAxis.Axis_X:
                    axis = transform.right;
                    break;
                case TurbulanceAxis.Axis_Y:
                    axis = transform.up;
                    break;
                case TurbulanceAxis.Axis_Z:
                    axis = transform.forward;
                    break;
                default:
                    axis = transform.up;
                    break;
            }
            float angle = 45.0f * Time.deltaTime * Turbulence;

            transform.Rotate(axis, angle, Space.World);
        }


    }
}
