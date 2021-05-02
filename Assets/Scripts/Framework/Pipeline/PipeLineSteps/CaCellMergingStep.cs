using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Demo.Pipeline.TrialPipeline.TrialCellularAutomata;
using Assets.Scripts.Framework.Pipeline.PipeLineSteps;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Geometry;
using Framework.Pipeline.Geometry.Interactors;
using Polybool.Net.Objects;
using UnityEngine;

public class CaCellMergingStep : PipelineStep
{
    public override Type[] RequiredGuarantees => new Type[0];
    public override GameWorld Apply(GameWorld world)
    {
        Epsilon.Eps = 0.0000000001m;
        
        IEnumerable<AreaTypeAssignmentStep.TypedArea> typedAreas = world.Root.GetAllChildrenOfType<AreaTypeAssignmentStep.TypedArea>();
     
        Parallel.ForEach(typedAreas, typedArea =>
        {
            Dictionary<uint, OwPolygon> polygons = new Dictionary<uint, OwPolygon>();
            IEnumerable<TrialAreaCell> cellAreas = typedArea.GetAllChildrenOfType<TrialAreaCell>().ToList();

            foreach (TrialAreaCell trialAreaCell in cellAreas.ToList())
            {
                uint state = (uint)trialAreaCell.Cell.CurrentState;

                if (polygons.ContainsKey(state))
                {
                    polygons[state] = PolygonPolygonInteractor.Use()
                        .Union(polygons[state], trialAreaCell.Shape as OwPolygon);
                }
                else
                {
                    polygons.Add(state, trialAreaCell.Shape as OwPolygon);
                }

                typedArea.RemoveChild(trialAreaCell);
            }

            foreach (KeyValuePair<uint, OwPolygon> typePolygonPair in polygons)
            {
                typedArea.AddChild(new Area(typePolygonPair.Value, $"{typePolygonPair.Key}"));
            }
        });

        return world;
    }
}
