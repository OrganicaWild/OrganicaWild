using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline.PipeLineSteps
{
    [LandmarksPlacedGuarantee]
    public class LandmarkPlacementStep : MonoBehaviour, IPipelineStep
    {
        public Random random;

        public GameWorldObjectRecipe landmarkRecipe;
        public GameWorldObjectRecipe startLandmarkRecipe;
        public GameWorldObjectRecipe endLandmarkRecipe;

        [Range(0, 1)] public float hasLandmarksPercent;
        public int minLandmarks;
        public int maxLandmarks;

        public int maxDistanceFromCenter;
        public int minDistanceFromCenter;

        public float freeSpaceInsideAtBorder;
        public float minimumDistanceBetweenLandmarks;
        public int maxTriesToGuaranteeConstraints;

        public bool addDebugScaledInnerArea;

        public Type[] RequiredGuarantees => new Type[] {typeof(AreaConnectionsGuarantee)};

        public GameWorld Apply(GameWorld world)
        {
            List<AreaTypeAssignmentStep.TypedArea> areas =
                world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>().ToList();

            foreach (AreaTypeAssignmentStep.TypedArea typedArea in areas)
            {
                //start Area
                if (typedArea.AreaType == -1)
                {
                    //add landmark at centroid
                    typedArea.AddChild(new Landmark(new OwPoint(typedArea.Shape.GetCentroid()), startLandmarkRecipe));
                    continue;
                }
                
                //end area
                if (typedArea.AreaType == int.MaxValue)
                {
                    //add landmark at centroid
                    typedArea.AddChild(new Landmark(new OwPoint(typedArea.Shape.GetCentroid()), endLandmarkRecipe));
                    continue;
                }
                
                bool hasLandmark = random.NextDouble() <= hasLandmarksPercent;

                if (hasLandmark)
                {
                    int numberOfLandmarks = (int) (random.NextDouble() * (maxLandmarks - minLandmarks) + minLandmarks);
                    List<OwPoint> placedPoints = new List<OwPoint>();
                    
                    OwPolygon areaShape = typedArea.Shape as OwPolygon;
                    OwPolygon scaledPolygon = new OwPolygon(areaShape.representation);
                    scaledPolygon.ScaleFromCentroid(new Vector2(1 - freeSpaceInsideAtBorder,
                        1 - freeSpaceInsideAtBorder));
                    
                    if (addDebugScaledInnerArea)
                    {
                        typedArea.AddChild(new Area(scaledPolygon, null));
                    }

                    for (int i = 0; i < numberOfLandmarks; i++)
                    {
                        OwPoint potentialLandMarkPoint;
                        bool isTooClose;
                        int tries = 0;

                        //generate point going out from middle of area into certain direction until point is inside of area (most of the time this is the first point generated)
                        do
                        {
                            //generate vector with specified length into random direction
                            Vector2 fromCentroidPos =
                                new Vector2((float) (random.NextDouble() * 2 - 1), (float) (random.NextDouble() * 2 - 1)).normalized;
                            //scale by specified length
                            float distanceFromCenter =
                                (float) (random.NextDouble() * (maxDistanceFromCenter - minDistanceFromCenter) +
                                         minDistanceFromCenter);
                            fromCentroidPos *= distanceFromCenter;

                            potentialLandMarkPoint = new OwPoint(typedArea.Shape.GetCentroid() + fromCentroidPos);
                            
                            //if any of the already added points are too close, consider this points as too close
                            isTooClose = placedPoints.Sum(point =>
                                ((point.Position - potentialLandMarkPoint.Position).magnitude <
                                 minimumDistanceBetweenLandmarks)
                                    ? 1
                                    : 0) > 0;
                            
                            tries++;
                            
                        } while ((!PolygonPointInteractor.Use().Contains(scaledPolygon, potentialLandMarkPoint) ||
                                  isTooClose) && tries < maxTriesToGuaranteeConstraints);

                        //if tries are reached, show warning and still add point, since we want to guarantee that the point is placed
                        if (tries == maxTriesToGuaranteeConstraints)
                        {
                            Debug.LogWarning("Could not place Landmark with number of allowed tries");
                        }

                        //add landmark
                        typedArea.AddChild(new Landmark(potentialLandMarkPoint, landmarkRecipe));
                        placedPoints.Add(potentialLandMarkPoint);
                    }
                }
            }

            return world;
        }
    }
}