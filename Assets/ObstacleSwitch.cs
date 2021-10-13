using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleSwitch : MonoBehaviour
{
    public NavMeshObstacle obstacle;

    private void Awake()
    {
        obstacle = gameObject.GetComponent<NavMeshObstacle>();
    }
    public void Switch(bool _state)
    {
        obstacle.enabled = _state;
    }
}
