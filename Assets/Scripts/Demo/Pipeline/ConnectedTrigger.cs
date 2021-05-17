using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedTrigger : MonoBehaviour
{
    public MonoBehaviour ScriptToTrigger;
    
    private void Start()
    {
        if (ScriptToTrigger != null)
        {
            ScriptToTrigger.enabled = false;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
