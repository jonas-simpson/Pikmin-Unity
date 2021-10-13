using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskGroup : MonoBehaviour
{
    public List<Task> myTasks;  //list of tasks in the object. some groups will only have 1
    public List<Task> availableTasks;
    public GameObject parentObject;
    public List<PikminBase> myPikmin;
    public List<PikminBase> pikminInRange;
    public bool open;           //open will allow pikmin to join the task

    private void Awake()
    {
        parentObject = transform.parent.gameObject;
        Task[] taskArray = parentObject.GetComponentsInChildren<Task>();
        for(int i = 0; i < taskArray.Length; i++)
        {
            myTasks.Add(taskArray[i]);          //add my tasks to all tasks
            availableTasks.Add(taskArray[i]);   //and they should all start open
        }
        //availableTasks = myTasks;
    }

    private void Update()
    {
        // List<Task> openTasks = CheckAvailability();
        // if(openTasks.Count > 0)
        // {
        //     open = true;
        // }
        // else
        //     open = false;
    }

    // public List<Task> CheckAvailability()
    // {
    //     List<Task> openTasks = new List<Task>();
    //     for(int i = 0; i < myTasks.Count; i++)
    //     {
    //         if(myTasks[i].GetOpen())
    //         {
    //             openTasks.Add(myTasks[i]);
    //         }
    //         // else
    //         // {
    //         //     if(!myPikmin.Contains(myTasks[i].myPikmin))
    //         //     {
    //         //         myPikmin.Add(myTasks[i].myPikmin);
    //         //     }
    //         // }
    //     }
    //     return openTasks;
    // }

    public bool CheckIfOpen()
    {
        return open;
    }

    public void AddPikminToRange(PikminBase _pikmin)
    {
        if(!pikminInRange.Contains(_pikmin))
        {
            pikminInRange.Add(_pikmin); //add pikmin to range
            //Debug.Log("Task Group: Adding Pikmin");
            AnnounceAvailability(); //announce open tasks to pikmin in range
        }
    }
    public void RemovePikminFromRange(PikminBase _pikmin)
    {
        if(pikminInRange.Contains(_pikmin))
        {
            pikminInRange.Remove(_pikmin);
            //Debug.Log("Task Group: Removing Pikmin");
        }
    }

    public void UpdateAvailableTasks(Task _task, bool _open)
    {
        Debug.Log("TaskGroup: UpdateAvailableTasks");
        if(_open && !availableTasks.Contains(_task))
        {
            //if task is open and not in open task list, add!
            availableTasks.Add(_task);
        }
        else if(!_open && availableTasks.Contains(_task))
        {
            //if task is not open and in open task list, remove!
            availableTasks.Remove(_task);
        }
        AnnounceAvailability(); //announce to all nearby pikmin when task changes state
    }
    public void AnnounceAvailability()
    {
        //Debug.Log("Task Group: Announce Availability");
        //announce to all pikmin in range when tasks open or close
        foreach(PikminBase p in pikminInRange)
        {
            Debug.Log("Task Group: Announce Availability: inside pikmin loop");
            p.UpdateNearbyTasks(myTasks, availableTasks);
        }
    }
}
