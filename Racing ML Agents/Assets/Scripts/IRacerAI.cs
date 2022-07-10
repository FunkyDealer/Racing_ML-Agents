using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRacerAI 
{
    public Transform GetAINextWayPoint();
    public void SetFirstWayPoint(Transform t);
    public void SetRespawnPoint(SpawnZone t);
    public SpawnZone GetRespawnPoint();
    public void GetNextWayPoint(Transform nextPoint, bool end);
    public void GetPreviousWayPoint(Transform previousPoint, bool end);
    public Transform GetTransform();
    public void SetRaceDir(bool forward);
    public bool GetRaceDir();
    public void SetTotalCheckPoints(int total);
    public void SetTrackName(string trackName);
    public void Die();
    public bool GetAliveStatus();
    public void ResetAgent();
    public void StartRace();
    public bool HasStartedRace();

}
