using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class IPositionedInAR : MonoBehaviour
{
    public void PositionOnARTrigger(ARTrackedImage image) {
        transform.position = image.transform.position;
        //transform.localScale = new Vector3(image.size.x, 1f, image.size.y);
    }
}
