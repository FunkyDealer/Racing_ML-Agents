using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField]
    private WayPoint previousWayPoint;
    [SerializeField]
    private WayPoint nextWayPoint;
 
    private bool startLine;
    public bool StartLine => startLine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            IRacerAI AI = other.GetComponent<IRacerAI>();            

            if (AI.GetAINextWayPoint() == this.transform)
            {
                bool forward = AI.GetRaceDir();

                if (!startLine)
                {
                    if (forward) AI.GetNextWayPoint(nextWayPoint.transform, startLine);
                    else AI.GetPreviousWayPoint(previousWayPoint.transform, startLine);
                }

                if (startLine && AI.HasStartedRace()) //If this is the startLine and the agent already passed through here
                {
                    if (forward) AI.GetNextWayPoint(nextWayPoint.transform, startLine);
                    else AI.GetPreviousWayPoint(previousWayPoint.transform, startLine);
                    
                }
                else if (startLine && !AI.HasStartedRace()) //if this is the startLine and the agent hasn't yet passed
                {
                    if (forward) AI.GetNextWayPoint(nextWayPoint.transform, false);
                    else AI.GetPreviousWayPoint(previousWayPoint.transform, false);
                    AI.StartRace();
                }
                
            }
            else
            {
                if (!startLine) AI.Die();
            }

        }
    }

    public void SetHasStartLine()
    {
        this.startLine = true;
    }

    public virtual void Reset()
    {
        this.startLine = false;
    }
}
