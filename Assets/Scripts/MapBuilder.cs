using System.Linq;
using Evolutionary.Framework;
using Evolutionary.Framework.Standard;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private int height;
    private int width;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject floorPrefab;
    
    void Awake()
    {
        var algorithm = GetComponent<StandardAlgorithmRunner>();
        height = algorithm.height;
        width = algorithm.width;

        algorithm.ApplyEvolution();
        var population = algorithm.GetPopulation();
        
        DrawRepresentation(population.First());
    }

    public void DrawRepresentation(IIndividual<StandardGenoPhenoCombination> individual)
    {
        var map = individual.Representation.Map;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //draw floor
                if (map[y, x] == 0)
                {
                    var localScale = floorPrefab.transform.localScale;
                    var position = new Vector3(x * localScale.x, 0, y * localScale.y);
                    Instantiate(floorPrefab, position, Quaternion.identity);
                }

                //draw player
                if (map[y, x] == 1)
                {
                    var localScale = floorPrefab.transform.localScale;
                    var position = new Vector3(x * localScale.x, 0, y * localScale.y);
                    Instantiate(floorPrefab, position, Quaternion.identity);

                    Instantiate(playerPrefab, position + Vector3.up, Quaternion.identity);
                }

                //draw enemy
                if (map[y, x] == 2)
                {
                    var localScale = floorPrefab.transform.localScale;
                    var position = new Vector3(x * localScale.x, 0, y * localScale.y);
                    Instantiate(floorPrefab, position, Quaternion.identity);

                    Instantiate(enemyPrefab, position + Vector3.up, Quaternion.identity);
                }
            }
        }
    }
}