using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField]
    protected string trackName;
    public string TrackName => trackName;

    [SerializeField]
    protected List <WayPoint> wayPoints = new List<WayPoint>();

    [SerializeField]
    private SpawnZone spawn;
    public SpawnZone Spawn => spawn;

    protected int totalWayPoints;
    public int TotalWayPoints => totalWayPoints;

    protected virtual void Awake()
    {
        this.totalWayPoints = wayPoints.Count;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ResetTrack()
    {
        foreach (var w in wayPoints)
        {
            //reset Ends
            w.Reset();
        }       

        spawn.ResetSpawn();
        this.gameObject.SetActive(false);
    }

    public virtual Tuple<WayPoint, SpawnZone> SetStartLine(bool forward)
    {
        Tuple<WayPoint, SpawnZone> info;
        WayPoint first = wayPoints[0];

        switch (forward)
        {
            case true:
                spawn.ForwardWaypoint.SetHasStartLine();
                first = spawn.ForwardWaypoint;
                break;
            case false:
                spawn.transform.Rotate(Vector3.up, 180);

                spawn.BackwardWaypoint.SetHasStartLine();
                first = spawn.BackwardWaypoint;
                break;
        }

        info = new Tuple<WayPoint, SpawnZone>(first, spawn);

        return info;
    }
}
