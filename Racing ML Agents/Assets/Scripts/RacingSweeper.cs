using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RacingSweeper : MonoBehaviour
{
    IRacerAI agent;
    int agentId;

    bool agentInside = false;
    bool movingToAgent = false;

    [SerializeField]
    float speed = 2;
    [SerializeField]
    float killTime = 3f;
    [SerializeField]
    float MoveToTargetTime = 2f;

    float distanceToAgent;

    [SerializeField]
    float distanceToStart = 10f;
    [SerializeField]
    float distanceToStop = 1f;
    [SerializeField]
    float warnMoveDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        distanceToAgent = Vector3.Distance(transform.localPosition, agent.GetTransform().localPosition);
        agentId = agent.GetTransform().gameObject.GetInstanceID();
    }

    public void Reset()
    {
        StopAllCoroutines();
        movingToAgent = false;
        agentInside = false;

        Transform agentPos = agent.GetTransform();

        transform.localPosition = agentPos.localPosition - agentPos.forward * distanceToStart;

        distanceToAgent = Vector3.Distance(transform.localPosition, agent.GetTransform().localPosition);

        StartCoroutine(MoveToAgent());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        distanceToAgent = Vector3.Distance(transform.localPosition, agent.GetTransform().localPosition);

        if (movingToAgent)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, agent.GetTransform().localPosition,  0.03f * speed);
        }

        //if (!movingToAgent && distanceToAgent > warnMoveDistance)
        //{
        //    movingToAgent = true;
        //}
    }

    //See if the car is still inside and kill it
    private IEnumerator CheckForkill()
    {
        yield return new WaitForSeconds(killTime);

        if (agentInside) {
            agent.Die();

        }
    }

    private IEnumerator MoveToAgent()
    {
            yield return new WaitForSeconds(MoveToTargetTime);

            movingToAgent = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID() == agentId)
        {
            agentInside = true;
            StopAllCoroutines();
            StartCoroutine(CheckForkill());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetInstanceID() == agentId)
        {
            agentInside = true;

            if (movingToAgent && distanceToAgent < distanceToStop)
            {
                movingToAgent = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetInstanceID() == agentId)
        {
            agentInside = false;
            StopAllCoroutines();
            StartCoroutine(MoveToAgent());
        }
    }

    public void GetAgent(IRacerAI agent)
    {
        this.agent = agent;
    }

    public void GetWarned(Transform agent)
    {
        //Debug.Log("warned");
        if (!movingToAgent && distanceToAgent > warnMoveDistance)
        {
            movingToAgent = true;
        }
    }

}
