using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        // Check if Camera.main is not null
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up); 
        }
        else
        {
            Debug.LogWarning("Main Camera not found. Make sure there is a camera tagged as 'MainCamera'.");
        }
    }
}
