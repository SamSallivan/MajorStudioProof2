using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEdgeDetect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.0) Debug.Log("I am left of the camera's view.");
        if (1.0 < pos.x) Debug.Log("I am right of the camera's view.");
        if (pos.y < 0.0) Debug.Log("I am below the camera's view.");
        if (1.0 < pos.y) Debug.Log("I am above the camera's view.");

    }
}
