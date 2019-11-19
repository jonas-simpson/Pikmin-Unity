using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PikminPlayer : MonoBehaviour
{
    private PikminBase pikmin;
    public float throwDrag, forceDampen, throwForceY;

    private void Awake()
    {
        pikmin = gameObject.GetComponent<PikminBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Whistle")
        {
            //pikmin is contacting with whistle
            pikmin.Call();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerController captain = other.gameObject.GetComponent<PlayerController>();
            //player is bumping into pikmin (ON PURPOSE)
            if(captain.GetMove())
                pikmin.Call();
        }
    }

    public void NewThrow(Rigidbody _rb, CustomGravity _grav, Vector3 _pikPos, Vector3 _capPos, Vector3 _pointer)
    {
        //place pikmin above player
        gameObject.transform.position = _capPos + new Vector3(0,2f,0);

        //face the target
        //gameObject.transform.LookAt(pointer.position);

        //reset velocity
        _rb.velocity = Vector3.zero;

        //calculate vectors & throw!
        _rb.drag = throwDrag;
        _grav.gravityScale = 5;
        Vector3 distance = _pointer - _capPos;
        float offset = (distance.y/20) + 1;
        Vector3 ThrowForce = new Vector3(distance.x*offset/forceDampen, throwForceY, distance.z*offset/forceDampen);
        _rb.AddForce(ThrowForce, ForceMode.Impulse);
    }


}
