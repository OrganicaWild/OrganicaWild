using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipelineGuarantees;
using Framework.Pipeline.PipeLineSteps;
using Framework.Poisson_Disk_Sampling;
using Framework.Util;
using UnityEngine;

[LandmarksPlacedGuarantee]
public class LandmarkAreaStep : PipelineStep
{
    [Range(0,1)]
    public float landMarkIsAreaPercentage;
    public int areaXTimes = 2;
    public int maxCircleSize;
    public int minCircleSize;
    public int circleResolution;
    public int radiusP;
    public int sizeP;
    public int rejectionP;

    public override Type[] RequiredGuarantees => new Type[] {typeof(LandmarksPlacedGuarantee)};

    public override GameWorld Apply(GameWorld world)
    {
        IEnumerable<AreaTypeAssignmentStep.TypedArea> areas =
            world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>();

        List<AreaTypeAssignmentStep.TypedArea> areasWithLandmarks =
            areas.Where(area => area.HasAnyChildrenOfType<Landmark>()).ToList();
        int areasWithLandmarksSum = (int) (areasWithLandmarks.Count() * landMarkIsAreaPercentage);
        int pairs = areasWithLandmarksSum / 2;

        //create pairs / triples / quadruples etc..
        for (int i = 0; i < pairs; i++)
        {
            //create unique Landmark
            IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(radiusP, sizeP, sizeP, rejectionP);
            OwPolygon baseCircle = new OwCircle(Vector2.zero, 1f, 20);
            
            foreach (Vector2 point in points)
            {
                int circleSize = (int) (random.NextDouble() * (maxCircleSize - minCircleSize) + minCircleSize);
                OwCircle circle = new OwCircle(point, circleSize, circleResolution);
                baseCircle = PolygonPolygonInteractor.Use().Union(baseCircle, circle);
            }

            for (int j = 0; j < areaXTimes; j++)
            {
               AreaTypeAssignmentStep.TypedArea chosenArea = areasWithLandmarks[(int) (random.NextDouble() * (areasWithLandmarks.Count - 1))];
               areasWithLandmarks.Remove(chosenArea);
               IEnumerable<Landmark> allLandmarksInArea = chosenArea.GetAllChildrenOfType<Landmark>();
               Landmark chosenLandmark = allLandmarksInArea.Skip((int) (random.NextDouble()) * (allLandmarksInArea.Count()))
                   .First();
               Vector2 landmarkPos = chosenLandmark.Shape.GetCentroid();
               OwPolygon movedCircle  = new OwPolygon(baseCircle.representation);
               movedCircle.MovePolygon(chosenLandmark.Shape.GetCentroid());
               chosenLandmark.Shape = movedCircle;
               
            }
        }
        
        return world;
    }
}