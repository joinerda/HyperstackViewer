using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour
{
    float elapsedTime = 0.0f;
    float period = 5.0f;
    bool wiggleLeft = true;
    float maxAngle = 15.0f;

    Quaternion initial_rotation;
    // Start is called before the first frame update
    void Start()
    {
        initial_rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = initial_rotation;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.Rotate(Vector3.right, 180, Space.World);
            initial_rotation  = transform.rotation;
        }
        elapsedTime += Time.deltaTime;
        if(elapsedTime>period)
		{
            elapsedTime = 0.0f;
            wiggleLeft = !wiggleLeft;
		}
        float angle = 2*Mathf.PI*elapsedTime/period;
        if(wiggleLeft)
		{
            transform.Rotate(Vector3.up, maxAngle * Mathf.Sin(angle), Space.World);
        } else
		{
            transform.Rotate(Vector3.right, maxAngle * Mathf.Sin(angle), Space.World);
        }
    }
}
