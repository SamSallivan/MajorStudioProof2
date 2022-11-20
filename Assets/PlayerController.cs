using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using Tobii.Gaming.Examples.ActionGame;

public class PlayerController : MonoBehaviour
{
    GazePoint gazePoint;
    public GameObject cursor;
    public bool isDashing;
    public bool dashCharged;
    public GameObject windVFX;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        gazePoint = TobiiAPI.GetGazePoint();

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(cursor.transform.position);
        Vector3 curScreenPoint = new Vector3(gazePoint.Screen.x, gazePoint.Screen.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        //cursor.transform.position = curPosition;

        if (!isDashing)
        {
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Default");

            if (Physics.Raycast(Camera.main.ScreenPointToRay(gazePoint.Screen), out hit, 1000f))
            {
                if (TobiiAPI.GetUserPresence().IsUserPresent() && TobiiAPI.GetGazePoint().IsRecent(0.05f))
                {
                    cursor.GetComponent<Renderer>().material.SetColor("Color", Color.red);

                    if (dashCharged)
                    {
                        dashCharged = false;
                        isDashing = true;
                        windVFX.GetComponent<ParticleSystem>().Play();
                        GetComponent<LineRenderer>().enabled = false;
                    }
                    else
                    {
                        cursor.transform.position = hit.point;
                        cursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        GetComponent<LineRenderer>().enabled = true;
                        GetComponent<LineRenderer>().SetPosition(0, Camera.main.transform.position + Camera.main.transform.forward * 10);
                        GetComponent<LineRenderer>().SetPosition(1, cursor.transform.position);
                    }
                }
                else
                {
                    cursor.GetComponent<Renderer>().material.SetColor("Color", Color.blue);
                }
            }
        }
        else if(isDashing){
            transform.position = Vector3.Lerp(transform.position, cursor.transform.position, Time.deltaTime * 5f);
            if(Vector3.Distance(transform.position, cursor.transform.position) < 5)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, cursor.transform.rotation, Time.deltaTime * 10f);

                windVFX.GetComponent<ParticleSystem>().Stop();
            }
            if (transform.rotation == cursor.transform.rotation)
            {
                isDashing = false;
            }
        }

        if (TobiiAPI.GetUserPresence().IsUserPresent() && !TobiiAPI.GetGazePoint().IsRecent(0.15f))
        {
            if (!isDashing)
            {
                dashCharged = true;
            }
            //transform.position = cursor.transform.position;
            //transform.rotation = cursor.transform.rotation;
        }

    }
}
