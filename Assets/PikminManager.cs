using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PikminManager : MonoBehaviour
{
    public int totalPikminWithPlayer, fieldCount;
    //public List<Pikmin> pikmin;
    //public List<Pikmin> pikminWithPlayer;
    public List<PikminBase> pikmin;
    public List<PikminBase> pikminWithPlayer;
    public Dictionary<float, Pikmin> pikminDictionary; //Pikmin, distance
    public GameObject d1, d2, d3, d4, d5;

    public UIManager ui;
    
    void Start()
    {
        ui = GameObject.FindObjectOfType<UIManager>();
        d1 = GameObject.Find("1");
        d2 = GameObject.Find("2");
        d3 = GameObject.Find("3");
        d4 = GameObject.Find("4");
        d5 = GameObject.Find("5");

        Initialize();
    }

    private void Update()
    {
        
        if(pikminWithPlayer.Count > 0)
        {
            SortByDistance();
        }
        
    }

    void Initialize()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Pikmin"))
        {
            pikmin.Add(obj.GetComponent<PikminBase>());
        }
        
        for(int i = 0; i < pikmin.Count; i++)
        {
            if(pikmin[i].GetWithPlayer())
            {
                pikminWithPlayer.Add(pikmin[i]);
            }
        }

        fieldCount = pikmin.Count;
        ui.ChangeCount(fieldCount, "field");
        ui.ChangeCount(pikminWithPlayer.Count, "withPlayer");
    }

    public void Dismiss()
    {
        int currentCount = pikminWithPlayer.Count;
        for (int i = 0; i < currentCount; i++)
        {
            pikminWithPlayer[i].Dismiss(d1);
        }
        for (int i = 0; i < currentCount; i++)
        {
            pikminWithPlayer.RemoveAt(0);
        }
        ui.ChangeCount(pikminWithPlayer.Count, "withPlayer");
    }

    public void Call(PikminBase _pikmin)
    {
        pikminWithPlayer.Add(_pikmin);
        ui.ChangeCount(pikminWithPlayer.Count, "withPlayer");
    }

    private void SortByDistance()
    {
        pikminWithPlayer.Sort((p1,p2) => p1.distance.CompareTo(p2.distance));
        for(int i = 0; i < pikminWithPlayer.Count; i++)
        {
            //pikminWithPlayer[i].UpdateDistance(i/10 + 2);
            pikminWithPlayer[i].order = i;
        }
    }

    public void RemovePikmin(PikminBase _pikmin)
    {
        pikminWithPlayer.Remove(_pikmin);
        ui.ChangeCount(pikminWithPlayer.Count, "withPlayer");
    }
}
