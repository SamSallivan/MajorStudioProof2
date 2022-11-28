using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


//Enabled when player dies
//Controlls the game restart function
public class PlayerDecapitate : MonoBehaviour
{
    private PlayerController player;

    private Transform t;

    private Rigidbody rb;


    private void Awake()
    {
        t = base.transform;
        rb = GetComponent<Rigidbody>();
        player = GetComponentInParent<PlayerController>();
    }

    //restarts game on input
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

    //separates the camera from player transform
    public void Decapitate(Transform pos, Vector3 dir)
    {
        if ((bool)transform.parent)
        {
            transform.SetParent(null);
        }
        transform.SetPositionAndRotation(pos.position, pos.rotation);
        rb.AddForce(dir * 5f, ForceMode.Impulse);
        rb.AddTorque(Vector3.one * 10f, ForceMode.Impulse);
    }
}/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    GazePoint gazePoint;
    public GameObject cursor;
    public GameObject rectCursor;
    public bool isDashing;
    public bool dashCharged;
    public GameObject windVFX;
    public float chargeTime;
    public float maxDashDistance = 35;
    public GameObject chargeBarUI;

    public PlayerDecapitate playerDecapitate;

    public PostProcessVolume volume;
    public Bloom bloom;
    public ChromaticAberration ca;
    public ColorGrading cg;
    public Vignette vg;

    void Start()
    {
        playerDecapitate = GetComponentInChildren<PlayerDecapitate>(true);

        volume = FindObjectOfType<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out ca);
        volume.profile.TryGetSettings(out cg);
        volume.profile.TryGetSettings(out vg);
    }

    // Update is called once per frame
    void Update()
    {
        gazePoint = TobiiAPI.GetGazePoint();

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(rectCursor.transform.position);
        Vector3 curScreenPoint = new Vector3(gazePoint.Screen.x, gazePoint.Screen.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        rectCursor.transform.position = curPosition;
        //rectCursor.transform.rotation = cursor.transform.rotation;

        if (!isDashing)
        {
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Default");

            if (Physics.Raycast(Camera.main.ScreenPointToRay(gazePoint.Screen), out hit, Mathf.Infinity))
            {
                cursor.SetActive(true);
                //rectCursor.SetActive(true); 
                if (TobiiAPI.GetUserPresence().IsUserPresent() && TobiiAPI.GetGazePoint().IsRecent(0.05f))
                {
                    //cursor.GetComponent<Renderer>().material.SetColor("Color", Color.red);

                    if (dashCharged)
                    {
                        chargeTime = 0;
                        dashCharged = false;
                        isDashing = true;
                        windVFX.GetComponent<ParticleSystem>().Play();
                        GetComponent<LineRenderer>().enabled = false;
                    }
                    else
                    {
                        cursor.transform.position = hit.point;
                        cursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        //GetComponent<LineRenderer>().enabled = true;
                        //GetComponent<LineRenderer>().SetPosition(0, Camera.main.transform.position + Camera.main.transform.forward * 10);
                        //GetComponent<LineRenderer>().SetPosition(1, cursor.transform.position);
                    }
                }
                *//* else
                 {
                     cursor.GetComponent<Renderer>().material.SetColor("Color", Color.blue);
                 }*//*
                else if (TobiiAPI.GetUserPresence().IsUserPresent() && !TobiiAPI.GetGazePoint().IsRecent(0.15f))
                {
                    maxDashDistance = 35 + chargeTime * 3;

                    if (Vector3.Distance(hit.point, transform.position) > maxDashDistance)
                    {
                        chargeTime += Time.deltaTime;
                        chargeBarUI.GetComponent<RectTransform>().sizeDelta = new Vector2(maxDashDistance / Vector3.Distance(hit.point, transform.position) * 100, 100);
                    }
                    else
                    {
                        dashCharged = true;
                    }
                }
            }
            else
            {
                //cursor.SetActive(false);
                //rectCursor.SetActive(false);
            }
        }
        else if (isDashing)
        {
            transform.position = Vector3.Lerp(transform.position, cursor.transform.position, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.position, cursor.transform.position) < 5)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, cursor.transform.rotation, Time.deltaTime * 10f);

                windVFX.GetComponent<ParticleSystem>().Stop();
            }
            if (Quaternion.Angle(transform.rotation, cursor.transform.rotation) <= 5)
            {
                isDashing = false;
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))
        {
            transform.SetParent(collision.gameObject.transform);
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("KillZone"))
        {
            Die();
        }
    }
    public void Die()
    {
        bloom.intensity.value = 10;
        ca.intensity.value = 10;
        cg.mixerGreenOutRedIn.value = -75;
        vg.intensity.value = 0.3f;

        playerDecapitate.gameObject.SetActive(true);
        playerDecapitate.Decapitate(transform, transform.up);
        base.gameObject.SetActive(false);
    }
}
*/