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
using Polybool.Net.Objects;
using UnityEngine;

[LandmarksPlacedGuarantee]
public class LandmarkAreaStep : PipelineStep
{ 
    [Range(0, 1)] public float landMarkIsAreaPercentage;
    public int areaXTimes = 2;
    public int maxCircleSize;
    public int minCircleSize;
    public int circleResolution;
    public int radiusP;
    public int sizeP;
    public int rejectionP;
    public int safetyMaxTries;
    
    public decimal epsilon = 0.0000000000000001m;

    public override Type[] RequiredGuarantees => new Type[] {typeof(LandmarksPlacedGuarantee)};

    public override GameWorld Apply(GameWorld world)
    {
        Epsilon.Eps = epsilon;
        
        IEnumerable<AreaTypeAssignmentStep.TypedArea> areas =
            world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>();

        List<AreaTypeAssignmentStep.TypedArea> areasWithLandmarks =
            areas.Where(area => area.HasAnyChildrenOfType<Landmark>() && area.Type != "start" && area.Type != "end").ToList();
        int areasWithLandmarksSum = (int) (areasWithLandmarks.Count() * landMarkIsAreaPercentage);
        int pairs = areasWithLandmarksSum / 2;

        //create pairs / triples / quadruples etc..
        for (int i = 0; i < pairs; i++)
        {
            //create unique Landmark
            OwPolygon uniqueShape = GetUniqueShape();

            for (int j = 0; j < areaXTimes; j++)
            {
                OwPolygon movedCircle;
                Landmark chosenLandmark;
                AreaTypeAssignmentStep.TypedArea chosenArea;
                int tries = 0;
                List<Landmark> allLandmarksInArea;
                do
                {
                    tries++;
                    chosenArea =
                        areasWithLandmarks[(int) (random.NextDouble() * (areasWithLandmarks.Count - 1))]; 
                    allLandmarksInArea = chosenArea.GetAllChildrenOfType<Landmark>().ToList();
                    chosenLandmark = allLandmarksInArea[(int) (random.NextDouble()) * (allLandmarksInArea.Count())];
                    movedCircle = new OwPolygon(uniqueShape.representation);
                    movedCircle.MovePolygon(chosenLandmark.Shape.GetCentroid());
                } while (!PolygonPolygonInteractor.Use().Contains(chosenArea.Shape as OwPolygon, movedCircle) && tries <= safetyMaxTries);

                if (tries == safetyMaxTries)
                {
                    Debug.LogError("Max tries where reached when trying to place an area that is inside of outer area");
                }
                
                chosenLandmark.Shape = movedCircle;
                chosenLandmark.Type = $"landmarkPair{i}";
                areasWithLandmarks.Remove(chosenArea);
                
                //remove chosen landmark from list, so that is is not checked against in below loop
                allLandmarksInArea.Remove(chosenLandmark);

                foreach (Landmark otherLandmark in allLandmarksInArea)
                {
                    if (PolygonPointInteractor.Use().Contains(movedCircle, otherLandmark.Shape as OwPoint))
                    {
                        chosenArea.RemoveChild(otherLandmark);
                    }
                }
            }
        }

        return world;
    }

    private OwPolygon GetUniqueShape()
    {
        IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(radiusP, sizeP, sizeP, rejectionP, random);
        OwPolygon baseCircle = new OwCircle(Vector2.zero, 1f, 20);

        foreach (Vector2 point in points)
        {
            int circleSize = (int) (random.NextDouble() * (maxCircleSize - minCircleSize) + minCircleSize);
            OwCircle circle = new OwCircle(point, circleSize, circleResolution);
            baseCircle = PolygonPolygonInteractor.Use().Union(baseCircle, circle);
        }

        return baseCircle;
    }
}