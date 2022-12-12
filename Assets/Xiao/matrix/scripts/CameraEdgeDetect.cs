using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEdgeDetect : MonoBehaviour
{
    public Animator cameraParent;
    public PlayerController myPlayerController;
    Vector3 oldEulerAngles;

    void Start()
    {
        oldEulerAngles = myPlayerController.gameObject.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayerController.isDashing)
        {
            cameraParent.enabled = false;
            return;
        }

        if (oldEulerAngles == myPlayerController.transform.rotation.eulerAngles && myPlayerController.isDashing == false)
        {
            //NO ROTATION
            cameraParent.enabled = true;
        }
        else
        {
            oldEulerAngles = myPlayerController.transform.rotation.eulerAngles;
        }
        

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.0)
        {
            Debug.Log("I am left of the camera's view.");
            cameraParent.SetBool("LookLeft", true);
        }
        else cameraParent.SetBool("LookLeft", false);

        if (1.0 < pos.x)
        {
            Debug.Log("I am right of the camera's view.");
            cameraParent.SetBool("LookRight", true);
        }
        else cameraParent.SetBool("LookRight", false);

        if (pos.y < 0.0)
        {
            Debug.Log("I am below the camera's view.");
            cameraParent.SetBool("LookDown", true);
        }
        else cameraParent.SetBool("LookDown", false);

        if (1.0 < pos.y) 
        { 
            Debug.Log("I am above the camera's view.");
            cameraParent.SetBool("LookUp", true);
        }
        else cameraParent.SetBool("LookUp", false);

    }
}
