namespace Framework.Pipeline.Geometry.Interactors
{
    public interface IGeometryInteractor<A, B> where A : IGeometry where B : IGeometry
    {
        bool Contains(A first, B second);

        bool PartiallyContains(A first, B second);

        OwLine CalculateShortestPath(A first, B second);

        IGeometry Intersect(A first, B second);

        float CalculateDistance(A first, B second);
    }
}