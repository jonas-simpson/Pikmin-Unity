using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PikminBase : MonoBehaviour
{
    //Pikmin Components ---------------------
    public Follower follower;
    public PikminHelper helper;
    public PikminPlayer player;
    //public PikminCarry carry;
    //public PikminAttack attack;
    public CustomGravity grav;
    public Animator machine;    //Pikmin State Machine
    
    //Physics Components --------------------
    public Rigidbody rb;
    public Collider col;

    //External Objects ----------------------
    public Transform captain, pointer;
    public GameObject target;
    public PikminManager manager;
    public GameObject[] tasks; //all tasks in the scene

    //Variables
    public bool withPlayer, held, grounded, thrown, active, hasTarget, callOnGround, isCarrying;  //machine variables
    public int order;  //order in group if with player
    public float distance, maxDistance, dismissTime, detectionDistance;
    Coroutine stop;


    private void Start()
    {
        Initialize();
        //hasTarget = true;
        //withPlayer = true;
        //target = captain.gameObject;
        rb.useGravity = false;
        grav.gravityScale = 1.0f;
        tasks = GameObject.FindGameObjectsWithTag("Task");
    }

    private void Initialize()
    {
        follower = gameObject.GetComponent<Follower>();
        helper = gameObject.GetComponent<PikminHelper>();
        player = gameObject.GetComponent<PikminPlayer>();
        grav = gameObject.GetComponent<CustomGravity>();
        machine = gameObject.GetComponent<Animator>();

        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponentInChildren<Collider>();

        manager = FindObjectOfType<PikminManager>();
        captain = FindObjectOfType<PlayerController>().gameObject.transform;
        pointer = FindObjectOfType<PointerController>().gameObject.transform;

        SetWithPlayer(false);
        SetHeld(false);
        SetHasTarget(false);
        grounded = true;
    }

    private void Update()
    {
        if(hasTarget)
        {
            distance = helper.UpdateDistance(target.transform, this.transform);
            if(withPlayer)
                follower.StoppingDistance(order/16+2);
        }
        if(held)
            Held();
        if(!withPlayer && grounded && !active) //if idle, grounded, and not currently working
        {
            GameObject newTask = SearchForTasks();
            if(newTask != null)
            {
                SetHasTarget(true);
                SetTarget(newTask);
                follower.StoppingDistance(0.0f);
            }
        }
    }

    public void SetWithPlayer(bool _state)
    {
        withPlayer = _state;
        machine.SetBool("With Player", _state);
    }

    public void SetHeld(bool _state)
    {
        held = _state;
        machine.SetBool("Hold", _state);
    }

    public void SetHasTarget(bool _state)
    {
        hasTarget = _state;
        machine.SetBool("Has Target", _state);
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
        follower.AgentState(true);
        follower.Destination(_target);
    }

    public void SetIsCarrying(bool _state)
    {
        isCarrying = _state;
        machine.SetBool("Carrying", _state);
    }
    public bool GetWithPlayer()
    {
        return withPlayer;
    }

    public bool GetHeld()
    {
        return held;
    }

    public void Call()
    {
        if(!withPlayer)
        {
            if(stop != null)
                StopCoroutine(stop);
            if(grounded)
            {
                //player is calling pikmin
                SetWithPlayer(true);
                follower.AgentState(true);
                SetHasTarget(true);
                SetTarget(captain.gameObject);
                follower.Destination(target);
                manager.Call(this);
            }
            else
            {
                callOnGround = true;
            }
        }
    }

    public void Dismiss(GameObject _target)
    {
        SetWithPlayer(false);
        SetHasTarget(false);
        follower.Destination(_target);
        Debug.Log(_target.transform.position);
        follower.StoppingDistance(0.0f);
        stop = StartCoroutine(follower.StopAfterTime(dismissTime));
        //Debug.Log("Dismissed! " + gameObject.name);
    }

    public void Throw()
    {
        SetHeld(false);
        SetWithPlayer(false);
        SetHasTarget(false);
        helper.SetTrail(true);
        machine.SetTrigger("Throw");
        helper.SetPhysics(true);
        player.NewThrow(rb, grav, transform.position, captain.transform.position, pointer.position);
    }

    public void SetGrav(float f)
    {
        grav.gravityScale = f;
    }

    public void Held()
    {
        SetHeld(true);
        follower.AgentState(false);
        //helper.SetPhysics(false);
        grounded = false;
        SetGrav(0);
        Vector3 playerVector = helper.GetPlayerVector(captain);
        gameObject.transform.position = captain.position + playerVector;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Ground")
        {
            //pikmin is grounded
            rb.velocity.Set(0,0,0);
            rb.drag = 2f;
            grounded = true;
            machine.ResetTrigger("Throw");
            machine.SetBool("Throw", false);
            helper.SetTrail(false);
            if(callOnGround)
            {
                callOnGround = false;
                Call();
            }
        }
    }

    private GameObject SearchForTasks()
    {
        List<GameObject> tasksInRange = new List<GameObject>();
        foreach(GameObject task in tasks)
        {
            if(Vector3.Distance(transform.position, task.transform.position) <= detectionDistance)
            {
                //if a task is in range, add it to tasksInRange
                tasksInRange.Add(task);
            }
        }
        if(tasksInRange.Count > 0)
        {
            if(tasksInRange.Count > 1)
            {
                tasksInRange.Sort((t1,t2) =>
                Vector3.Distance(transform.position, t1.transform.position)
                .CompareTo(Vector3.Distance(transform.position, t2.transform.position)));
            }
            return tasksInRange[0];
        }
        else
            return null;
    }
}
