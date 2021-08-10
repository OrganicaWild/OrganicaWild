using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.PipeLineSteps;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [LandmarksPlacedGuarantee]
    public class LandmarkPlacementStep : PipelineStep
    {
        [Range(0, 1)] public float hasLandmarksPercent;
        public int minLandmarks;
        public int maxLandmarks;

        public int maxDistanceFromCenter;
        public int minDistanceFromCenter;

        [Range(0,1)]
        public float freeSpaceInsideAtBorder;
        public float minimumDistanceBetweenLandmarks;
        public int maxTriesToGuaranteeConstraints;

        public bool addDebugScaledInnerArea;

        public override Type[] RequiredGuarantees => new Type[] {typeof(AreaConnectionsGuarantee)};
        
        public override bool AddToDebugStackedView => true;

        public override GameWorld Apply(GameWorld world)
        {
            List<Area> areas =
                world.Root.GetAllChildrenOfType<Area>().ToList();

            foreach (Area area in areas)
            {
                Vector2 rectangleSize = new Vector2(5, 5);
                OwCircle circle0 = new OwCircle(area.GetShape().GetCentroid(), 5f, 8);
                OwCircle circle1 = new OwCircle(area.GetShape().GetCentroid()+ new Vector2(2,2), 5f, 8);
                OwPolygon result = PolygonPolygonInteractor.Use().Union(circle0, circle1);
                
                //start Area
                if (area.Identifier == "startArea")
                {
                    //add landmark at centroid
                    area.AddChild(new Landmark(result, "Spawn"));
                    continue;
                }

                //end area
                if (area.Identifier == "endArea")
                {
                    //add landmark at centroid
                    area.AddChild(new Landmark(result, "Goal"));
                    continue;
                }

                bool hasLandmark = random.NextDouble() <= hasLandmarksPercent;

                if (hasLandmark)
                {
                    int numberOfLandmarks = (int) (random.NextDouble() * (maxLandmarks - minLandmarks) + minLandmarks);
                    List<OwPoint> placedPoints = new List<OwPoint>();

                    OwPolygon areaShape = area.GetShape();
                    OwPolygon scaledPolygon = new OwPolygon(areaShape.Representation);
                    scaledPolygon.ScaleFromCentroid(new Vector2(1 - freeSpaceInsideAtBorder,
                        1 - freeSpaceInsideAtBorder));

                    if (addDebugScaledInnerArea)
                    {
                        area.AddChild(new Area(scaledPolygon, null));
                    }

                    for (int i = 0; i < numberOfLandmarks; i++)
                    {
                        OwPoint potentialLandMarkPoint;
                        bool isTooClose;
                        int tries = 0;
                        bool isInside;

                        // passing a function via constructor as we planned seems kinda hard . ngl
                        do
                        {
                            potentialLandMarkPoint = CreatePotentialLandMarkPoint(area);

                            //if any of the already added points are too close, consider this points as too close
                            isTooClose = placedPoints.Sum(point =>
                                ((point.Position - potentialLandMarkPoint.Position).magnitude <
                                 minimumDistanceBetweenLandmarks)
                                    ? 1
                                    : 0) > 0;

                            tries++;

                            
                            isInside = PolygonPointInteractor.Use().Contains(scaledPolygon, potentialLandMarkPoint);
                        } while ((!isInside || isTooClose) && tries < maxTriesToGuaranteeConstraints);

                        // if we are here and the proposed point is still not good we have reached max tries limit
                        if (!isInside || isTooClose)
                        {
                            // search for new point without the distance constraint
                            do
                            {
                                potentialLandMarkPoint = CreatePotentialLandMarkPoint(area);
                                isInside = PolygonPointInteractor.Use().Contains(scaledPolygon, potentialLandMarkPoint);
                            } while (!isInside);

                        }
                        
                        //if tries are reached, show warning and still add point, since we want to guarantee that the point is placed
                        if (tries == maxTriesToGuaranteeConstraints)
                        {
                            //Debug.LogWarning("Could not place Landmark with number of allowed tries");
                        }

                        //add landmark
                        area.AddChild(new Landmark(potentialLandMarkPoint, "genericLandmark"));
                        placedPoints.Add(potentialLandMarkPoint);
                    }
                }
            }

            return world;
        }

        private OwPoint CreatePotentialLandMarkPoint(Area area)
        {
            OwPoint potentialLandMarkPoint;
            //generate vector with specified length into random direction
            Vector2 fromCentroidPos =
                new Vector2((float) (random.NextDouble() * 2 - 1),
                    (float) (random.NextDouble() * 2 - 1)).normalized;
            //scale by specified length
            float distanceFromCenter =
                (float) (random.NextDouble() * (maxDistanceFromCenter - minDistanceFromCenter) +
                         minDistanceFromCenter);
            fromCentroidPos *= distanceFromCenter;

            potentialLandMarkPoint = new OwPoint(area.GetShape().GetCentroid() + fromCentroidPos);
            return potentialLandMarkPoint;
        }
    }
}