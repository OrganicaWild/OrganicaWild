using System;
using Demo.Pipeline;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class GameManagerMono : MonoBehaviour
    {
        public Text clearedLevelText;

        public GameObject pipelineManagerObject;

        private void Start()
        {
            PipelineManager manager = pipelineManagerObject.GetComponent<PipelineManager>();
            manager.seed = Environment.TickCount;
            manager.Setup();
            manager.Generate();
        }

        private void Update()
        {
            if (GameManager.Get().foundAreas == GameManager.Get().uniqueAreasAmount)
            {
                clearedLevelText.text = "Du hast alle Gebiete gefunden. Gehe zum Ausgang!";
            }
        }
    }
}