using System;
using Demo.Pipeline;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class GameManagerMono : MonoBehaviour
    {
        public Text clearedLevelText;
        private void Update()
        {
            if (GameManager.Get().foundAreas == GameManager.Get().uniqueAreasAmount)
            {
                clearedLevelText.text = "Du hast alle Gebiete gefunden. Gehe zum Ausgang!";
            }
        }
    }
}