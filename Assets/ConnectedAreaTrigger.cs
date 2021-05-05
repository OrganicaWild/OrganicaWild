using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConnectedAreaTrigger : MonoBehaviour
{
    public int partOfGroupX;
    public static Dictionary<int, bool> groupsActivated = new Dictionary<int, bool>();

    public GameObject toSpawn;
    
    private void Start()
    {
        if (!groupsActivated.ContainsKey(partOfGroupX))
        {
            groupsActivated.Add(partOfGroupX, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (groupsActivated[partOfGroupX] == false)
        {
            groupsActivated[partOfGroupX] = true;
            Instantiate(toSpawn);
        }
    }
}
