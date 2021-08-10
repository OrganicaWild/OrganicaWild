using System.Collections;
using System.Collections.Generic;
using Framework.PerlinNoise;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    public Renderer textureRenderer;

    public int MapWidth, MapHeight;
    public float NoiseScale;
    public int Octaves, Lacunarity;
    public Vector2 Offset;

    [Range(0, 1)] public float Persistence;

    public int Seed;

    public void GenerateMap()
    {
        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(Seed, MapWidth, MapHeight, NoiseScale, Octaves, Persistence, Lacunarity, Offset);
        DrawTexture(TextureFromHeightMap(noiseMap));


    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }


    private void OnValidate()
    {
        MapWidth = MapWidth < 1 ? 1 : MapWidth;
        MapHeight = MapHeight < 1 ? 1 : MapHeight;
        Octaves = Octaves < 1 ? 1 : Octaves;
        Lacunarity = Lacunarity < 1 ? 1 : Lacunarity;
    }

    public void DrawTexture(Texture2D texture)
    {
        var width = texture.width;
        var height = texture.height;

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);

        var colorMap = new Color[width * height];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromColorMap(Color[] colormap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colormap);
        texture.Apply();
        return texture;
    }
}
