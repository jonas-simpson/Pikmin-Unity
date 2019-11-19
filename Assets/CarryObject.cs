using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryObject : MonoBehaviour
{
    public CarryTarget[] myTargets;
    public Follower follower;
    public int minPikmin, maxPikmin;
    public bool minReached, maxReached;
    public float minSpeed, maxSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        follower = gameObject.GetComponent<Follower>();
        DisableFollower(); //disable agent when not moving
        myTargets = gameObject.GetComponentsInChildren<CarryTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableFollower()
    {
        follower.Destination();
        follower.AgentState(false);
    }
}
