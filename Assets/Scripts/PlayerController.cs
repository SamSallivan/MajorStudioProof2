using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Unity.Burst.CompilerServices;

public class PlayerController : MonoBehaviour
{
    public bool debugWithMouse;
    GazePoint gazePoint;

    public Camera mainCam;
    public GameObject cursor;
    public GameObject rectCursor;
    public PlayerDecapitate playerDecapitate;

    [Space(25)]

    public float maxDashDistance = 35;
    public bool isDashing;
    public bool dashCharged;
    public float chargeTime;
    public float targetingRadius = 2.5f;
    public GameObject targetedPlatform;

    [Space(25)]

    public GameObject chargeBarUI;
    public GameObject chargeBarBonusUI;
    public Image chargeCircleUI;
    public Image chargeCircleBonusUI;
    public GameObject pieCursor;
    public GameObject eyeIconUI1;
    public GameObject eyeIconUI2;

    public GameObject windVFX;

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
        Vector2 gazePos;
        if (debugWithMouse)
        {
            gazePos = Input.mousePosition;
        }
        else {
            gazePos = gazePoint.Screen;
        }

        Vector3 screenPoint = mainCam.WorldToScreenPoint(rectCursor.transform.position);
        Vector3 curScreenPoint = new Vector3(gazePos.x, gazePos.y, 0.35f);
        Vector3 curPosition = mainCam.ScreenToWorldPoint(curScreenPoint);
        rectCursor.transform.position = curPosition;
        rectCursor.transform.rotation = cursor.transform.rotation;
        //pieCursor.GetComponent<RectTransform>().position = screenPoint;

