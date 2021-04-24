using System.Collections.Generic;

namespace Framework.Pipeline.Geometry.Interactors
{
    public interface IGeometryInteractor<A, B> where A : IGeometry where B : IGeometry
    {
        /// <summary>
        /// Evaluates whether the first IGeometry object fully contains the second one
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true or false</returns>
        bool Contains(A first, B second);

        /// <summary>
        /// Evaluates whether the first IGeometry object partially contains the second one
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true or false</returns>
        bool PartiallyContains(A first, B second);

        /// <summary>
        /// Calculates the shortest path between the first and the second IGeometry objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>a line</returns>
        OwLine CalculateShortestPath(A first, B second);

        /// <summary>
        /// Calculates the intersection of the first and the second IGeometry objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>the appropriate IGeometry objects</returns>
        IEnumerable<IGeometry> Intersect(A first, B second);

        /// <summary>
        /// Calculates the distance between the first and the second IGeometry objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>the distance</returns>
        float CalculateDistance(A first, B second);
    }
}