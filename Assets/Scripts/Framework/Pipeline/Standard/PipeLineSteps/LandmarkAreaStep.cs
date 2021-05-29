using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Pipeline.PipeLineSteps;
using Framework.Poisson_Disk_Sampling;
using Polybool.Net.Objects;
using UnityEngine;

namespace Framework.Pipeline.Standard.PipeLineSteps
{
    [LandmarksPlacedGuarantee]
    public class LandmarkAreaStep : PipelineStep
    {
        [Range(0, 1)] public float landMarkIsAreaPercentage;
        public int areaXTimes = 2;
        public int maxCircleSize;
        public int minCircleSize;
        public int circleResolution;
        public float radiusP;
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
                areas.Where(area => area.HasAnyChildrenOfType<Landmark>() && area.Type != "start" && area.Type != "end")
                    .ToList();
            int areasWithLandmarksSum = (int) (areasWithLandmarks.Count() * landMarkIsAreaPercentage);
            int pairs = areasWithLandmarksSum / areaXTimes;

            GameManager.uniqueAreasAmount = pairs;

            //create pairs / triples / quadruples etc..
            for (int pair = 0; pair < pairs; pair++)
            {
                //create unique Landmark
                AreaTypeAssignmentStep.TypedArea[] chosenAreas = GetConnectedChosenAreas(areasWithLandmarks);
                Landmark[] chosenLandmarks = new Landmark[areaXTimes];
                OwPolygon[] movedCircle = new OwPolygon[areaXTimes];

                for (int index = 0; index < areaXTimes; index++)
                {
                    List<Landmark> allLandmarksInArea = chosenAreas[index].GetAllChildrenOfType<Landmark>().ToList();
                    chosenLandmarks[index] =
                        allLandmarksInArea[(int) (random.NextDouble() * allLandmarksInArea.Count())];
                }

                OwPolygon uniqueShape = GetUniqueShape(chosenAreas, chosenLandmarks);

                for (int index = 0; index < areaXTimes; index++)
                {
                    movedCircle[index] = new OwPolygon(uniqueShape.representation);
                    movedCircle[index].MovePolygon(chosenLandmarks[index].Shape.GetCentroid());
                }

                /*bool shapeFits = false;
            int shapeTries = 0;
            OwPolygon uniqueShape = GetUniqueShape();

            do
            {
                tries = 0;
                //check if unique shape fits into area
                do
                {
                    //test for each chosen area a random landmark and check if it fits
                    for (int index = 0; index < chosenAreas.Length; index++)
                    {
                        AreaTypeAssignmentStep.TypedArea chosenArea = chosenAreas[index];

                        chosenLandmarks[index] =
                           
                        movedCircle[index] = new OwPolygon(uniqueShape.representation);
                        movedCircle[index].MovePolygon(chosenLandmarks[index].Shape.GetCentroid());

                        if (PolygonPolygonInteractor.Use().Contains(chosenArea.Shape as OwPolygon, movedCircle[index]))
                        {
                            isInside[index] = true;
                        }
                    }

                    tries++;
                } while (isInside.All(inside => inside) && tries < safetyMaxTries);

                //if this shape did not fit inside after trying safetyMaxTries time, then try a new shape
                shapeTries++;

                if (tries >= safetyMaxTries)
                {
                    uniqueShape = GetUniqueShape();
                }
                else
                {
                    shapeFits = true;
                }
            } while (!shapeFits && shapeTries < safetyMaxTries);

            if (shapeTries >= safetyMaxTries)
            {
                Debug.LogError("Max tries where reached when trying to place an area that is inside of outer area");
            }*/

                for (int index = 0; index < areaXTimes; index++)
                {
                    Landmark chosenLandmarkI = chosenLandmarks[index];
                    OwPolygon movedCircleI = movedCircle[index];
                    chosenLandmarkI.Shape = movedCircleI;
                    chosenLandmarkI.Type = $"landmarkPair{pair}";
                    areasWithLandmarks.Remove(chosenAreas[index]);

                    List<Landmark> allLandmarksInArea = chosenAreas[index].GetAllChildrenOfType<Landmark>().ToList();
                    allLandmarksInArea.Remove(chosenLandmarkI);

                    foreach (Landmark otherLandmark in allLandmarksInArea)
                    {
                        if (otherLandmark.Shape is OwPoint)
                        {
                            if (PolygonPointInteractor.Use().Contains(movedCircleI, otherLandmark.Shape as OwPoint))
                            {
                                chosenAreas[index].RemoveChild(otherLandmark);
                            }
                        }
                    }
                }
            }

            return world;
        }

        private AreaTypeAssignmentStep.TypedArea[] GetConnectedChosenAreas(
            List<AreaTypeAssignmentStep.TypedArea> areasWithLandmarks)
        {
            int[] areaIndices = new int[areaXTimes];
            
            if (areasWithLandmarks.Count == areaXTimes)
            {
                for (int i = 0; i < areaXTimes; i++)
                {
                    areaIndices[i] = i;
                }
            }
            else
            {
                do
                {
                    for (int i = 0; i < areaXTimes; i++)
                    {
                        areaIndices[i] = (int) (random.NextDouble() * (areasWithLandmarks.Count - 1));
                    }
                } while (areaIndices.Distinct().Count() != areaIndices.Length);
            }

            AreaTypeAssignmentStep.TypedArea[] chosenAreas = new AreaTypeAssignmentStep.TypedArea[areaXTimes];
            for (int i = 0; i < areaXTimes; i++)
            {
                chosenAreas[i] = areasWithLandmarks[areaIndices[i]];
            }
            return chosenAreas;
        }

        private OwPolygon GetUniqueShape(AreaTypeAssignmentStep.TypedArea[] chosenAreas, Landmark[] chosenLandmarks)
        {
            IEnumerable<Vector2> points = PoissonDiskSampling.GeneratePoints(radiusP, sizeP, sizeP, rejectionP, random);
            OwPolygon baseCircle = new OwCircle(Vector2.zero, 1f, circleResolution);

            foreach (Vector2 point in points)
            {
                int circleSize = (int) (random.NextDouble() * (maxCircleSize - minCircleSize) + minCircleSize);
                OwCircle circle = new OwCircle(point, circleSize, circleResolution);
                baseCircle = PolygonPolygonInteractor.Use().Union(baseCircle, circle);

                //bool[] inside = new bool[areaXTimes];


                //if all areas can contain this point at given landmark, move forward with this shape
                /*if (inside.All(x => x))
            {*/
                //baseCircle = newCircle;
                /*}*/
            }

            /*for (int i = 0; i < areaXTimes; i++)
        {
            Vector2 landmarkPos = chosenLandmarks[i].Shape.GetCentroid();
            baseCircle.MovePolygon(landmarkPos);
            baseCircle = PolygonPolygonInteractor.Use()
                .Intersection(chosenAreas[i].Shape as OwPolygon, baseCircle);
        }*/

            return baseCircle;
        }
    }
}