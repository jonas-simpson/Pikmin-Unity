using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    //Look at script for world space UI elements
    //to face the camera.

    public Transform target;
    public float damping;

    void Start()
    {
        target = FindObjectOfType<Camera>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(target, Vector3.up);
        // Vector3 rot = new Vector3(transform.eulerAngles.x, 0, 0);
        // transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(rot));

        Vector3 lookPos = target.position - transform.position;
        lookPos.x = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}
