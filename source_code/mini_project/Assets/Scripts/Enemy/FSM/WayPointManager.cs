using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assign different routes to each enemy
/// </summary>
public class WayPointManager : MonoBehaviour
{

    private static WayPointManager _instance;

    /* Property encapsulation */
    public static WayPointManager Instance
    {
        get
        {
            return _instance;
        }
    }

    /* Use 2 lists to randomly generate different routes for multiple enemies to prevent enemies from walking the same route (assign different route IDs to each enemy) */
    public List<int> usingIndex = new List<int>();//Route ID used for each enemy assignment
    public List<int> rawIndex = new List<int>();//Auxiliary list, shuffle 0, 1, 2, and reassign

    private void Awake()
    {
        _instance = this;
        int tempCount = rawIndex.Count;//Assign route ID
        for (int i = 0; i < tempCount; i++)
        {
            int tempIndex = Random.Range(0, rawIndex.Count);//Randomly all route positions
            Mathf.Clamp(tempIndex, 0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);//Assign route
            rawIndex.RemoveAt(tempIndex);//After assigning the route, delete the number (to prevent repetition)

        }


    }


}
