using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BayAudioPlayer : MonoBehaviour
{

#if UNITY_EDITOR
    public bool OverrideDataFeed = false;
    public float TurbidityOverride = 0.0f;
    public float PHOverride = 1.0f;
#endif

    [Range(0, 1)]
    [SerializeField]
    float m_MaxVolume = 1.0f;
    [Range(0, 1)]
    [SerializeField]
    float m_MinVolume = 0.0f;
    [Range(1, 14)]
    [SerializeField]
    float m_MinPh = 1.0f;
    [Range(1, 14)]
    [SerializeField]
    float m_MaxPh = 14.0f;

    [Range(0, 3)]
    [SerializeField]
    float m_MinSpeed = 0.0f;
    [Range(0, 3)]
    [SerializeField]
    float m_MaxSpeed = 3.0f;
    [SerializeField]
    float m_MinTurbidity = 0.0f;
    [SerializeField]
    float m_MaxTurbidity = 100.0f;

    private AudioSource audioSource;
    
    [SerializeField]
    #pragma warning disable 0649
    private AudioClip audioClip;
    #pragma warning restore 0649

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.playOnAwake = true;
        audioSource.loop = true;
        audioSource.pitch = 1.0f;
        audioSource.Play();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var data = DataContainer.GetData();
        float ph = data.PH;
        float turbidity = data.Turbidity;

#if UNITY_EDITOR
        if (OverrideDataFeed)
        {
            ph = PHOverride;
            turbidity = TurbidityOverride;
        }
#endif
        float volume_t = (ph - m_MinPh) / (m_MaxPh - m_MinPh);
        float volume = Mathf.Lerp(m_MinVolume, m_MaxVolume, volume_t);

        float speed_t = (turbidity - m_MinTurbidity) / (m_MaxTurbidity - m_MinTurbidity);
        float speed = Mathf.Lerp(m_MinSpeed, m_MaxSpeed, speed_t);


        audioSource.volume = volume;
        audioSource.pitch = speed;
    }


    private void Update()
    {
    }
}
