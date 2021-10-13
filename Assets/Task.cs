using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public TaskGroup myGroup;
    public GameObject parentObject;
    public PikminBase myPikmin;
    public bool open;

    private void Awake()
    {
        parentObject = transform.parent.gameObject;
        myGroup = parentObject.GetComponentInChildren<TaskGroup>();
        //SetOpen(true);
        open = true;
    }
    public void SetOpen(bool _state)
    {
        open = _state;
        myGroup.UpdateAvailableTasks(this, open);
    }
    public bool GetOpen()
    {
        return open;
    }
}
