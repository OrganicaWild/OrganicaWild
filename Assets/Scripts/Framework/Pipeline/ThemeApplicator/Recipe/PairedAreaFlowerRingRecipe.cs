using System;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Util;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.ThemeApplicator.Recipe
{
    [CreateAssetMenu(fileName = "PairedAreaFlowerRingRecipe", menuName = "Pipeline/PairedAreaFlowerRingRecipe", order = 0)]
    public class PairedAreaFlowerRingRecipe : GameWorldObjectRecipe
    {
        public Material floorMaterial;
        public int rings;
        public float minRadius;
        public float maxRadius;
        public int ringResolution;
        public float staticScaleFactor;
        public GameObject[] prefabs;
        public GameObject[] centerPiecePrefabs;
        public GameObject toSpawn;
        public float secondsToSpawn;
        
        public override GameObject Cook(IGameWorldObject individual)
        {
            OwPolygon areaShape = individual.Shape as OwPolygon;
            GameObject mesh;
            try
            {
                mesh = GameObjectCreation.CombineMeshesFromPolygon(areaShape, floorMaterial);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                mesh = new GameObject();
            }

            mesh.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

            Vector2 center2d = individual.Shape.GetCentroid();
            Vector3 center3d = new Vector3(center2d.x, 0, center2d.y);
            
            //create trigger area
            Rect boundingBox = areaShape.GetBoundingBox();
            BoxCollider collider = mesh.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = ((Vector3)boundingBox.center) + new Vector3(0,0,-10);
            collider.size = new Vector3(boundingBox.width, boundingBox.height, 20);

            ConnectedAreaTrigger connectedAreaTrigger = mesh.AddComponent<ConnectedAreaTrigger>();
            string groupString = $"{individual.Type.Last()}";
            connectedAreaTrigger.partOfGroupX = int.Parse(groupString);
            connectedAreaTrigger.toSpawn = toSpawn;
            connectedAreaTrigger.spawnPoint = new Vector3(boundingBox.center.x, 2, boundingBox.center.y);
            connectedAreaTrigger.secondsToWait = secondsToSpawn;

            Random lRandom = new Random(individual.Type.Sum(c => c) + areaShape.GetPoints().Sum(p => (int) Mathf.Floor((p - areaShape.GetCentroid()).magnitude)));
            float radiusPerRing = (maxRadius - minRadius) / rings;
            
            for (int i = 1; i <= rings; i++)
            {
                OwCircle circle = new OwCircle(areaShape.GetCentroid(), minRadius + radiusPerRing * i, ringResolution);
                float scale = (maxRadius - i * radiusPerRing) * staticScaleFactor;
                //float scale = 1;
                
                foreach (Vector2 point in circle.GetPoints())
                {
                    Vector2 offset = (Vector2Extensions.GetRandomNormalizedVector(lRandom) - new Vector2(0.5f, 0.5f)) / (maxRadius - i * radiusPerRing);
                    Vector2 vector2 = point;
                    vector2 += offset;

                    Vector3 worldPos = new Vector3(vector2.x, 0, vector2.y);

                    var prefab = prefabs[(int) (lRandom.NextDouble() * prefabs.Length)];
                    GameObject instantiated =
                        Instantiate(prefab,
                            worldPos, Quaternion.identity);
                    instantiated.transform.parent = mesh.transform;
                    instantiated.transform.localScale = prefab.transform.localScale * scale;
                }
            }

            Vector3 center = new Vector3(areaShape.GetCentroid().x, 0, areaShape.GetCentroid().y);
            GameObject centerPiece =
                Instantiate(centerPiecePrefabs[(int) (lRandom.NextDouble() * centerPiecePrefabs.Length)],
                   center , Quaternion.identity);
            centerPiece.transform.parent = mesh.transform;

            return mesh;
        }
    }
}