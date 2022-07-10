using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPointer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GetNewTarget(Transform waypoint)
    {
        transform.position = new Vector3(waypoint.position.x, transform.position.y, waypoint.position.z);
    }
}
