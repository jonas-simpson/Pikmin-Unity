using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whistle : MonoBehaviour
{
    public GameObject whistle;
    public float max, scale, speed, dampTime, startTime, currentTime, maxTime = 1;

    void Start()
    {
        whistle = GameObject.Find("Whistle");
        whistle.SetActive(false);
    }

    void Update()
    {
        if(GetInput())
        {
            currentTime = Time.time;
            Scale();
        }
        else
        {
            whistle.SetActive(false);
        }
    }

    bool GetInput()
    {
        if(Input.GetMouseButtonDown(1))
        {
            whistle.SetActive(true);
            startTime = Time.time;
            return true;
        }
        else if(Input.GetMouseButtonUp(1) || maxTime <= currentTime - startTime)
        {
            whistle.transform.localScale = new Vector3(1,1,1);
            scale = 1;
            whistle.SetActive(false);
            return false;
        }
        else if(Input.GetMouseButton(1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Scale()
    {
        if(scale < max-0.01f)
        {
            scale = Mathf.SmoothDamp(scale, max, ref speed, dampTime);
            whistle.transform.localScale = new Vector3(scale,scale,scale);
        }
    }
}