        if (!isDashing)
        {
            RaycastHit hit;
            RaycastHit hitSphere;
            LayerMask mask = LayerMask.GetMask("Default", "MovingPlatform", "KillZonePlatform");

            if ((!debugWithMouse && TobiiAPI.GetUserPresence().IsUserPresent() && TobiiAPI.GetGazePoint().IsRecent(0.05f)) || (debugWithMouse && !Input.GetMouseButton(0)))
            {
                //cursor.GetComponent<Renderer>().material.SetColor("Color", Color.red);
                eyeIconUI1.SetActive(true);
                eyeIconUI2.SetActive(false);
                //chargeTime = 0;
                maxDashDistance = 35 + chargeTime * 7.5f;

                if (dashCharged)
                {
                    //chargeTime = 0;
                    //maxDashDistance = 35;
                    dashCharged = false;
                    isDashing = true;
                    windVFX.GetComponent<ParticleSystem>().Play();
                }
                else
                {
                    chargeTime = 0;
                    maxDashDistance = 35 + chargeTime * 7.5f;
                    chargeBarBonusUI.SetActive(false);
                    chargeCircleBonusUI.gameObject.SetActive(false);

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(gazePos), out hit, Mathf.Infinity, mask))
                    {
                        cursor.SetActive(true);
                        cursor.transform.position = hit.point;
                        cursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                        rectCursor.SetActive(true);
                        chargeBarUI.GetComponent<RectTransform>().sizeDelta = new Vector2(maxDashDistance / Vector3.Distance(hit.point, transform.position) * 100, 100);
                        chargeCircleUI.fillAmount = maxDashDistance / Vector3.Distance(hit.point, transform.position);
                        if (chargeBarUI.GetComponent<RectTransform>().sizeDelta.x >= 100)
                        {
                            chargeBarUI.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                        }

                        if (Vector3.Distance(hit.point, transform.position) > 50)
                        {
                            rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            chargeBarUI.GetComponent<Image>().color = Color.red;
                            chargeCircleUI.color = Color.red;
                        }
                        else if (Vector3.Distance(hit.point, transform.position) > maxDashDistance)
                        {
                            rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                            chargeBarUI.GetComponent<Image>().color = Color.yellow;
                            chargeCircleUI.color = Color.yellow;
                        }
                        else
                        {
                            rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                            chargeBarUI.GetComponent<Image>().color = Color.green;
                            chargeCircleUI.color = Color.green;
                        }

                    }

                    if (Physics.SphereCast(transform.position, targetingRadius, Camera.main.ScreenPointToRay(gazePos).direction, out hitSphere, Mathf.Infinity, LayerMask.GetMask("MovingPlatform", "TargetablePlatform")))
                    {
                        if (targetedPlatform != null && targetedPlatform != hitSphere.collider.gameObject)
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.white;
                        }
                        targetedPlatform = hitSphere.collider.gameObject;

                        if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > 50)
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        else if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > maxDashDistance)
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        else
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                    else if (targetedPlatform != null)
                    {
                        targetedPlatform.GetComponent<Renderer>().material.color = Color.white;
                        targetedPlatform = null;
                    }
                }
            }
            else if ((!debugWithMouse && TobiiAPI.GetUserPresence().IsUserPresent() && !TobiiAPI.GetGazePoint().IsRecent(0.15f)) || (debugWithMouse && Input.GetMouseButton(0)))
            {
                eyeIconUI1.SetActive(false);
                eyeIconUI2.SetActive(true);

                maxDashDistance = 35 + chargeTime * 7.5f;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(gazePos), out hit, Mathf.Infinity, mask))
                {
                    chargeBarBonusUI.SetActive(true);
                    chargeCircleBonusUI.gameObject.SetActive(true);
                    chargeBarBonusUI.GetComponent<RectTransform>().sizeDelta = new Vector2(maxDashDistance / Vector3.Distance(hit.point, transform.position) * 100, 100);
                    chargeCircleBonusUI.fillAmount = maxDashDistance / Vector3.Distance(hit.point, transform.position);
                    if (chargeBarBonusUI.GetComponent<RectTransform>().sizeDelta.x >= 100)
                    {
                        chargeBarBonusUI.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    }

                    if (Vector3.Distance(hit.point, transform.position) > 50)
                    {
                        rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                        chargeBarUI.GetComponent<Image>().color = Color.red;
                            chargeCircleUI.color = Color.red;
                    }
                    else if (Vector3.Distance(hit.point, transform.position) > maxDashDistance)
                    {
                        rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                        chargeBarUI.GetComponent<Image>().color = Color.yellow;
                            chargeCircleUI.color = Color.yellow;
                    }
                    else
                    {
                        rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                        chargeBarUI.GetComponent<Image>().color = Color.green;
                        chargeCircleUI.color = Color.green;
                    }
                }
                if (Physics.SphereCast(transform.position, targetingRadius, Camera.main.ScreenPointToRay(gazePos).direction, out hitSphere, Mathf.Infinity, LayerMask.GetMask("MovingPlatform", "TargetablePlatform")))
                {
                    if (targetedPlatform != null)
                    {
                        if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > 50)
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        else if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > maxDashDistance)
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.yellow;
                        }
                        else
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.green;
                        }
                    }
                }

                if (targetedPlatform != null)
                {
                    if (Vector3.Distance(transform.position, targetedPlatform.transform.position) <= maxDashDistance)
                    {
                        dashCharged = true;
                    }
                }
                else if(Vector3.Distance(cursor.transform.position, transform.position) <= maxDashDistance)
                {
                    dashCharged = true;
                }
                else
                {
                    if (chargeTime < 2)
                    {
                        chargeTime += Time.deltaTime;
                    }
                }
            }

        }
        else if(isDashing){
            if (targetedPlatform != null)
            {
                GameObject target = targetedPlatform.GetComponent<TargetablePlatform>().landingSpots[1];
                float minDistance = Mathf.Infinity;
                foreach(GameObject spot in targetedPlatform.GetComponent<TargetablePlatform>().landingSpots)
                {
                    if (Vector3.Distance(transform.position, spot.transform.position) < minDistance)
                    {
                        minDistance = Vector3.Distance(transform.position, spot.transform.position);
                        target = spot;
                    }
                }
                transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * 5f);
                if (Vector3.Distance(transform.position, target.transform.position) < 5)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, Time.deltaTime * 10f);

                    windVFX.GetComponent<ParticleSystem>().Stop();
                    if (Quaternion.Angle(transform.rotation, target.transform.rotation) <= 5)
                    {
                        isDashing = false;
                    }
                }
            }
            else
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

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))// && isDashing)
        {
            //transform.SetParent(collision.gameObject.transform);
        }
        /*if (collision.gameObject.layer == LayerMask.NameToLayer("KillZone") || collision.gameObject.layer == LayerMask.NameToLayer("KillZonePlatform"))
        {
            Die();
        }*/
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))// && isDashing)
        {
            //transform.SetParent(collision.gameObject.transform); 

            GameObject target = collision.gameObject.GetComponent<TargetablePlatform>().landingSpots[1];
            float minDistance = Mathf.Infinity;
            foreach (GameObject spot in collision.gameObject.GetComponent<TargetablePlatform>().landingSpots)
            {
                if (Vector3.Distance(transform.position, spot.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(transform.position, spot.transform.position);
                    target = spot;
                }
            }
            transform.position = target.transform.position;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))
        {
            transform.SetParent(null);
            transform.localScale = new Vector3(1,1,1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("KillZone") || other.gameObject.layer == LayerMask.NameToLayer("KillZonePlatform"))
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
