using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Cellular;
using Framework.Evolutionary;
using Framework.Evolutionary.Nsga2;
using Framework.ShapeGrammar;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo.ShapeGrammar.Combination
{
    public class ForestCaNetwork : CANetwork, INsga2Individual
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        private readonly float radiusPoisson;
        private readonly float widthPoisson;
        private readonly float heightPoisson;
        private readonly int numberOfRejections;
        private readonly int numberOfPoisson;
        private readonly ScriptableConnections connections;
        private readonly float initialFillPercentage;
        private readonly float radius;
        private readonly int minWidth;
        private readonly int minHeight;
        
        public ForestCaNetwork(ScriptableConnections connections, float initialFillPercentage, float radius,
            int minWidth, int minHeight, IFitnessFunction[] fitnessFunctions, int numberOfPoisson,
            int numberOfRejections, float heightPoisson, float widthPoisson, float radiusPoisson)
        {
            dominatedIndividuals = new List<INsga2Individual>();
            this.connections = connections;
            this.radius = radius;
            this.minWidth = minWidth;
            this.minHeight = minHeight;
            FitnessFunctions = fitnessFunctions;
            this.numberOfPoisson = numberOfPoisson;
            this.numberOfRejections = numberOfRejections;
            this.heightPoisson = heightPoisson;
            this.widthPoisson = widthPoisson;
            this.radiusPoisson = radiusPoisson;
            this.initialFillPercentage = initialFillPercentage;
            fitnessResults = new double[fitnessFunctions.Length];

            float minX = connections.corners.Min(x => x.connectionPoint.x);
            float minZ = connections.corners.Min(x => x.connectionPoint.z);

            float maxX = connections.corners.Max(x => x.connectionPoint.x);
            float maxZ = connections.corners.Max(x => x.connectionPoint.z);

            minX = Mathf.Min(connections.entryCorner.connectionPoint.x, minX);
            minZ = Mathf.Min(connections.entryCorner.connectionPoint.z, minZ);

            maxX = Mathf.Max(connections.entryCorner.connectionPoint.x, maxX);
            maxZ = Mathf.Max(connections.entryCorner.connectionPoint.z, maxZ);

            Width = Math.Max(minWidth, (int) (maxX - minX));
            Height = Math.Max(minHeight, (int) (maxZ - minZ));

            Start = new Vector2(minX, minZ);
            End = new Vector2(maxZ, maxZ);

            Cells = new CACell[Width * Height];
            Connections = new bool[Width * Height][];

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new ForestCell(i);
                CACell cell = Cells[i];
                cell.Network = this;
                float r = Random.value;
                if (r <= initialFillPercentage)
                {
                    ((ForestCell) cell).state = State.Water;
                }
                else if (r > initialFillPercentage || r <= 1)
                {
                    ((ForestCell) cell).state = State.Land;
                }
                else
                {
                    ((ForestCell) cell).state = State.Beach;
                }
            }

            FillAtPoint(connections.entryCorner.connectionPoint, radius);

            foreach (SpaceNodeConnection spaceNodeConnection in connections.corners)
            {
                FillAtPoint(spaceNodeConnection.connectionPoint, radius);
            }

            for (int i = 0; i < Width * Height; i++)
            {
                Connections[i] = new bool[Width * Height];
            }

            //Finish initialization for NSGA-2
            Run(25);
            AddBushes();
        }

        private void FillAtPoint(Vector3 point, float radius)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //offset position
                    Vector2 mappedPoint = GetMappedPoint(point);

                    //equation for circle
                    bool pointIsInCircle =
                        (i - mappedPoint.x) * (i - mappedPoint.x) +
                        (j - mappedPoint.y) * (j - mappedPoint.y) <= radius;

                    if (pointIsInCircle)
                    {
                        int index = j * Width + i;
                        ((ForestCell) Cells[index]).state = State.Land;
                    }
                }
            }
        }

        private void AddBushes()
        {
            for (var i = 0; i < numberOfPoisson; i++)
            {
                IEnumerable<Vector2> points =
                    PoissonDiskSampling.GeneratePoints(radiusPoisson, widthPoisson, heightPoisson, numberOfRejections);
                Vector3 randomPointOnMap = new Vector3(Random.Range(0, Width), 0,
                    Random.Range(0, Height));
                foreach (Vector2 point in points)
                {
                    Debug.Log(point);
                    SetMappedPosition(randomPointOnMap + new Vector3(point.x, 0, +point.y),
                        State.Bush, State.Land);
                }
            }
        }

        private Vector2 GetMappedPoint(Vector3 point)
        {
            float a = point.x - Start.x;
            float b = point.z - Start.y;
            return new Vector2(a, b);
        }

        public void SetMappedPosition(Vector3 point, State state, State ifState)
        {
            Vector2 mappedPosition = GetMappedPoint(point);
            int x = (int) mappedPosition.x % Width;
            int y = (int) mappedPosition.y % Height;

            int index = y * Width + x;
            ForestCell cell = Cells[index] as ForestCell;
            if (cell.state == ifState)
            {
                cell.state = state;
            }

            Cells[index] = cell;
        }

        public bool IsOnTopBorder(int index)
        {
            return index < Width;
        }

        public bool IsOnRightBorder(int index)
        {
            return index % Width == Width - 1;
        }

        public bool IsOnBottomBorder(int index)
        {
            return Width * (Height - 1) <= index;
        }

        public bool IsOnLeftBorder(int index)
        {
            return index % Width == 0;
        }

        public override IEnumerable<CACell> GetNeighborsOf(int cellNumber)
        {
            List<CACell> result = new List<CACell>();


            // The four directly adjacent neighbors
            if (!IsOnTopBorder(cellNumber))
            {
                // Top
                result.Add(Cells[cellNumber - Width]);
                if (!IsOnRightBorder(cellNumber))
                {
                    // Top Right
                    result.Add(Cells[cellNumber - Width + 1]);
                }
            }

            if (!IsOnRightBorder(cellNumber))
            {
                // Right
                result.Add(Cells[cellNumber + 1]);
                if (!IsOnBottomBorder(cellNumber))
                {
                    // Right Bottom
                    result.Add(Cells[cellNumber + Width + 1]);
                }
            }

            if (!IsOnBottomBorder(cellNumber))
            {
                // Bottom
                result.Add(Cells[cellNumber + Width]);
                if (!IsOnLeftBorder(cellNumber))
                {
                    // Bottom Left
                    result.Add(Cells[cellNumber - 1 + Width]);
                }
            }

            if (!IsOnLeftBorder(cellNumber))
            {
                // Left
                result.Add(Cells[cellNumber - 1]);
                if (!IsOnTopBorder(cellNumber))
                {
                    // Left Top
                    result.Add(Cells[cellNumber - Width - 1]);
                }
            }

            return result;
        }
        
        public INsga2Individual MakeOffspring(INsga2Individual parent2)
        {
            var other = parent2 as ForestCaNetwork;
            
            ForestCaNetwork child = new ForestCaNetwork(connections, initialFillPercentage, radius, minWidth, minHeight,
                FitnessFunctions, numberOfPoisson, numberOfRejections, heightPoisson, widthPoisson, radiusPoisson);
            for (var i = 0; i < Cells.Length / 2; i++)
            {
                child.Cells[i] = Cells[i];
            }

            for (var i = Cells.Length / 2; i < Cells.Length; i++)
            {
                child.Cells[i] = other.Cells[i];
            }

            return child;
        }

        //boilerplate from the INsga2Interface
        private IFitnessFunction[] FitnessFunctions;
        private double[] fitnessResults;
        private List<INsga2Individual> dominatedIndividuals;
        
        public void EvaluateFitness()
        {
            for (var index = 0; index < FitnessFunctions.Length; index++)
            {
                IFitnessFunction f = FitnessFunctions[index];
                fitnessResults[index] = f.DetermineFitness(this);
            }
        }

        public int GetNumberOfFitnessFunctions()
        {
            return FitnessFunctions.Length;
        }

        public double GetOptimizationTarget(int index)
        {
            if (index >= FitnessFunctions.Length || index < 0)
            {
                throw new IndexOutOfRangeException(
                    $"The Index is out of range for the Optimization Targets. Valid Range is {0} to {FitnessFunctions.Length - 1} ");
            }

            return fitnessResults[index];
        }

        public void PrepareForNextGeneration()
        {
            DominationCount = 0;
            Rank = 0;
            Crowding = 0;
            dominatedIndividuals = new List<INsga2Individual>();
        }
        
        public int Rank { get; set; }
        public double Crowding { get; set; }
        public int DominationCount { get; set; }

        public void AddDominatedIndividual(INsga2Individual dominated)
        {
            dominatedIndividuals.Add(dominated);
        }

        public IList<INsga2Individual> GetDominated()
        {
            return dominatedIndividuals;
        }
    }
}