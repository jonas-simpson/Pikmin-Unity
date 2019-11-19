using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float h, v; //horizontal & vertical input
    public float speed, moveDrag, stopDrag;

    private Rigidbody rb;
    public Transform cam;
    public Transform pointer;
    public Throw myThrow;

    public PikminManager pikminManager;
    public GameObject heldPikmin;
    public string pikminToThrow;

    void Start()
    {
        rb = gameObject.GetComponentInChildren<Rigidbody>();
        myThrow = gameObject.GetComponentInChildren<Throw>();

        cam = GameObject.Find("Camera Base").transform;
        pikminManager = FindObjectOfType<PikminManager>();
        pointer = FindObjectOfType<PointerController>().gameObject.transform;
        pikminToThrow = "none";
    }

    void Update()
    {
        GetButtons();
    }
    
    private void FixedUpdate()
    {
        if(GetMove())
        {
            rb.drag = moveDrag;
            Move();
        }
        else
        {
            rb.drag = stopDrag;
        }
    }

    public bool GetMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        if(h != 0 || v != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Move()
    {
        /*
        Vector3 moveDir = new Vector3(h, 0.0f, v).normalized;
        rb.AddForce(moveDir * speed, ForceMode.Force);
        */

        //get camera forward & right axes
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward*v + right*h;
        rb.AddForce(moveDir.normalized * speed, ForceMode.Force);

        Vector3 lookDir = rb.velocity.normalized;
        lookDir.y = 0;

        if(rb.velocity.magnitude > 0.01f)
        {
            transform.LookAt(transform.position + lookDir);
        }
    }

    void GetButtons()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Dismiss pikmin
            pikminManager.Dismiss();
            Debug.Log("Dismiss Pikmin (Player Controller)");
        }

        if(Input.GetMouseButtonDown(0) && heldPikmin == null && pikminManager.pikminWithPlayer.Count > 0)
        {
            //if pikmin in group & close, hold && if not holding a pikmin already
            Debug.Log("Hold Pikmin", pikminManager.pikminWithPlayer[0]);
            heldPikmin = pikminManager.pikminWithPlayer[0].gameObject;
            pikminManager.pikminWithPlayer[0].Held();
            pikminToThrow = "red";

            //change line to pikmin's arc

            //pikminManager.pikminWithPlayer[0].gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*1, ForceMode.Impulse);
            //else, punch attack
        }

        if(Input.GetMouseButton(0))
        {
            //if whistle while holding, drop pikmin
            if(Input.GetMouseButtonDown(1))
            {
                Debug.Log("Dropping Pikmin -- Hold Canceled");
                heldPikmin.GetComponent<Pikmin>().Drop();       //drop pikmin
                heldPikmin = null;                              //no longer holding a pikmin
                pikminToThrow = "none";
            }
        }

        if(Input.GetMouseButtonUp(0) && heldPikmin != null)
        {
            //if holding pikmin, throw
            pikminToThrow = "none";
            Debug.Log("Throw Pikmin! - ", heldPikmin);
            heldPikmin.GetComponent<PikminBase>().Throw();
            pikminManager.RemovePikmin(heldPikmin.GetComponent<PikminBase>());
            heldPikmin = null;
        }
    }
}
