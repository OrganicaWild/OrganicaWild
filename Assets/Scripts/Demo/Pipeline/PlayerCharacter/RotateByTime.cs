using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByTime : MonoBehaviour
{

    public int fullCircleTimeInMinutes;
    public Vector3 axis;
    private float currentTime;

    private void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        float rotationPercent = currentTime / fullCircleTimeInMinutes;
        float degrees = rotationPercent * 360;
        this.transform.rotation = Quaternion.Euler(axis * degrees);
    }
}
