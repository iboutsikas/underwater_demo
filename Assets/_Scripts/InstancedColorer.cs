using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedColorer : MonoBehaviour
{

    private MeshRenderer meshRenderer;

    public Color InstanceColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();

        props.SetColor("_Color", InstanceColor);

        meshRenderer.SetPropertyBlock(props);
    }
}
