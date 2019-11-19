using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerLine : MonoBehaviour
{
    public GameObject player;
    public GameObject pointer;
    public LineRenderer line;

    public Vector3[] points;
    public int numPoints = 10;
    public float radianAngle, distance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        pointer = GameObject.FindObjectOfType<PointerController>().gameObject;
        line = GetComponent<LineRenderer>();

        InitializePoints();
    }

    void InitializePoints()
    {
        line.positionCount = numPoints;
        points = new Vector3[numPoints];
        for(int i = 0; i < numPoints; i++)
        {
            points[i] = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLine(player.GetComponent<PlayerController>().pikminToThrow, player.transform.position, pointer.transform.position);
        //DrawLine();
        distance = Vector3.Distance(player.transform.position, pointer.transform.position);
    }

    void DrawLine()
    {
        // y = -(x+(pointer.transform.position-player.transform.position)/2)^2
        /*
        for(int i = 0; i < line.positionCount; i++)
        {
            points[i] = player.transform.position + (pointer.transform.position - player.transform.position)*i/(line.positionCount-1);
        }
        */
        line.SetPositions((points));
    }

    void CalculateLine(string type, Vector3 _player, Vector3 _target)
    {
        List<float> y = new List<float>();
        //Vector3 newPlayer = new Vector3(_player.x, _player.y+0.5f, _player.z);
        _player.y += 0.5f;
        _target.y += 0.25f;
        float mag = Vector3.Magnitude(_player-_target);
        /*
        if(type == "none")
        {
            for (int i = 0; i < line.positionCount; i++)
            {
                points[i] = player.transform.position + (pointer.transform.position - player.transform.position)*i/(line.positionCount-1);
            }
        }
        */
        //calculate x & z
        for (int i = 0; i < line.positionCount; i++)
        {
            points[i] = player.transform.position + (pointer.transform.position - player.transform.position)*i/(line.positionCount-1);
        }
        //calculate y
        if(type == "red") //normal arc
        {
            for (int i = 0; i < line.positionCount; i++)
            {
                points[i] = player.transform.position +
                            (pointer.transform.position - player.transform.position)*i*
                            ((pointer.transform.position.y - player.transform.position.y)/18+1)
                            /(line.positionCount-1);
                /*
                float xz = Vector3.Magnitude(new Vector3(points[i].x, 0, points[i].z));
                float pow = -1 * Mathf.Pow(xz/2, 2);
                y.Add(pow + 5);
                //points[i] = player.transform.position + (pointer.transform.position - player.transform.position)*i/(line.positionCount-1);
                points[i].y = y[i];
                */
                //points[i].y = -Mathf.Abs(i-5)+5;
            }
            List<float> y1 = new List<float>();
            List<float> y2 = new List<float>();
            for(int i = 0; i < numPoints/2; i++)
            {
                //Debug.Log("in loop");
                //Debug.Log(numPoints);
                //Debug.Log((i/numPoints)*6);
                //float newY = i/numPoints;
                //points[i].y = Mathf.Pow(i*2/((float)numPoints-2), 0.5f);
                //points[i].y = 1-Mathf.Pow(i*2/((float)numPoints-2), 2);
                y2.Add((1-Mathf.Pow(i*2/((float)numPoints-2), 2)));
            }
            for(int i = numPoints/2; i < numPoints; i++)
            {
                //points[i].y = Mathf.Pow((100-i)*2/((float)numPoints-1), 0.5f);
                //points[i].y = 1-Mathf.Pow((100-i)*2/((float)numPoints-2), 2);
                y1.Add((1-Mathf.Pow((numPoints-i)*2/((float)numPoints-2), 2)));
            }
            for(int i = 0; i < numPoints; i++)
            {
                if(i < numPoints/2)
                    points[i].y = player.transform.position.y + y1[i]*5;
                else
                    points[i].y = player.transform.position.y + y2[i-numPoints/2]*5;
            }
            

            /*
            points[0].y = 1.0f;
            points[1].y = 3.0f;
            points[2].y = 4.5f;
            points[3].y = 5.5f;
            points[4].y = 6;
            points[5].y = 5.75f;
            points[6].y = 5;
            points[7].y = 4;
            points[8].y = 2.5f;
            points[9].y = 0;
            */
            //points[10].y = 0;

        }

        line.SetPositions((points));
    }
}
