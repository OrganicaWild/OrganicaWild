using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PoissonPerformance : MonoBehaviour
{
    // Start is called before the first frame update

    public int size;
    void Start()
    {
        StartCoroutine(nameof(Run));
    }

    private IEnumerator Run()
    {
        string path = @"C:\Users\Christoph\Desktop\results.txt";
        var timeString = "";
        var memoryString = "";

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(path))
        {
            for (int i = 0; i <= size; i += 20)
            {

                Stopwatch start = new Stopwatch();
                start.Start();
                long preInitiMemory = GC.GetTotalMemory(true);
                
                IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(2f, i, i);

               
                
                start.Stop();
                timeString += $"{start.ElapsedMilliseconds} ms elapsed for {i*i} size.         points: {points.Count()} \n";


                long postInit = GC.GetTotalMemory(true);
                memoryString +=
                    $"diff:{postInit - preInitiMemory}, {postInit} {preInitiMemory}  bytes allocated for {i*i} size. \n";
                    
                Debug.Log($"Poisson ran for {i}  big field {points.Count()}");
                
                if (i == size)
                {
                    sw.Write(timeString);
                    sw.Write(memoryString);
                    sw.Flush();
                    Application.Quit();
                }

                yield return null;
            }
        }
    }
}
