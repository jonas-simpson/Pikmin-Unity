using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;
    public Vector3 actualTarget;
    public Transform player;
    public GameObject arcObject;
    public float forceDampen = 18;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            //if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) //only change position if ground
                gameObject.transform.position = hit.point;
        }
        if(Input.GetMouseButton(0))
        {
            //player is clicking
            //DrawArc();
        }
        //actualTransform.y = player.position.y;
    }

    void DrawArc()
    {
        GameObject obj = Instantiate(arcObject, new Vector3(player.transform.position.x, player.transform.position.y+1.5f, player.transform.position.z), Quaternion.identity);
        obj.GetComponent<CustomGravity>().gravityScale = 5;
        Vector3 distance = transform.position - player.transform.position;
        float offset = (distance.y/20) + 1;
        Vector3 ThrowForce = new Vector3(distance.x*offset/forceDampen, 1, distance.z*offset/forceDampen);
        obj.GetComponent<Rigidbody>().AddForce(ThrowForce, ForceMode.Impulse);
    }
}
