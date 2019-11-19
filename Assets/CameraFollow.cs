using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float h, orbitSpeed, desiredRot, orbitDamp;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject.transform;
        transform.position = player.position;
        desiredRot = transform.eulerAngles.y;
    }
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        Follow();
        GetInput();
        Orbit();
    }

    void Follow()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    bool GetInput()
    {
        h = Input.GetAxis("Orbit");
        if(h != 0)
            return true;
        else
            return false;
    }

    void Orbit()
    {
        transform.Rotate(0.0f,orbitSpeed*h,0.0f);
        /*
        desiredRot = transform.eulerAngles.y + orbitSpeed * Time.deltaTime;
        Quaternion quat = Quaternion.Euler(transform.rotation.x, desiredRot*h, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, Time.deltaTime * orbitDamp);
        */
    }

}
