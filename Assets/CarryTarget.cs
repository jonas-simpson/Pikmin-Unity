using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryTarget : Task
{
    //public PikminBase currentPikmin;
    public CarryObject parent;

    public bool RequestCarry(PikminBase pikmin)
    {
        if(open)
        {
            Debug.Log("Pikmin accepted");
            SetOpen(false);
            myPikmin = pikmin;
            parent.AddPikmin();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCarrying()
    {
        if(parent.IsCarrying())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
