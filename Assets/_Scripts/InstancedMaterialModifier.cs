using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class InstancedMaterialModifier : MonoBehaviour
{
    static readonly string EmissionFlagName = "_EMISSION";
    static readonly string EmissionColorPropertyName = "_EmissionColor";
    static readonly string TexturePropertyName = "_BaseMap";

    private Renderer meshRenderer;
    private bool emissionEnabled = false;
    private bool offsetEnabled = false;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Color instanceColor = Color.white;
    private Color _originalColor = Color.black;

    public Color InstanceColor { get => instanceColor; 
        set { 
            instanceColor = value;
        } 
    }

    [SerializeField]
    public Vector2 InstanceOffset = new Vector2(0, 0);

    private void OnValidate()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();

        _material = new Material(meshRenderer.sharedMaterial.shader);
        //_material.name = "Potato";
        _material.CopyPropertiesFromMaterial(meshRenderer.sharedMaterial);
        meshRenderer.material = _material;
    }

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
        
        if (_material == null)
        {
            _material = new Material(meshRenderer.sharedMaterial.shader);
            _material.name = "Potato";
            _material.CopyPropertiesFromMaterial(meshRenderer.sharedMaterial);
            meshRenderer.material = _material;
        }

        _originalColor = _material.GetColor(EmissionColorPropertyName);
    }

    public void EnableEmission()
    {
        if (emissionEnabled)
            return;

        _material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        _material.EnableKeyword(EmissionFlagName);
        emissionEnabled = true;
    }

    public void DisableEmission()
    {

        if (!emissionEnabled)
            return;

        _material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        _material.DisableKeyword(EmissionFlagName);

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor(EmissionColorPropertyName, _originalColor);
        meshRenderer.SetPropertyBlock(props);

        emissionEnabled = false;
    }

    public void EnableOffset()
    {
        if (offsetEnabled)
            return;

        offsetEnabled = true;
    }

    public void DisableOffset()
    {
        if (!offsetEnabled)
            return;

        _material.SetTextureOffset(TexturePropertyName, new Vector2(0, 0));
        offsetEnabled = false;
    }

    private void FixedUpdate()
    {
        if (meshRenderer == null)
            return;
        UpdateColor();
        UpdateOffset();
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

    private void OnEditorUpdate()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying && meshRenderer != null)
        {
            UpdateColor();
            UpdateOffset();
        }
#endif
    }

    private void UpdateColor()
    {

        if (!emissionEnabled)
            return;

        MaterialPropertyBlock props = new MaterialPropertyBlock();

        props.SetColor(EmissionColorPropertyName, InstanceColor);
        
        meshRenderer.SetPropertyBlock(props);
    }

    private void UpdateOffset()
    {
        if (!offsetEnabled)
            return;
        _material.SetTextureOffset(TexturePropertyName, InstanceOffset);
    }

}
