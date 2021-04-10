using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Pipeline.Geometry
{
    public class LineLineInteractor : IGeometryInteractor<OwLine, OwLine>
    {
        private static LineLineInteractor instance;

        private LineLineInteractor()
        {
        }

        public static LineLineInteractor use()
        {
            return instance ?? (instance = new LineLineInteractor());
        }

        /// <summary>
        /// One line is fully contained in another line if both of the lines end or start at the same spot.
        /// The lines may go in the opposite directions.
        /// </summary>
        /// <param name="first">contained in</param>
        /// <param name="second">contained</param>
        /// <returns>true : if second is contained in first</returns>
        public bool Contains(OwLine first, OwLine second)
        {
            if (first.Start == second.Start && first.End == second.End
                || first.Start == second.End && first.End == second.Start)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Two lines are partially contained if they are both on the same line in space.
        /// </summary>
        /// <param name="first">contained in</param>
        /// <param name="second">contained</param>
        /// <returns></returns>
        public bool PartiallyContains(OwLine first, OwLine second)
        {
            Vector3 A = new Vector3(first.Start.x, first.Start.y, 0);
            Vector3 B = new Vector3(first.End.x, first.End.y, 0);

            Vector3 C = new Vector3(first.Start.x, first.Start.y, 0);
            Vector3 D = new Vector3(first.End.x, first.End.y, 0);

            Vector3 AB = B - A;
            Vector3 AC = C - A;
            Vector3 AD = D - A;

            Vector3 crossProduct0 = Vector3.Cross(AB, AD);
            Vector3 crossProduct1 = Vector3.Cross(AB, AC);

            return (crossProduct0 == crossProduct1) && crossProduct0 == Vector3.zero;
        }

        public OwLine CalculateShortestPath(OwLine first, OwLine second)
        {
            OwPoint A = new OwPoint(first.Start);
            OwPoint B = new OwPoint(first.End);
            
            OwPoint C = new OwPoint(second.Start);
            OwPoint D = new OwPoint(second.End);

            OwLine d0 = LinePointInteractor.use().CalculateShortestPath(first, C);
            OwLine d1 = LinePointInteractor.use().CalculateShortestPath(first, D);
            
            OwLine d2 = LinePointInteractor.use().CalculateShortestPath(second, A);
            OwLine d3 = LinePointInteractor.use().CalculateShortestPath(second, B);

            List<OwLine> all = new List<OwLine>() {d0, d1, d2, d3};
            all.Sort((line, owLine) => (int) (200 * (line.Length() - owLine.Length())));

            return all.First();
        }

        public IGeometry Intersect(OwLine first, OwLine second)
        {
            const double eps = 0.000001d;

            Vector2 CD = second.End - second.Start;
            Vector2 AB = first.End - first.Start;
            Vector2 AC = second.Start - first.Start;

            double det = 1d / (AB.y * CD.x - AB.x * CD.y);

            double alpha = det * (-CD.y * AC.x + CD.x * AC.y);
            double beta = det * (-AB.y * AC.x + AB.x * AC.y);

            if (0 < alpha && alpha < 1 && alpha > eps && 1 - alpha > eps && beta > eps && 1 - beta > eps && 0 < beta &&
                beta < 1)
            {
                var result =  first.Start + AB * (float) alpha;
                return new OwPoint(result);
            }

            return new OwInvalidGeometry();
        }

        public float CalculateDistance(OwLine first, OwLine second)
        {
            return CalculateShortestPath(first, second).Length();
        }
    }
}