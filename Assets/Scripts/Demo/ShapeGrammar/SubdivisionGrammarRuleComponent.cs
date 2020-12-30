using System;
using System.Collections.Generic;
using System.Linq;
using Framework.ShapeGrammar;
using Framework.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.ShapeGrammar
{
    public class SubdivisionGrammarRuleComponent : ShapeGrammarRuleComponent
    {
        private void Awake()
        {
            Modify();
        }

        public override void Modify()
        {
            connection = Instantiate(connection) as ScriptableConnections;

            connection.entryCorner.center = transform.position;

            foreach (SpaceNodeConnection shapeConnection in connection.corners)
            {
                shapeConnection.center = transform.position;
            }

            for (int i = 0; i < 20; i++)
            {
                SubDivide();
            }

            DrawPolygon();
        }

        public void SubDivide()
        {
            List<SpaceNodeConnection> corners = new List<SpaceNodeConnection>() {connection.entryCorner};
            corners.AddRange(connection.corners);

            int index = Random.Range(0, corners.Count);
            float percentage = Random.value;
            Vector3 v0 = corners[index].connectionPoint;
            Vector3 v1 = v0 - corners[index].connectionDirection;

            Vector3 d = v1 - v0;
            int sign = Math.Sign(Random.value - 0.5);
            Vector3 normal = new Vector3(sign * d.z, 0, -sign * d.x);

            Vector3 newPoint = Vector3.Lerp(v0, v1, percentage) + normal;

            SpaceNodeConnection newConnection = new SpaceNodeConnection()
                {connectionPoint = newPoint, connectionDirection = -normal, center = transform.position};
            connection.corners.Add(newConnection);
        }

        public void DrawPolygon()
        {
            List<SpaceNodeConnection> corners = new List<SpaceNodeConnection>() {connection.entryCorner};
            corners.AddRange(connection.corners);

            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            List<Vector2> newVertices = corners.Select(x => x.connectionPoint)
                .Select(x => new Vector2(x.x, x.z)).ToList();
            newVertices.Sort(new PointComparator(transform.position));
            mesh.vertices = newVertices.Select(x => new Vector3(x.x, 0f, x.y)).ToArray();

            Triangulator tr = new Triangulator(newVertices.ToArray());
            int[] indeces = tr.Triangulate();
            mesh.triangles = indeces;

            Vector3[] normals = new Vector3[newVertices.Count];
            Debug.Log($"number of indeces: {indeces.Length}");

            Vector3 p0 = mesh.vertices[indeces[0]];
            Vector3 p1 = mesh.vertices[indeces[1]];
            Vector3 p2 = mesh.vertices[indeces[2]];

            Vector3 d0 = p0 - p1;
            Vector3 d1 = p0 - p2;

            Vector3 normal = Vector3.Cross(d0, d1);
            //Debug.Log($"normal {normal}");
            if (normal.y < 0)
            {
                normal *= -1;
            }

            //Debug.Log($"pot flipeed normal {normal}");
            Debug.DrawRay(p0, normal, Color.green, 10f);

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normal.normalized;
                Debug.Log($"{normals[i]}");
            }
            
            mesh.normals = normals.ToArray();
            //mesh.RecalculateBounds();
            Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
        }

        public class PointComparator : Comparer<Vector2>
        {
            private Vector2 center;

            public PointComparator(Vector2 center)
            {
                this.center = center;
            }

            public override int Compare(Vector2 a, Vector2 b)
            {
                return (int) ((b.x - center.x) * (a.y - center.y) - (a.x - center.x) * (b.y - center.y));
            }
        }
    }
}