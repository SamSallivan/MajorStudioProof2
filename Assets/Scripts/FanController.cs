using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour
{
    public float speed = 1;
    //Rotates the fan in Title screen.
	private void Update()
	{
        transform.Rotate(Vector3.up * (360f * Time.deltaTime * speed));
    }
}
