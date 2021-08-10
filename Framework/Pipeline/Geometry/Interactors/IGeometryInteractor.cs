using System.Collections.Generic;

namespace Framework.Pipeline.Geometry.Interactors
{
    public interface IGeometryInteractor<A, B> where A : IGeometry where B : IGeometry
    {
        /// <summary>
        /// Evaluates whether the first IGeometry object fully contains the second one
        /// </summary>
        /// <param name="first">maybe contained</param>
        /// <param name="second">container</param>
        /// <returns>
        ///     true - if first is fully contained in second.
        ///     false - all other cases.
        /// </returns>
        bool Contains(A first, B second);

        /// <summary>
        /// Evaluates whether the first IGeometry object partially contains the second one
        /// </summary>
        /// <param name="first">maybe contained</param>
        /// <param name="second">container</param>
        /// <returns>
        ///  true - if first is partially contained in second.
        ///   false - all other cases.
        /// </returns>
        bool PartiallyContains(A first, B second);

        /// <summary>
        /// Calculates the shortest path between the first and the second IGeometry objects
        /// </summary>
        /// <param name="first">A</param>
        /// <param name="second">B</param>
        /// <returns>a line</returns>
        OwLine CalculateShortestPath(A first, B second);

        /// <summary>
        /// Returns the geometric shape where A and B both fully or partially are.
        /// </summary>
        /// <param name="first">A</param>
        /// <param name="second">B</param>
        /// <returns>intersection of A and B. The type of the intersection result depends on the supplied A and B</returns>
        IEnumerable<IGeometry> Intersect(A first, B second);

        /// <summary>
        /// Calculates the distance between the first and the second IGeometry objects.
        /// </summary>
        /// <param name="first">A</param>
        /// <param name="second">B</param>
        /// <returns>the length of the result of CalculateShortestPath(A,B)</returns>
        float CalculateDistance(A first, B second);
    }
}