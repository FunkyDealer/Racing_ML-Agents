using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    float moveSmoothness;
    [SerializeField]
    float rotSmoothness;

    [SerializeField]
    Vector3 moveOffset;
    [SerializeField]
    Vector3 rotOffset;

    [SerializeField]
    Transform target;


    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 targetPos = new Vector3();
        targetPos = target.TransformPoint(moveOffset);

        transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = target.position - transform.position;
        var rotation = new Quaternion();

        rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
    }

}
