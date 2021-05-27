using System;
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
        if (prevScore != GameManager.foundAreas)
        {
            int totalAreas = GameManager.uniqueAreasAmount;
            int foundAreas = GameManager.foundAreas;

            scoreText.text = $"{foundAreas} von {totalAreas} einzigartigen Gebietsarten gefunden";
            prevScore = foundAreas;
        }
    }
}
