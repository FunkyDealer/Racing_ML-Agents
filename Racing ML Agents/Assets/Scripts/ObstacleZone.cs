using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleZone : MonoBehaviour
{
    List<GameObject> obstacles = new List<GameObject>();

    private void Awake()
    {
        if (obstacles.Count == 0)
        {
            int childCount = transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                GameObject c = transform.GetChild(i).gameObject;
                c.SetActive(false);
                obstacles.Add(c);
            }
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

    public void ResetObstacles()
    {
        foreach (GameObject c in obstacles)
        {
            c.SetActive(false);
        }

       // if (UnityEngine.Random.value < 0.7)
            obstacles[UnityEngine.Random.Range(0, obstacles.Count)].SetActive(true);

    }
}
