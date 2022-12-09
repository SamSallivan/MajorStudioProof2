using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


//Manages UI in Pause Screen.
public class MenuManager : MonoBehaviour
{
    //Makes it class static so it's easy to access.
    public static MenuManager instance;

    //Pause screen canvas.
    public GameObject canvas;

    public string nextScene;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!canvas.activeInHierarchy)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    public void Pause()
    {
        canvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        TimeManager.instance.Stop();
    }

    public void Resume()
    {
        canvas.SetActive(false);
        TimeManager.instance.Play();
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Next()
    {
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

    //Gets IKTarget's position in screen, 
    //Gets Mouse position in world,
    //Sets IKTarget's position to mouse position in world,
    //So it only moves on the plane parallel to screen.
}
