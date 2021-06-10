using System.Collections.Generic;
using System.Linq;
using Framework.Util;
using UnityEngine;

namespace Polybool.Net.Objects
{
    public class Region
    {
        public List<Point> Points { get; set; }

        public bool IsClockWise()
        {
            decimal sum = 0;
            Point first = null;
            Point prev = null;
            foreach (Point point in Points)
            {
                if (first == null)
                {
                    first = point;
                    prev = point;
                }
                else
                {
                    sum += (point.X - prev.X) * (point.Y + prev.Y);
                    prev = point;
                }
            }
            sum += (prev.X - first.X) * (prev.Y + first.Y);

            return sum > 0;
        }

        public bool Equals(Region other)
        {
            if (other.Points.Count != Points.Count)
            {
                return false;
            }
            
            int startIndexOther = -1;
            int startIndexThis = -1;

            //TODO: this would not behave correctly if two points where on the same spot in the polygon. I don't know if this ever happens but maybe this could be an issue
            for (int indexThis = 0; indexThis < Points.Count; indexThis++)
            {
                Point point = Points[indexThis];
                for (int indexOther = 0; indexOther < other.Points.Count; indexOther++)
                {
                    Point otherPoint = other.Points[indexOther];
                    if (point.Equals(otherPoint))
                    {
                        startIndexOther = indexOther;
                        startIndexThis = indexThis;
                        break;
                    }
                }

                if (startIndexOther != -1)
                {
                    break;
                }
            }

            bool forwardFalse = false;
            bool backwardFalse = false;

            List<Point> shiftThisPoints = Points.ShiftLeft(startIndexThis);
            List<Point> shiftOtherPoints = other.Points.ShiftLeft(startIndexOther);


            for (int i = 0; i < shiftThisPoints.Count; i++)
            {
                if (!shiftThisPoints[i].Equals(shiftOtherPoints[i]))
                {
                    forwardFalse = true;
                }
            }

            shiftOtherPoints.Reverse(1, shiftOtherPoints.Count -1);
            
            for (int i = 0; i < shiftThisPoints.Count; i++)
            {
                if (!shiftThisPoints[i].Equals(shiftOtherPoints[i]))
                {
                    backwardFalse = true;
                }
            }

            return !forwardFalse || !backwardFalse;
        }
    }
}