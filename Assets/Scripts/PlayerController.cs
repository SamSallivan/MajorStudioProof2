using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public bool debugWithMouse;
    public bool enableChargeBar;
    GazePoint gazePoint;

    public Camera mainCam;
    public GameObject cursor;
    public GameObject rectCursor;
    public PlayerDecapitate playerDecapitate;

    [Space(25)]

    public float dashDistance = 35;
    public float maxBaseDashDistance = 35;
    public float maxChargedDashDistance = 50;
    public float maxChargedTime = 2;
    public bool isDashing;
    public bool dashCharged;
    public float chargeTime;
    public float targetingRadius = 2.5f;
    public GameObject targetedPlatform;

    [Space(25)]

    public GameObject chargeBarUI;
    public GameObject chargeBarBonusUI;
    public GameObject chargeBarBaseUI;
    public Image chargeCircleUI;
    public Image chargeCircleBonusUI;
    public GameObject pieCursor;
    public GameObject eyeIconUI1;
    public GameObject eyeIconUI2;

    public GameObject windVFX;

    public Volume volume;
    public Bloom bloom;
    public ChromaticAberration ca;
    //public ColorGrading cg;
    public Vignette vg;

    public AudioClip chargeSFX;
    public AudioClip chargedSFX;
    public AudioClip shotSFX;
    public AudioClip landSFX;
    public AudioClip deathFX;
    public AudioSource audioSource;

    public float totalTime = 60;
    public float timer;
    public TMP_Text timeUI;

    void Start()
    {
        playerDecapitate = GetComponentInChildren<PlayerDecapitate>(true);

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out ca);
        //volume.profile.TryGet(out cg);
        volume.profile.TryGet(out vg);

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!TimeManager.instance.timeIsStopped)
        {
            float chargeRate = (maxChargedDashDistance - maxBaseDashDistance) / maxChargedTime;

            gazePoint = TobiiAPI.GetGazePoint();
            Vector2 gazePos;
            if (debugWithMouse)
            {
                gazePos = Input.mousePosition;
            }
            else
            {
                gazePos = gazePoint.Screen;
            }

            if (!isDashing)
            {
                RaycastHit hit;
                RaycastHit hitSphere;
                LayerMask mask = LayerMask.GetMask("Default", "MovingPlatform", "KillZonePlatform");

                if ((!debugWithMouse && TobiiAPI.GetUserPresence().IsUserPresent() && TobiiAPI.GetGazePoint().IsRecent(0.05f)) || (debugWithMouse && !Input.GetMouseButton(0)))
                {

                    if (targetedPlatform == null)
                    {
                        Vector3 curScreenPoint = new Vector3(gazePos.x, gazePos.y, 0.35f);
                        Vector3 curPosition = mainCam.ScreenToWorldPoint(curScreenPoint);
                        rectCursor.transform.position = curPosition;
                        rectCursor.transform.rotation = cursor.transform.rotation;
                    }
                    else
                    {
                        Vector3 screenPoint = Vector3.MoveTowards(mainCam.WorldToScreenPoint(rectCursor.transform.position), mainCam.WorldToScreenPoint(targetedPlatform.transform.position), 25);
                        Vector3 curScreenPoint = new Vector3(screenPoint.x, screenPoint.y, 0.35f);
                        Vector3 curPosition = mainCam.ScreenToWorldPoint(curScreenPoint);
                        rectCursor.transform.position = curPosition;
                        rectCursor.transform.localRotation = Quaternion.Euler(90, 90, 90);
                    }

                    eyeIconUI1.SetActive(true);
                    eyeIconUI2.SetActive(false);

                    dashDistance = maxBaseDashDistance + chargeTime * chargeRate;

                    if (dashCharged)
                    {
                        //chargeTime = 0;
                        //maxBaseDashDistance = 35;
                        dashCharged = false;
                        isDashing = true;
                        windVFX.GetComponent<ParticleSystem>().Play();
                        audioSource.PlayOneShot(shotSFX);
                    }
                    else
                    {
                        chargeTime = 0;
                        dashDistance = maxBaseDashDistance + chargeTime * chargeRate;
                        chargeBarBonusUI.SetActive(false);
                        chargeCircleBonusUI.gameObject.SetActive(false);

                        if (Physics.SphereCast(transform.position, targetingRadius, Camera.main.ScreenPointToRay(gazePos).direction, out hitSphere, Mathf.Infinity, LayerMask.GetMask("MovingPlatform", "TargetablePlatform")))
                        {
                            if (targetedPlatform != null && targetedPlatform != hitSphere.collider.gameObject)
                            {
                                targetedPlatform.GetComponent<Renderer>().material.color = Color.white;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
                            }
                            targetedPlatform = hitSphere.collider.gameObject;

                            chargeCircleUI.fillAmount = maxBaseDashDistance / Vector3.Distance(targetedPlatform.transform.position, transform.position);

                            if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > maxChargedDashDistance)
                            {
                                //targetedPlatform.GetComponent<Renderer>().material.color = Color.red;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
                                chargeCircleUI.color = Color.red;
                            }
                            else if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > maxBaseDashDistance)
                            {
                                //targetedPlatform.GetComponent<Renderer>().material.color = Color.yellow;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
                                chargeCircleUI.color = Color.yellow;
                            }
                            else
                            {
                                //targetedPlatform.GetComponent<Renderer>().material.color = Color.green;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
                                chargeCircleUI.color = Color.green;
                            }
                        }
                        else if (targetedPlatform != null)
                        {
                            targetedPlatform.GetComponent<Renderer>().material.color = Color.white;
                            targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
                            targetedPlatform = null;
                        }

                        if (targetedPlatform == null && Physics.Raycast(Camera.main.ScreenPointToRay(gazePos), out hit, Mathf.Infinity, mask))
                        {
                            cursor.SetActive(true);
                            cursor.transform.position = hit.point;
                            cursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                            rectCursor.SetActive(true);
                            chargeBarUI.GetComponent<RectTransform>().sizeDelta = new Vector2(dashDistance / Vector3.Distance(hit.point, transform.position) * 100, 100);
                            chargeCircleUI.fillAmount = maxBaseDashDistance / Vector3.Distance(hit.point, transform.position);
                            if (chargeBarUI.GetComponent<RectTransform>().sizeDelta.x >= 100)
                            {
                                chargeBarUI.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                            }

                            if (Vector3.Distance(hit.point, transform.position) > maxChargedDashDistance)
                            {
                                //rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                                chargeBarUI.GetComponent<Image>().color = Color.red;
                                chargeCircleUI.color = Color.red;
                            }
                            else if (Vector3.Distance(hit.point, transform.position) > maxBaseDashDistance)
                            {
                                //rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                                chargeBarUI.GetComponent<Image>().color = Color.yellow;
                                chargeCircleUI.color = Color.yellow;
                            }
                            else
                            {
                                //rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                                chargeBarUI.GetComponent<Image>().color = Color.green;
                                chargeCircleUI.color = Color.green;
                            }

                        }

                    }
                }
                else if ((!debugWithMouse && TobiiAPI.GetUserPresence().IsUserPresent() && !TobiiAPI.GetGazePoint().IsRecent(0.15f)) || (debugWithMouse && Input.GetMouseButton(0)))
                {

                    if (targetedPlatform != null)
                    {
                        Vector3 screenPoint = Vector3.MoveTowards(mainCam.WorldToScreenPoint(rectCursor.transform.position), mainCam.WorldToScreenPoint(targetedPlatform.transform.position), 25);
                        Vector3 curScreenPoint = new Vector3(screenPoint.x, screenPoint.y, 0.35f);
                        Vector3 curPosition = mainCam.ScreenToWorldPoint(curScreenPoint);
                        rectCursor.transform.position = curPosition;
                        rectCursor.transform.rotation = cursor.transform.rotation;
                    }

                    eyeIconUI1.SetActive(false);
                    eyeIconUI2.SetActive(true);

                    dashDistance = maxBaseDashDistance + chargeTime * chargeRate;
                    if (Physics.SphereCast(transform.position, targetingRadius, Camera.main.ScreenPointToRay(gazePos).direction, out hitSphere, Mathf.Infinity, LayerMask.GetMask("MovingPlatform", "TargetablePlatform")))
                    {
                        if (targetedPlatform != null)
                        {
                            chargeCircleBonusUI.gameObject.SetActive(true);
                            chargeCircleBonusUI.fillAmount = dashDistance / Vector3.Distance(targetedPlatform.transform.position, transform.position);
                            if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > maxChargedDashDistance)
                            {
                                //targetedPlatform.GetComponent<Renderer>().material.color = Color.red;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
                            }
                            else if (Vector3.Distance(transform.position, targetedPlatform.transform.position) > maxBaseDashDistance)
                            {
                                //targetedPlatform.GetComponent<Renderer>().material.color = Color.yellow;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
                            }
                            else
                            {
                                //targetedPlatform.GetComponent<Renderer>().material.color = Color.green;
                                targetedPlatform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
                            }
                        }
                    }

                    if (targetedPlatform == null && Physics.Raycast(Camera.main.ScreenPointToRay(gazePos), out hit, Mathf.Infinity, mask))
                    {
                        chargeBarBonusUI.SetActive(true);
                        chargeCircleBonusUI.gameObject.SetActive(true);
                        chargeBarBonusUI.GetComponent<RectTransform>().sizeDelta = new Vector2(dashDistance / Vector3.Distance(hit.point, transform.position) * 100, 100);
                        chargeCircleBonusUI.fillAmount = dashDistance / Vector3.Distance(hit.point, transform.position);
                        if (chargeBarBonusUI.GetComponent<RectTransform>().sizeDelta.x >= 100)
                        {
                            chargeBarBonusUI.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                        }

                        if (Vector3.Distance(hit.point, transform.position) > maxChargedDashDistance)
                        {
                            //rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            chargeBarUI.GetComponent<Image>().color = Color.red;
                            chargeCircleUI.color = Color.red;
                        }
                        else if (Vector3.Distance(hit.point, transform.position) > maxBaseDashDistance)
                        {
                            //rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                            chargeBarUI.GetComponent<Image>().color = Color.yellow;
                            chargeCircleUI.color = Color.yellow;
                        }
                        else
                        {
                            //rectCursor.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                            chargeBarUI.GetComponent<Image>().color = Color.green;
                            chargeCircleUI.color = Color.green;
                        }
                    }

                    if (targetedPlatform != null)
                    {
                        if (Vector3.Distance(transform.position, targetedPlatform.transform.position) <= dashDistance && !dashCharged)
                        {
                            dashCharged = true;
                            audioSource.Stop();
                            audioSource.PlayOneShot(chargedSFX);
                        }
                        else
                        {
                            if (chargeTime < maxChargedTime && !dashCharged)
                            {
                                chargeTime += Time.deltaTime;
                                if (!audioSource.isPlaying)
                                {
                                    audioSource.PlayOneShot(chargeSFX);
                                }
                            }
                        }
                    }
                    else if (Vector3.Distance(cursor.transform.position, transform.position) <= dashDistance && !dashCharged)
                    {
                        dashCharged = true;
                        audioSource.Stop();
                        audioSource.PlayOneShot(chargedSFX);
                    }
                    else
                    {
                        if (chargeTime < maxChargedTime && !dashCharged)
                        {
                            chargeTime += Time.deltaTime;
                            if (!audioSource.isPlaying)
                            {
                                audioSource.PlayOneShot(chargeSFX);
                            }
                        }
                    }
                }

            }
            else if (isDashing)
            {
                if (targetedPlatform != null)
                {
                    GameObject target = targetedPlatform.GetComponent<TargetablePlatform>().landingSpots[0];
                    float minDistance = Mathf.Infinity;
                    foreach (GameObject spot in targetedPlatform.GetComponent<TargetablePlatform>().landingSpots)
                    {
                        if (Vector3.Distance(transform.position, spot.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(transform.position, spot.transform.position);
                            target = spot;
                        }
                    }
                    transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * 5f);
                    if (Vector3.Distance(transform.position, target.transform.position) < 1)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, Time.deltaTime * 10f);

                        windVFX.GetComponent<ParticleSystem>().Stop();
                        if (Quaternion.Angle(transform.rotation, target.transform.rotation) <= 5)
                        {
                            isDashing = false;
                            audioSource.PlayOneShot(landSFX);
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
                        audioSource.PlayOneShot(landSFX);
                    }
                }
            }

            if (!enableChargeBar)
            {
                chargeBarUI.SetActive(false);
                chargeBarBonusUI.SetActive(false);
                chargeBarBaseUI.SetActive(false);

            }


            //Converts time in second into time as in Min : Sec : Millisec.
            timer = totalTime - Time.timeSinceLevelLoad;
            if (timer <= 0)
            {
                Die();
            }

            float minutes = Mathf.Floor(timer / 60);
            float seconds = Mathf.Floor(timer % 60);
            float milliseconds = Mathf.Floor((timer * 1000) % 1000);

            string min = minutes.ToString();
            string sec = seconds.ToString();
            string milli = milliseconds.ToString();

            //Adds 0 digits before the string if its small enough.
            if (minutes < 10)
            {
                min = "0" + minutes.ToString();
            }
            if (seconds < 10)
            {
                sec = "0" + Mathf.RoundToInt(seconds).ToString();
            }
            if (milliseconds < 10)
            {
                milli = "00" + Mathf.RoundToInt(milliseconds).ToString();
            }
            else if (milliseconds < 100)
            {
                milli = "0" + Mathf.RoundToInt(milliseconds).ToString();
            }

            timeUI.text = min + ":" + sec + ":" + milli;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform") || collision.gameObject.layer == LayerMask.NameToLayer("TargetablePlatform"))
        {
            totalTime += collision.gameObject.GetComponent<TargetablePlatform>().bonusTime;
            collision.gameObject.GetComponent<TargetablePlatform>().bonusTime = 0;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("KillZone") || collision.gameObject.layer == LayerMask.NameToLayer("KillZonePlatform"))
        {
            Die();
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))// && isDashing)
        {
            //transform.SetParent(collision.gameObject.transform); 

            GameObject target = collision.gameObject.GetComponent<TargetablePlatform>().landingSpots[0];
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

    private void OnTriggerStay(Collider other) {
        
    }

    public void Die()
    {
        bloom.intensity.value = 100;
        ca.intensity.value = 10;
        //cg.mixerGreenOutRedIn.value = -75;
        //vg.intensity.value = 0.3f;

        playerDecapitate.gameObject.SetActive(true);
        playerDecapitate.Decapitate(transform, transform.up);
        base.gameObject.SetActive(false);
    }
}
