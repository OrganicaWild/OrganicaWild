using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Pipeline;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConnectedAreaTrigger : MonoBehaviour
{
    public int partOfGroupX;
    public static Dictionary<int, bool> groupsActivated = new Dictionary<int, bool>();

    public GameObject toSpawn;
    public Vector3 spawnPoint;
    public float secondsToWait;
    private float timeSinceActivated;
    private bool spawnedThing;

    private void Start()
    {
        if (!groupsActivated.ContainsKey(partOfGroupX))
        {
            groupsActivated.Add(partOfGroupX, false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        timeSinceActivated += Time.deltaTime;
        if (timeSinceActivated > secondsToWait && !groupsActivated[partOfGroupX])
        {
            groupsActivated[partOfGroupX] = true;
            Instantiate(toSpawn, spawnPoint, Quaternion.identity);
            GameManager.Get().foundAreas++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (timeSinceActivated < secondsToWait)
        {
            groupsActivated[partOfGroupX] = false;
            timeSinceActivated = 0;
        }
    }

    public static void SetAllToFalse()
    {
        groupsActivated.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0, 0, .5f);
        if (spawnPoint != Vector3.zero)
        {
            Gizmos.DrawCube(spawnPoint, Vector3.one);
        }
    }
}