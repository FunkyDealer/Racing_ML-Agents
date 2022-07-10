using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalWayPoint : WayPoint
{


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

                if (forward) AI.GetNextWayPoint(this.transform, true);
                else AI.GetPreviousWayPoint(this.transform, true);
            }
            else
            {
                AI.Die();
            }

        }
    }

    public override void Reset()
    {
        
    }

}
