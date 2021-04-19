using System;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class OwLine : IGeometry
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }

        public OwLine(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        
        public Vector2 GetCentroid()
        {
            //simply returns the middle point of the line
            return End - Start / 2f;
        }

        public void DrawDebug(Color debugColor, Vector2 coordinateSystemCenter)
        {
            Gizmos.color = debugColor;
            Vector3 center = new Vector3(coordinateSystemCenter.x, 0, coordinateSystemCenter.y);
            Gizmos.DrawLine(center + new Vector3(Start.x, 0, Start.y), center + new Vector3(End.x, 0, End.y));
        }

        public float Length()
        {
            return (End - Start).magnitude;
        }
    }
}