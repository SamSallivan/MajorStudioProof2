using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetablePlatform : MonoBehaviour
{
    public List<GameObject> landingSpots;
    public float bonusTime;
    public GameObject matrix;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (matrix != null)
        {
            if (bonusTime > 0)
            {
                matrix.SetActive(true);
            }
            else
            {
                matrix.SetActive(false);
            }
        }
    }
}
