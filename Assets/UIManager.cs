using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private float _wobbleScale = 0.1f;
    [SerializeField]
    private float _wobbleFrequency = 0.1f;
    [SerializeField]
    private float _wobbleSpeed = 0.1f;
    [SerializeField]
    private float _pixelOffset = 0.005f;
    [SerializeField]
    private Color _tintColor = Color.white;

    public TMP_Text ScaleText;
    public TMP_Text FrequencyText;
    public TMP_Text SpeedText;
    public TMP_Text OffsetText;
    public ColorPicker ColorPicker;


    // Start is called before the first frame update
    void Start()
    {
        ColorPicker.onValueChanged.AddListener(color =>
        {
            _tintColor = color;
        });

       ColorPicker.AssignColor(_tintColor);
    }

    // Update is called once per frame
    void Update()
    {
        ScaleText.text = string.Format("Wobble Scale: {0:0.00}", _wobbleScale);
        FrequencyText.text = string.Format("Wobble Frquency: {0:0.00}", _wobbleFrequency);
        SpeedText.text = string.Format("Wobble Speed: {0:0.00}", _wobbleSpeed);
        OffsetText.text = string.Format("Wobble Offset: {0:0.000}", _pixelOffset);
    }

    public WaterSimulationOptions GetSimulationOptions()
    {
        return new WaterSimulationOptions()
        { 
            NoiseScale = _wobbleScale,
            NoiseFrequency = _wobbleFrequency,
            NoiseSpeed = _wobbleSpeed,
            MaxPixelDisplacement = _pixelOffset,
            TintColor = _tintColor
        };
    }


    public void OnScaleUpdated(System.Single scale)
    {
        _wobbleScale = scale;
    }
    public void OnFrequencyUpdated(System.Single frequency)
    {
        _wobbleFrequency = frequency;
    }
    public void OnSpeedUpdated(System.Single speed)
    {
        _wobbleSpeed = speed;
    }
    public void OnOffsetUpdated(System.Single offset)
    {
        _pixelOffset = offset;
    }
}
