using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : TargetablePlatform
{
    public List<GameObject> targets;
    public GameObject platform;
    public int index;
    public float speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(platform.transform.position, targets[index].transform.position) >= 1)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, targets[index].transform.position, speed/100);
        }
        else
        {
            index++;
            if (index > targets.Count - 1)
            {
                index = 0;
            }
        }
    }


/*    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }
    void OnTriggerExit(Collider collision)
    {
        Debug.Log("????");
        if (collision.gameObject.layer == 3)
        {
            collision.gameObject.transform.SetParent(null);
        }
    }*/
}
