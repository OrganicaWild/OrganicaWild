using System.Collections.Generic;
using Framework.PoissonDiskSampling;
using UnityEngine;

[ExecuteInEditMode]
public class PoissonDiskSamplingHelper : MonoBehaviour
{
    [Range(1,100)]
    public float radius;
    
    public Vector2 size;
    
    [Range(1,50)]
    public int numberOfTriesBeforeRejection;

    public bool generateSwitch;

    private IEnumerable<Vector2> points = new List<Vector2>();

    // Start is called before the first frame update
    void Update()
    {
        Generate();
    }

    public void Generate()
    {
        if (size.x < 0)
        {
            size.x = 0;
        }

        if (size.y < 0)
        {
            size.y = 0;
        }
        points = PoissonDiskSampling.GeneratePoints(radius, size.x, size.y, numberOfTriesBeforeRejection);
    }

    private void OnDrawGizmos()
    {
        foreach (Vector2 point in points)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(point, radius / 4);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(point, radius / 2);
        }
    }
}