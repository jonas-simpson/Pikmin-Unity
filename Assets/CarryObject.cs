using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryObject : MonoBehaviour
{
    // public CarryTarget[] myTargets;
    public Follower follower;
    public TaskGroup group;
    public int minPikmin, maxPikmin;
    public bool minReached, maxReached;
    public float minSpeed, maxSpeed;
    public int currentPikmin;
    
    // Start is called before the first frame update
    void Start()
    {
        follower = gameObject.GetComponent<Follower>();
        //group = gameObject.GetComponentInChildren<TaskGroup>();
        DisableFollower(); //disable agent when not moving
        // myTargets = gameObject.GetComponentsInChildren<CarryTarget>();
        // for(int i = 0; i < myTargets.Length; i++)
        // {
        //     myTargets[i].parent = this;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        //currentPikmin = group.myPikmin.Count;
    }

    void DisableFollower()
    {
        follower.Destination();
        follower.AgentState(false);
    }

    public void AddPikmin()
    {
        currentPikmin++;
    }
    public void SubtractPikmin()
    {
        currentPikmin--;
    }
    public bool IsCarrying()
    {
        if(currentPikmin >= minPikmin)
        {
            minReached = true;
            return true;
        }
        else
        {
            minReached = false;
            return false;
        }
    }
}
