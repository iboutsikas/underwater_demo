using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class UnderwaterEffect : MonoBehaviour
{
    public Material material;

    public UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var opts = _uiManager.GetSimulationOptions();

        material.SetFloat("_NoiseScale", opts.NoiseScale);
        material.SetFloat("_NoiseFrequency", opts.NoiseFrequency);
        material.SetFloat("_NoiseSpeed", opts.NoiseSpeed);
        material.SetFloat("_PixelOffset", opts.MaxPixelDisplacement);
        material.SetColor("_TintColor", opts.TintColor);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
