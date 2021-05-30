using UnityEngine;
using UnityEngine.UI;

namespace Demo.Pipeline
{
    public class ScoreUpdater : MonoBehaviour
    {
        public Text scoreText;

        private int prevScore = -1;

        // Update is called once per frame
        void Update()
        {
            int totalAreas = GameManager.uniqueAreasAmount;
            int foundAreas = GameManager.foundAreas;

            scoreText.text = $"{foundAreas} von {totalAreas} einzigartigen Gebietsarten gefunden";
            prevScore = foundAreas;
        }
    }
}