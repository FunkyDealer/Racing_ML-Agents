using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingMaster : MonoBehaviour
{
    private static TrainingMaster _instance;
    public static TrainingMaster inst { get { return _instance; } }

    List<Track> tracks = new List<Track>();

    enum Mode
    {
        none,
        ForceTrack,
        ForceRandom
    }

    enum Direction
    {
        UP,
        DOWN,
        RANDOM
    }

    [SerializeField]
    Mode mode = Mode.none;
    [SerializeField]
    Direction direction = Direction.RANDOM;
    [SerializeField, Range(0, 4)]
    int trackNumber = 0;

    [SerializeField]
    int differenceToForce = 10;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool TrackExists(string trackName)
    {
        if (tracks.Count > 0)
        {
            foreach (Track t in tracks)
            {
                //Debug.Log($"Checking if track is already in list -> comparing track {t.TrackName} to track {trackName}, result is {t.TrackName == trackName}");
                if (t.TrackName == trackName) return true;                
            }
        }

        return false;
    }

    private bool InsertTrack(string trackName, bool forward, float reward)
    {
        if (!TrackExists(trackName))
        {
            Debug.Log($"inserting track named {trackName}");
            Track newtrack = new Track(tracks.Count + 1, trackName, forward, reward);
            tracks.Add(newtrack);
            return true;
        }

        return false;
    }

    public void ReceiveEpisodeInformation(string trackName, bool forward, float reward)
    {
        if (!InsertTrack(trackName, forward, reward))
        {
            Track t = GetTrackFromList(trackName);

            t.AddEpisodeReward(forward, reward);
        }
    }

    private Track GetTrackFromList(string trackName)
    {
        Track track = tracks[0];

        foreach (Track t in tracks)
        {
            if (t.TrackName == trackName) track = t;
        }
        return track;
    }

    public Tuple<TrackManager, bool> DecideTrack(List<TrackManager> trackList)
    {
        bool forward = UnityEngine.Random.value > 0.5f;

        switch (direction)
        {
            case Direction.UP:
                forward = true;
                break;
            case Direction.DOWN:
                forward = false;
                break;
            case Direction.RANDOM:
                //do Nothing
                break;
        }

        Tuple<TrackManager, bool> trackAndWay = new Tuple<TrackManager, bool>(trackList[UnityEngine.Random.Range(0, trackList.Count)], forward);

        switch (mode)
        {
            case Mode.none:
                TryToSelectWeakestTrack(trackList, trackAndWay);
                break;
            case Mode.ForceTrack:
                trackAndWay = new Tuple<TrackManager, bool>(trackList[trackNumber], forward);
                break;
            case Mode.ForceRandom:
                //do Nothing
                break;
        }
       

        return trackAndWay;
    }

    void TryToSelectWeakestTrack(List<TrackManager> trackList, Tuple<TrackManager, bool> trackAndWay)
    {
        if (tracks.Count == trackList.Count)
        {
            Tuple<string, bool, float> highest = GetHighestScore();
            Tuple<string, bool, float> lowest = GetLowestScore();

            float difference = highest.Item3 - lowest.Item3;

            //Debug.Log($"difference between {highest.Item1}({highest.Item2}) and {lowest.Item1}({lowest.Item2}): {difference}");

            if (difference > differenceToForce)
            {
                TrackManager m = trackList[0];

                foreach (TrackManager i in trackList)
                {
                    if (i.TrackName == lowest.Item1) m = i;
                }

                trackAndWay = new Tuple<TrackManager, bool>(m, lowest.Item2);

                if (UnityEngine.Random.value < 0.25f) //randomly with 25% chance put in random map
                    trackAndWay = new Tuple<TrackManager, bool>(trackList[UnityEngine.Random.Range(0, trackList.Count)], UnityEngine.Random.value > 0.5f);
            }



        }
    }


    public Tuple<string, bool, float> GetLowestScore()
    {
        Tuple<string, bool, float> lowest = new Tuple<string, bool, float>(tracks[0].TrackName, true, tracks[0].ForwardAverageReward);
        float lowestScore = tracks[0].ForwardAverageReward;

        foreach (Track t in tracks)
        {
            if (t.ForwardAverageReward < lowestScore) {
                lowest = new Tuple<string, bool, float>(t.TrackName, true, t.ForwardAverageReward);
                lowestScore = t.ForwardAverageReward;
            }
            if (t.BackWardsAverageReward < lowestScore)
            {
                lowest = new Tuple<string, bool, float>(t.TrackName, false, t.BackWardsAverageReward);
                lowestScore = t.BackWardsAverageReward;
            }
        }

        return lowest;
    }

    public Tuple<string, bool, float> GetHighestScore()
    {
        Tuple<string, bool, float> highest = new Tuple<string, bool, float>(tracks[0].TrackName, true, tracks[0].ForwardAverageReward);
        float highestScore = tracks[0].ForwardAverageReward;

        foreach (Track t in tracks)
        {
            if (t.ForwardAverageReward > highestScore)
            {
                highest = new Tuple<string, bool, float>(t.TrackName, true, t.ForwardAverageReward);
                highestScore = t.ForwardAverageReward;
            }
            if (t.BackWardsAverageReward > highestScore)
            {
                highest = new Tuple<string, bool, float>(t.TrackName, false, t.BackWardsAverageReward);
                highestScore = t.BackWardsAverageReward;
            }
        }

        return highest;
    }



}
