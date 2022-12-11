using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetablePlatform : MonoBehaviour
{
    public List<GameObject> landingSpots;
    public float bonusTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (bonusTime > 0)
        {
            GetComponent<Renderer>().material.SetColor("Color_matrix", new Color(0, 27, 191, 255) * 2073741824); ;
        }
        else
        {
            GetComponent<Renderer>().material.SetColor("Color_matrix", Color.white * 2073741824);
        }
    }
}
