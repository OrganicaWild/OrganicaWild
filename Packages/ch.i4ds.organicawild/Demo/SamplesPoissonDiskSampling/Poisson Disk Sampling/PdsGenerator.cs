using System;
using System.Collections.Generic;
using Framework.Poisson_Disk_Sampling;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Demo.Poisson_Disk_Sampling
{
    public class PdsGenerator : MonoBehaviour
    {
        public float radius = 1;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;
        public float displayRadius = 1;

        private IEnumerable<Vector2> points;

        private void OnValidate()
        {
           points = PoissonDiskSampling.GeneratePoints(radius, regionSize.x, regionSize.y, rejectionSamples);
        }

        private void OnDrawGizmos()
        {
            Vector3 size = new Vector3(regionSize.x+displayRadius*2,0, regionSize.y+displayRadius*2);
        
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(regionSize.x/2,0, regionSize.y/2), size);
            Gizmos.color = Color.green;
            if (points != null)
            {
                foreach (Vector2 point in points)
                {
                    Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), displayRadius);
                }
            }
        }
    }
}
