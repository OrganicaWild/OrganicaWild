using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Util
{
    public class Vector2Comparer : Comparer<Vector2>
    {
        /// <summary>
        /// Compares Vector 2 based on distance to origin
        /// </summary>
        /// <param name="x">vector0</param>
        /// <param name="y">vector1</param>
        /// <returns>
        ///             positve if x is closer than y to the origin
        ///             negative if y is closer than x to the origin
        ///             0 if both are the exact same distance to the origin
        /// </returns>
        public override int Compare(Vector2 x, Vector2 y)
        {
            return (int) (x.magnitude - y.magnitude);
        }
    }
}