using System.Collections.Generic;
using System.Linq;
using Framework.Cellular_Automata;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Framework.Poisson_Disk_Sampling;
using Polybool.Net.Objects;
using Tektosyne.Geometry;
using UnityEngine;

namespace Framework.Pipeline.Example
{
    public class AreaRefinementStep : IPipelineStep
    {
        public bool IsValidStep(IPipelineStep prev)
        {
            return true;
        }

        public GameWorld Apply(GameWorld world)
        {

            var r = 4f;
            
            //CA
            List<Vector2> caPoints = PoissonDiskSampling.GeneratePoints(r, 50f, 50f).ToList();
            NaturalCa naturalCa = new NaturalCa(caPoints.Count());

            //add all points as cell with their index
            for (int i = 0; i < caPoints.Count; i++)
            {
                naturalCa.AddCell(new NaturalCaCell(i, naturalCa, caPoints[i]));
            }

            for (int index0 = 0; index0 < caPoints.Count; index0++)
            {
                Vector2 caPoint0 = caPoints[index0];
                for (int index1 = 0; index1 < caPoints.Count; index1++)
                {
                    Vector2 caPoint1 = caPoints[index1];
                    if (caPoint0 != caPoint1)
                    {
                        float d = (caPoint0 - caPoint1).magnitude;
                        if (d < r * r/2f)
                        {
                            (naturalCa.Cells[index0] as NaturalCaCell)?.AddNeighbour(index1);

                            //world.Root.AddChild(new Subsidiary(new OwLine(caPoint0, caPoint1)));
                        }
                    }
                }
            }

            foreach (CaCell cell in naturalCa.Cells)
            {
                NaturalCaCell naCell = (cell as NaturalCaCell);
                foreach (IGameWorldObject potentialArea in world.Root.GetChildren())
                {
                    if (potentialArea is Area area)
                    {
                        OwPoint owPoint = new OwPoint(naCell.Position);
                        if (PolygonPointInteractor.Use().Contains(area.Shape as OwPolygon, owPoint))
                        {
                            naCell.State = true;
                        }
                    }
                }
            }

            naturalCa.Run(1);
  
            List<Vector2> activeCellPositions = new List<Vector2>();
            OwPolygon areaPolygon = world.Root.GetChildren().First(x => (x as Area).Shape is OwPolygon).Shape as OwPolygon;

            for (int index = 1; index < naturalCa.Cells.Length; index++)
            {
                CaCell caCell = naturalCa.Cells[index];
                NaturalCaCell naturalCaCell = (NaturalCaCell) caCell;
                if (naturalCaCell.State)
                {
                    activeCellPositions.Add(naturalCaCell.Position);

                    OwCircle partArea = new OwCircle(naturalCaCell.Position, r, 8);
                    //segments.Add(partArea.representation);
                    areaPolygon = PolygonPolygonInteractor.Use().Union(areaPolygon, partArea);

                    world.Root.AddChild(new Subsidiary(new OwPoint(naturalCaCell.Position)));
                }
            }

            Area newGround = new Area(areaPolygon);
            
            foreach (IGameWorldObject children in world.Root.GetChildren().ToArray())
            {
                if (children is Area area)
                {
                    //transfer children if any
                    if (area.GetChildCount() > 0)
                    {
                        IEnumerable<IGameWorldObject> areaChildren = area.GetChildren();
                        foreach (IGameWorldObject areaChild in areaChildren)
                        {
                            newGround.AddChild(areaChild);
                        }
                    }
                    
                    world.Root.RemoveChild(area);

                    areaPolygon = PolygonPolygonInteractor.Use().Union(areaPolygon, area.Shape as OwPolygon);
                }
            }

            newGround.Shape = areaPolygon;
            
            world.Root.AddChild(newGround);


            return world;
        }
    }
}