using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingTrainerManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> agentsObject;

    List<IRacerAI> agents;

    [SerializeField]
    Transform cameraPos;
    public Transform GetCameraPos => cameraPos;

    [SerializeField]
    MeshRenderer sucessRenderer;

    [SerializeField]
    List<TrackManager> tracks = new List<TrackManager>();

    private void Awake()
    {
        agents = new List<IRacerAI>();

        foreach (var t in tracks)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var a in agentsObject)
        {
            IRacerAI AI = (IRacerAI)a.GetComponent(typeof(IRacerAI));
            if (AI != null) agents.Add(AI);
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        SetTrackFirstTime();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetTrackFirstTime()
    {
        
        foreach (var t in tracks)
        {
            t.ResetTrack();
        }

        Tuple<TrackManager, bool> trackAndWay = TrainingMaster.inst.DecideTrack(tracks);

        trackAndWay.Item1.gameObject.SetActive(true); //set track active

        Tuple<WayPoint, SpawnZone> firstWayPoint = trackAndWay.Item1.SetStartLine(trackAndWay.Item2);

        
        foreach (var a in agents)
        {
            a.GetTransform().parent = trackAndWay.Item1.transform; //set the agent's parent as the track

            a.SetRaceDir(trackAndWay.Item2);
            a.SetRespawnPoint(firstWayPoint.Item2);
            a.SetFirstWayPoint(firstWayPoint.Item1.transform);
            a.SetTotalCheckPoints(trackAndWay.Item1.TotalWayPoints);
            a.SetTrackName(trackAndWay.Item1.TrackName);            
        }
    }

    public void ResetManager()
    {
        foreach (var t in tracks) 
        {            
            t.ResetTrack();
        }

        Tuple<TrackManager, bool> trackAndWay = TrainingMaster.inst.DecideTrack(tracks);

        trackAndWay.Item1.gameObject.SetActive(true); //set track active

        Tuple<WayPoint, SpawnZone> firstWayPoint = trackAndWay.Item1.SetStartLine(trackAndWay.Item2);

        ResetAgents(trackAndWay, firstWayPoint);
    }

    private void ResetAgents(Tuple<TrackManager, bool> trackAndWay, Tuple<WayPoint, SpawnZone> firstWayPoint)
    {        
        foreach (var a in agents)
        {
            a.GetTransform().parent = trackAndWay.Item1.transform; //set the agent's parent as the track

            a.SetRaceDir(trackAndWay.Item2);
            a.SetRespawnPoint(firstWayPoint.Item2);
            a.SetFirstWayPoint(firstWayPoint.Item1.transform);
            a.SetTotalCheckPoints(trackAndWay.Item1.TotalWayPoints);
            a.SetTrackName(trackAndWay.Item1.TrackName);

            a.ResetAgent();
        }

    }

    public void EndAgentEpisode(IRacerAI agent, float reward, string trackName, bool forward)
    {
        TrainingMaster.inst.ReceiveEpisodeInformation(trackName, forward, reward);

        CheckAllAgentsStatus();

    }

    private void CheckAllAgentsStatus()
    {
        int totalDeadAgents = 0;
        foreach (var a in agents)
        {
            if (!a.GetAliveStatus()) totalDeadAgents++;
        }

        if (totalDeadAgents == agents.Count) {
            ResetManager();
        }
    }

    public void SucessChange(Color c)
    {
        sucessRenderer.material.color = c;
    }
}
