using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikmin : MonoBehaviour
{
    public bool withPlayer, held;
    public float distFromPlayer, maxDistFromPlayer;
    public int order;
    public float throwForceY, throwDrag, forceDampen;

    public Follower follower;
    public Rigidbody rb;
    public Collider col;
    public Throw myThrow;
    public CustomGravity grav;

    public PlayerController player;
    public PikminManager manager;
    public Transform pointer;

    //overall state that determines what the pikmin will do next
    public enum State
    {
        IDLE,       //completely idle, and open to any nearby task
        ACTIVE,     //focused on one task, will not swap tasks until current task is complete or interrupted
        DEAD        //Pikmin has died and needs to be subtracted and destroyed
    }
    //Active state that determines what the pikmin is currently doing if active
    public enum Activity
    {
        PURSUE,     //follower script has a target, pikmin is moving towards a task or the player
        ATTACK,     //pikmin is attacking an enemy or obstacle
        CARRY,      //pikmin is carrying object
        DIG,        //pikmin is digging up a buried object.
        HELD,       //pikmin is currently being held by the player, and will either be thrown (THROWN) or added back to the group (PURSUE)
        THROWN,     //pikmin is airborne and thrown by the player. Will become idle on impact with the ground
        STAND,      //pikmin is standing by their target, but otherwise doing nothing.
        ATRISK      //pikmin is being eaten, attacked, or otherwise suffering and will die on its next state if nothing interferes.
    }

    public State state;
    public Activity activity;

    // Start is called before the first frame update
    void Awake()
    {
        follower = GetComponentInChildren<Follower>();
        withPlayer = true;
        player = FindObjectOfType<PlayerController>();
        manager = FindObjectOfType<PikminManager>();
        rb = GetComponentInChildren<Rigidbody>();
        col = GetComponentInChildren<Collider>();
        myThrow = GetComponentInChildren<Throw>();
        pointer = FindObjectOfType<PointerController>().gameObject.transform;
        grav = GetComponentInChildren<CustomGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        if(withPlayer && !held)
        {
            UpdateDistance();
        }
        if(held)
        {
            Held();
        }
    }

    private protected void StateMachine()
    {
        switch(state)
        {
            case (State.IDLE):
            {
                //pikmin will become ACTIVE and PURSUE a target if nearby or called by the player
                //if transition to ACTIVE PURSUE: follower enable nav mesh agent
                break;
            }
            case (State.ACTIVE):
            {
                switch(activity)
                {
                    case (Activity.PURSUE):
                    {
                        //if agent reaches destination - change activity
                        break;
                    }
                    case (Activity.THROWN):
                    {
                        //if pikmin touches ground, become IDLE
                        break;
                    }
                }
                break;
            }
        }
    }

    public void Dismiss()
    {
        withPlayer = false;
        state = State.IDLE;
        activity = Activity.STAND;
        follower.Destination();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Whistle" && state != State.DEAD && !withPlayer && activity != Activity.THROWN)
        {
            //whistled by player
            //set state to pursue, target = player
            Called();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player" && state == State.IDLE && !withPlayer)
        {
            //player touches pikmin, call them
            Called();
        }
        if(other.gameObject.tag == "Ground")
        {
            rb.drag = 2;
            grav.gravityScale = 1;
            state = State.IDLE;
            activity = Activity.STAND;
        }
    }

    private void Called()
    {
            state = State.ACTIVE;
            activity = Activity.PURSUE;
            withPlayer = true;
            //manager.Call(this);
            follower.AgentState(true);
            follower.Destination(player.gameObject);
    }

    public void UpdateDistance()
    {
        distFromPlayer = Mathf.Abs(Vector3.Distance(player.transform.position, transform.position));
        follower.StoppingDistance(order/16+2);
    }

    public void Held()
    {
        follower.AgentState(false);
        SetPhysics(false);
        Vector3 playerVector = GetPlayerVector();
        gameObject.transform.position = player.transform.position + playerVector;
    }

    public Vector3 GetPlayerVector()
    {
        Vector3 forward = player.transform.forward;
        Vector3 right = player.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward + right;
    }

    public void Drop()
    {
        SetPhysics(true);
        held = false;
        state = State.ACTIVE;
        activity = Activity.PURSUE;
        follower.AgentState(true);
    }

    public void Throw()
    {
        held = false;
        state = State.ACTIVE;
        activity = Activity.THROWN;
        withPlayer = false;
        SetPhysics(true);
        NewThrow();
    }

    public void SetPhysics(bool _state)
    {
        rb.detectCollisions = _state;
        //rb.useGravity = _state;
        col.enabled = _state;
    }

    public void SetCollider(bool _state)
    {
        col.enabled = _state;
    }

    public void NewThrow()
    {
        //place pikmin above player
        gameObject.transform.position = player.transform.position + new Vector3(0,2f,0);

        //face the target
        //gameObject.transform.LookAt(pointer.position);

        //reset velocity
        rb.velocity = Vector3.zero;

        //calculate vectors & throw!
        rb.drag = throwDrag;
        grav.gravityScale = 5;
        Vector3 distance = pointer.position - player.transform.position;
        float offset = (distance.y/20) + 1;
        Vector3 ThrowForce = new Vector3(distance.x*offset/forceDampen, throwForceY, distance.z*offset/forceDampen);
        rb.AddForce(ThrowForce, ForceMode.Impulse);
    }
}
