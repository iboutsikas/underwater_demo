using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedColorer : MonoBehaviour
{

    private Renderer meshRenderer;

    public Color InstanceColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        //var thing = GetComponent<LODGroup>();        
        //var thing2 = thing.GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();

        props.SetColor("_Color", InstanceColor);

        meshRenderer.SetPropertyBlock(props);
    }

}
