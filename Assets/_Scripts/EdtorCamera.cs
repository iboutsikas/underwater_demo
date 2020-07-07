using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EdtorCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float HorizontalSensitivity = 1.0f;
    public float VerticalSensitivity = 1.0f;
    public float MovementSpeed = 10.0f;
    public bool FlipY = true;
    public bool FlipX = false;

    [Header("Keybinds")]
    public KeyCode Forwards = KeyCode.W;
    public KeyCode Backwards = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Up = KeyCode.Space;
    public KeyCode Down = KeyCode.X;
    public KeyCode CameraLock = KeyCode.Mouse2;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor && Application.isPlaying)
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }

    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(CameraLock))
        {
            DoMouseInput();
        }
        DoKeyboardInput();
    }
#endif

    void DoKeyboardInput()
    {
        Vector3 translation = Vector3.zero;

        if (Input.GetKey(Forwards))
        {
            translation += transform.forward* MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(Backwards))
        {
            translation -= transform.forward * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(Left))
        {
            translation -= transform.right * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(Right))
        { 
            translation += transform.right * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(Up))
        {
            translation += transform.up * MovementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(Down))
        {
            translation -= transform.up * MovementSpeed * Time.deltaTime;
        }

        transform.Translate(translation);
    }


    void DoMouseInput()
    {

        float horizontalRotation = HorizontalSensitivity * Input.GetAxis("Mouse X");
        float verticalRotation = VerticalSensitivity * Input.GetAxis("Mouse Y");

        if (FlipY)
        {
            horizontalRotation *= -1.0f;
        }

        transform.Rotate(Vector3.up, horizontalRotation, Space.World);

        float rotationAngle = Mathf.Clamp(verticalRotation, -85.0f, 85.0f);
        if (FlipX)
        {
            rotationAngle *= -1.0f;
        }
        transform.Rotate(transform.right, rotationAngle, Space.World);
    }
}
