using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class FollowMe : MonoBehaviour
{
    private enum MovingStatus
    {
        Moving,
        Idle
    };


    private TouchControlls touchControls;
    private Camera camera;
    private bool following = false;

    private Vector3 currentTarget;
    private MovingStatus currentStatus = MovingStatus.Idle;

    public float DepthBias = 0.0f;
    public float Speed = 5.0f;

    // Start is called before the first frame update
    void Awake()
    {
        touchControls = new TouchControlls();
        camera = Camera.main;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        TouchSimulation.Enable();
#endif
        if (touchControls == null)
            return;

        touchControls.Enable();
        touchControls.Touch.TouchPress.started += On_TouchPressStarted;
        touchControls.Touch.TouchPress.canceled += On_TouchPressCanceled;
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        TouchSimulation.Disable();
#endif
        if (touchControls == null)
            return;

        touchControls.Disable();
        touchControls.Touch.TouchPress.started -= On_TouchPressStarted;
        touchControls.Touch.TouchPress.canceled -= On_TouchPressCanceled;
    }

    private void Start()
    {
    }

    private void On_TouchPressStarted(InputAction.CallbackContext ctx)
    {
        var touchPosition = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        var screenPosition = new Vector3(touchPosition.x, touchPosition.y, transform.position.z + DepthBias);
        var worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = transform.position.z;

        currentTarget = worldPosition;

        currentStatus = MovingStatus.Moving;
        following = true;
    }

    private void On_TouchPressCanceled(InputAction.CallbackContext ctx)
    {
        following = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentStatus != MovingStatus.Moving)
            return;

        var touchPosition = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
        var screenPosition = new Vector3(touchPosition.x, touchPosition.y, transform.position.z + DepthBias);
        var worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = transform.position.z;

        currentTarget = worldPosition;

        float step = Speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);
        if (Vector3.Distance(transform.position, currentTarget) < 0.001f && !following)
            currentStatus = MovingStatus.Idle;
    }
}
