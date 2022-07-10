using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField]
    WayPoint forwardWayPoint;
    public WayPoint ForwardWaypoint => forwardWayPoint;
    [SerializeField]
    WayPoint backwardWayPoint;
    public WayPoint BackwardWaypoint => backwardWayPoint;

    [SerializeField]
    Vector3 originalRot;



    private void Awake()
    {
        //originalRot = transform.localEulerAngles;
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetSpawn()
    {
        Quaternion currentRot = transform.rotation;
        currentRot.eulerAngles = originalRot;

        transform.localEulerAngles = originalRot;
    }
}
