using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline.Geometry;
using UnityEngine;

public class RectangleGenerator : MonoBehaviour, IPolygonGenerator
{
    public Vector2 first;
    public Vector2 second;

    public RectangleGenerator() {}

    public static RectangleGenerator WithCenterAndSize(Vector2 center, Vector2 size)
    {
        RectangleGenerator generator = new RectangleGenerator();
        generator.first = center - size / 2;
        generator.second = center + size / 2;
        return generator;
    }

    public static RectangleGenerator WithTwoPoint(Vector2 start, Vector2 end)
    {
        RectangleGenerator generator = new RectangleGenerator();
        generator.first = start;
        generator.second = end;
        return generator;
    }

    public OwPolygon Generate()
    {
        return new OwRectangle(first, second);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
