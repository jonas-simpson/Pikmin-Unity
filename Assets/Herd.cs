using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herd : MonoBehaviour
{
    public Vector3 avg;
    public Transform player;
    public List<Vector3> pikmin;
    public PikminManager manager;
    public float x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PikminManager>();
        player = FindObjectOfType<PlayerController>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(manager.pikminWithPlayer.Count > 0)
        {
            CalculateAverage();
            transform.position = player.transform.position;
            transform.LookAt(avg);
        }
    }

    void CalculateAverage()
    {
        pikmin.Clear();
        //avg = Vector3.zero;
        foreach(PikminBase _pikmin in manager.pikminWithPlayer)
        {
            if(!_pikmin.held)
            {
                pikmin.Add(_pikmin.transform.position);
                avg += _pikmin.transform.position;
            }
        }
        float scale = (float)1/(float)(manager.pikminWithPlayer.Count+1);
        //Debug.Log(scale);
        avg *= scale;
    }
}
