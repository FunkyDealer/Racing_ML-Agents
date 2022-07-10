using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyTrackManager : TrackManager
{
    [SerializeField]
    private SpawnZone forwardSpawn;
    [SerializeField]
    private SpawnZone backWardsSpawn;

    [SerializeField]
    List<ObstacleZone> obstacleZones;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ResetTrack()
    {
        foreach (var w in wayPoints)
        {
            //reset Ends
            w.Reset();
        }

        forwardSpawn.ResetSpawn();
        backWardsSpawn.ResetSpawn();
        this.gameObject.SetActive(false);
    }

    public override Tuple<WayPoint, SpawnZone> SetStartLine(bool forward)
    {
        Tuple<WayPoint, SpawnZone> info;
        WayPoint first = wayPoints[0];
        SpawnZone spawn = forwardSpawn;        

        switch (forward)
        {
            case true:
                spawn = forwardSpawn;
                spawn.ForwardWaypoint.SetHasStartLine();
                first = spawn.ForwardWaypoint;
                break;
            case false:
                spawn = backWardsSpawn;
                spawn.ForwardWaypoint.SetHasStartLine();
                first = spawn.ForwardWaypoint;
                break;
        }

        info = new Tuple<WayPoint, SpawnZone>(first, spawn);

        ResetObstacles();

        return info;
    }

    private void ResetObstacles()
    {
        foreach(var o in obstacleZones)
        {
            o.ResetObstacles();
        }
    }
}
