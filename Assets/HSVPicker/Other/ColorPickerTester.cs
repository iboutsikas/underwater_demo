using UnityEngine;

public class ColorPickerTester : MonoBehaviour 
{

    public Renderer the_renderer;
    public ColorPicker picker;

    public Color Color = Color.red;

	// Use this for initialization
	void Start () 
    {
        picker.onValueChanged.AddListener(color =>
        {
            the_renderer.material.color = color;
            Color = color;
        });

		the_renderer.material.color = picker.CurrentColor;

        picker.CurrentColor = Color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
