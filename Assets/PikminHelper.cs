using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PikminHelper : MonoBehaviour
{
    private PikminBase pikmin;
    private TrailRenderer trail;

    private void Awake()
    {
        pikmin = gameObject.GetComponent<PikminBase>();
        trail = gameObject.GetComponentInChildren<TrailRenderer>();
    }

    public void SetPhysics(bool _state)
    {
        pikmin.rb.detectCollisions = _state;
        pikmin.col.enabled = _state;
    }

    public void SetOnlyRB(bool _state)
    {
        pikmin.rb.detectCollisions = _state;
    }

    public void SetKinematic(bool _state)
    {
        pikmin.rb.isKinematic = _state;
    }

    public float UpdateDistance(Transform target, Transform me)
    {

        return Mathf.Abs(Vector3.Distance(target.position, me.position));
        //pikmin.follower.StoppingDistance(pikmin.order/16+2);
    }

    public Vector3 GetPlayerVector(Transform _player)
    {
        Vector3 forward = _player.forward;
        Vector3 right = _player.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward + right;
    }

    public void SetTrail(bool _state)
    {
        trail.enabled = _state;
    }
}
