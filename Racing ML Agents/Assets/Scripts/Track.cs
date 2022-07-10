using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track
{
    public int Id { get; private set; }
    public string TrackName { get; private set; }
    private float totalForwardReward;
    public float ForwardAverageReward { get; private set; }
    public float TimesDoneForward { get; private set; }
    private float totalBackWardsReward;
    public float BackWardsAverageReward { get; private set; }
    public float TimesDoneBackWards { get; private set; }
    
    public Track(int id, string trackName, bool forward, float reward)
    {
        this.Id = id;
        this.TrackName = trackName;

        if (forward)
        {
            totalForwardReward = reward;
            ForwardAverageReward = reward;
            TimesDoneForward = 1;

            totalBackWardsReward = 0;
            BackWardsAverageReward = 0;
            TimesDoneBackWards = 0;
        }
        else
        {
            totalForwardReward = 0;
            ForwardAverageReward = 0;
            TimesDoneForward = 0;

            totalBackWardsReward = reward;
            BackWardsAverageReward = reward;
            TimesDoneBackWards = 1;
        }

    }

    public void AddEpisodeReward(bool forward, float reward)
    {
        if (forward)
        {
            TimesDoneForward++;
            totalForwardReward += reward;
            ForwardAverageReward = totalForwardReward / TimesDoneForward;
        }
        else
        {
            TimesDoneBackWards++;
            totalBackWardsReward += reward;
            BackWardsAverageReward = totalBackWardsReward / TimesDoneBackWards;
        }
    }



}
