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
    public ObstacleSwitch obstacleSwitch;
    
    //Physics Components --------------------
    public Rigidbody rb;
    public Collider col;

    //External Objects ----------------------
    public Transform captain, pointer;
    public GameObject target;
    public PikminManager manager;
    //public GameObject[] taskArray; //all tasks in the scene
    //public List<GameObject> openTasks;
    public List<GameObject> nearbyTasks;
    public Task myTask;
    public GameObject nearestOpenTask;

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
        // taskArray = GameObject.FindGameObjectsWithTag("Task");
        // for(int i = 0; i < taskArray.Length; i++)
        // {
        //     openTasks.Add(taskArray[i]);
        // }
    }

    private void Initialize()
    {
        follower = gameObject.GetComponent<Follower>();
        helper = gameObject.GetComponent<PikminHelper>();
        player = gameObject.GetComponent<PikminPlayer>();
        grav = gameObject.GetComponent<CustomGravity>();
        machine = gameObject.GetComponent<Animator>();
        obstacleSwitch = gameObject.GetComponent<ObstacleSwitch>();

        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponentInChildren<Collider>();

        manager = FindObjectOfType<PikminManager>();
        captain = FindObjectOfType<PlayerController>().gameObject.transform;
        pointer = FindObjectOfType<PointerController>().gameObject.transform;

        grounded = true;
        SetWithPlayer(false);
        SetHeld(false);
        follower.AgentState(true);
        SetHasTarget(false);
    }
        

    private void LateUpdate()
    {
        if(hasTarget)
        {
            if(withPlayer)
            {
                SetTarget(captain.gameObject);
                distance = helper.UpdateDistance(target.transform, this.transform);
                follower.StoppingDistance(order/16+2);
            }
        }
        if(held)
            Held();
        
        if(machine.GetCurrentAnimatorStateInfo(0).IsName("Idle")
        && nearbyTasks.Count > 0)
        {
            //if idle or active and has nearby tasks
            nearestOpenTask = null;
            nearestOpenTask = SortTasks();
            if(nearestOpenTask != null)
            {
                //if we actually have a nearest open task, pursue!!
                Debug.Log(gameObject.name + "is chasing " + nearestOpenTask.name);
                SetHasTarget(true);
                SetTarget(nearestOpenTask);
                follower.StoppingDistance(0.0f);
                myTask = nearestOpenTask.GetComponent<Task>(); //current task = closest
            }
        }
        if(machine.GetCurrentAnimatorStateInfo(0).IsName("Pursue (Task)"))
        {
            //Debug.Log(gameObject.name + " is pursuing " + nearestOpenTask.name);
            if(!nearbyTasks.Contains(nearestOpenTask))
            {
                Debug.Log(gameObject.name + "'s task is taken. Finding new task...");
                //if pursuing task and it closes, switch back to idle
                obstacleSwitch.Switch(false);
                follower.Destination();
                nearestOpenTask = null;
                myTask = null;
                SetHasTarget(false);
            }
        }
        else if(machine.GetCurrentAnimatorStateInfo(0).IsName("Pursue (Player)"))
        {
            //Debug.Log(gameObject.name+" is pursuing player");
            if(follower.currentTargetObject != captain.gameObject)
            {
                SetTarget(captain.gameObject);
            }
        }
        else if(machine.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if(follower.currentTargetObject == captain.gameObject)
            {
                //SetTarget(null);
            }
        }
        // if(machine.GetCurrentAnimatorStateInfo(0).IsName("Idle") && nearbyTasks.Count > 0) //if idle
        // {
        //     //GameObject newTask = SearchForTasks();
        //     nearbyTasks.Sort((t1,t2) =>
        //         Vector3.Distance(transform.position, t1.transform.position)
        //         .CompareTo(Vector3.Distance(transform.position, t2.transform.position))); //put the closes task on top
        //     Task newTask = nearbyTasks[0].GetComponent<Task>(); //new task = closest
        //     if(newTask != null && newTask.open)
        //     {
        //         SetHasTarget(true);
        //         SetTarget(newTask.gameObject);
        //         follower.StoppingDistance(0.0f);
        //         myTask = nearbyTasks[0].GetComponent<Task>(); //current task = closest
        //     }
        // }
        // if(machine.GetCurrentAnimatorStateInfo(0).IsName("Pursue (Task)"))
        // {
        //     //if pursuing a task, check to make sure it is open
        //     //myTask = nearbyTasks[0].GetComponent<Task>(); //current task = closest
        //     if(!myTask.GetOpen())
        //     {
        //         //if not open, stop follower agent!!
        //         follower.AgentState(false);
        //         SetHasTarget(false);
        //         //SetTarget(null);
        //     }
        // }
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

    public void SetLift(bool _state)
    {
        if(_state)
            machine.SetTrigger("Lifting");
        else
            machine.ResetTrigger("Lifting");
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
                nearbyTasks.Clear();
                SetWithPlayer(true);
                obstacleSwitch.Switch(false);
                follower.AgentState(true);
                helper.SetKinematic(false);
                SetHasTarget(true);
                SetTarget(captain.gameObject);
                myTask = null;
                follower.Destination(target);
                manager.Call(this);
                transform.SetParent(null);
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
        //Debug.Log(gameObject.name + " is dismissed to " +_target.transform.position);
        follower.Destination(_target);
        //Debug.Log(_target.transform.position);
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
        nearbyTasks.Clear();
        SetHeld(true);
        follower.AgentState(false);
        //helper.SetPhysics(false);
        grounded = false;
        SetGrav(0);
        Vector3 playerVector = helper.GetPlayerVector(captain);
        gameObject.transform.position = captain.position + playerVector;
    }

    public void Lift(CarryTarget _carry)
    {
        SetHasTarget(false);
        //SetTarget(null);
        SetLift(true);
        follower.AgentState(false);
        obstacleSwitch.Switch(true);
        grav.gravityScale = 0;
        transform.SetParent(_carry.transform);
        //helper.SetOnlyRB(false);
    }

    public void UpdateNearbyTasks(List<Task> allTasks, List<Task> newTasks)
    {
        //Debug.Log("PikminBase: UpdateNearbyTasks");
        //remove all tasks from group...
        foreach (Task t in allTasks)
        {
            if(nearbyTasks.Contains(t.gameObject))
            {
                //Debug.Log("PikminBase: UpdateNearbyTasks: Removing task t!");
                nearbyTasks.Remove(t.gameObject);
            }
        }
        //...and add only the open ones!
        foreach(Task t in newTasks)
        {
            nearbyTasks.Add(t.gameObject);
        }
    }

    private GameObject SortTasks()
    {
        if(nearbyTasks.Count > 0)
        {
            nearbyTasks.Sort((t1,t2) =>
                    Vector3.Distance(transform.position, t1.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, t2.transform.position))); //put the closes task on top
            return nearbyTasks[0];
        }
        else
            return null;
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
            machine.SetTrigger("Hit Ground");
            machine.SetBool("Throw", false);
            helper.SetTrail(false);
            if(callOnGround)
            {
                callOnGround = false;
                Call();
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag == "Ground")
        {
            //not on ground
            machine.ResetTrigger("Hit Ground");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Carry Target" && !withPlayer)
        {
            //reached carry target

            if((machine.GetCurrentAnimatorStateInfo(0).IsName("Pursue (Task)")
            || machine.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            && nearestOpenTask == other.gameObject)
            {
                //if trying to interact with this specific object
                CarryTarget _target = other.gameObject.GetComponent<CarryTarget>();
                Task _task = other.gameObject.GetComponent<Task>();
                if(_target.RequestCarry(this))
                {
                    //if we can carry this object
                    nearestOpenTask = null;
                    transform.position = other.transform.position;
                    //helper.SetOnlyRB(false);
                    //helper.SetPhysics(false);
                    helper.SetKinematic(true);
                    _target.SetOpen(false);
                    Lift(_target);
                    
                    //attach
                    
                }
            }
        }

        if(other.gameObject.tag == "Task")
        {
            TaskGroup taskGroup = other.GetComponent<TaskGroup>();
            taskGroup.AddPikminToRange(this);
        }
    }

    // //carry
    // private void OnTriggerStay(Collider other)
    // {
    //     if(other.gameObject.tag == "Task")
    //     {
    //         //pikmin has entered the task field
    //         // Task _task = other.gameObject.GetComponent<Task>();
    //         // for(int i = 0; i < _task.myTasks.Count; i++)
    //         //     nearbyTasks.Add(_task.myTasks[i]); //add tasks to nearby task list
    //         //Debug.Log("Inside Task Field");
    //         TaskGroup localGroup = other.gameObject.GetComponent<TaskGroup>();
    //         List<Task> localTasks = new List<Task>();

    //         if(localGroup.CheckIfOpen())
    //         {
    //             //if task is not currently fulfilled
    //             //localTasks = localGroup.CheckAvailability();
    //             localTasks = localGroup.myTasks;
    //             //Debug.Log(localTasks.Count);
    //         }
            
    //         for(int i = 0; i < localTasks.Count; i++)
    //         {
    //             if(nearbyTasks.Contains(localTasks[i].gameObject))
    //             {
    //                 //clear local tasks from nearby task list
    //                 nearbyTasks.Remove(localTasks[i].gameObject);
    //             }

    //             //Debug.Log("Inside local tasks");
    //             // Debug.Log(localTasks[i].GetOpen());
    //             if(localTasks[i].GetOpen()) //only add the task if it is new
    //                 nearbyTasks.Add(localTasks[i].gameObject);
    //             // else if(nearbyTasks.Contains(localTasks[i].gameObject) && !localTasks[i].GetOpen())
    //             // {
    //             //     Debug.Log("removing task");
    //             //     nearbyTasks.Remove(localTasks[i].gameObject); //if task is in list and no longer open, 
    //             // }
    //         }
    //         //List<Task> localTasks = localGroup.myTasks;
    //     }
    // }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Task")
        {
            //pikmin has entered the task field
            //Task _task = other.gameObject.GetComponent<Task>();
            // for(int i = 0; i < _task.myTasks.Count; i++)
            //     nearbyTasks.Remove(_task.myTasks[i]); //add tasks to nearby task list
            TaskGroup localGroup = other.gameObject.GetComponent<TaskGroup>();
            // List<Task> localTasks = localGroup.CheckAvailability();
            // for(int i = 0; i < localTasks.Count; i++)
            // {
            //     if(nearbyTasks.Contains(localTasks[i].gameObject)) //remove tasks from nearby tasks
            //     {
            //         nearbyTasks.Remove(localTasks[i].gameObject);
            //         //Debug.Log("Removing task on trigger exit");
            //     }
            // }
            localGroup.RemovePikminFromRange(this);
            foreach(Task t in localGroup.myTasks)
            {
                //remove tasks from list when too far away
                if(nearbyTasks.Contains(t.gameObject))
                {
                    nearbyTasks.Remove(t.gameObject);
                }
            }
        }

        if(other.gameObject.tag == "Carry Target"
        )
        {
            //leaving carry target
            CarryTarget _target = other.gameObject.GetComponent<CarryTarget>();
            _target.SetOpen(true);
            obstacleSwitch.Switch(false);
            // if(!machine.GetCurrentAnimatorStateInfo(0).IsName("Pursue (Player)"))
            // {
            //     SetHasTarget(false);
            //     follower.Destination();
            //     transform.SetParent(null);
            // }
            //SetTarget(null);
        }
        
    }
}
