using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follower : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent agent;
    public GameObject currentTargetObject;

    public Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        agent = GetComponent<NavMeshAgent>();

        //currentTargetObject = player.gameObject;
        //target = player.transform.position;
        //agent.SetDestination(target);
    }

    // Update is called once per frame
    void Update()
    {
        //target = player.transform.position;
        if(currentTargetObject != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(currentTargetObject.transform.position);
        }
    }

    //Enable / disable nav mesh agent.
    public void AgentState(bool state)
    {
        agent.enabled = state;
    }

    public void Destination(GameObject targetObject)
    {
        agent.isStopped = false;
        currentTargetObject = targetObject;
    }

    public void Destination(Vector3 targetObject)
    {
        agent.isStopped = false;
        agent.SetDestination(targetObject);
    }

    public void Destination()
    {
        Debug.Log("Pikmin is Dismissed");
        agent.isStopped = true;
        currentTargetObject = null;
    }

    public void StoppingDistance(float scale)
    {
        agent.stoppingDistance = scale;
    }

    public IEnumerator StopAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        Destination();
    }
}
