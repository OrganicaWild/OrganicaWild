using Demo.Pipeline;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour
{
    public Text scoreText;

    private int prevScore = -1;
    // Update is called once per frame
    void Update()
    {
        if (prevScore != GameManager.Get().foundAreas)
        {
            int totalAreas = GameManager.Get().uniqueAreasAmount;
            int foundAreas = GameManager.Get().foundAreas;

            scoreText.text = $"Gebiete gefunden: {foundAreas}/{totalAreas}";
            prevScore = foundAreas;
        }
    }
}
