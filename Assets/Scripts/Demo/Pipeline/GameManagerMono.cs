using System;
using System.Collections.Generic;
using Demo.Pipeline;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Demo
{
    public class GameManagerMono : MonoBehaviour
    {
        public GameObject clearedLevelText;

        public GameObject pipelineManagerObject;
        public Image progressCircleImage;

        private void Start()
        {
            PipelineManager manager = pipelineManagerObject.GetComponent<PipelineManager>();
            ConnectedAreaTrigger.SetAllToFalse();
            manager.seed = Environment.TickCount;
            manager.Setup();
            manager.Generate();
            
            //assign progress canvas/image to every trigger
            ConnectedAreaTrigger[] triggers = pipelineManagerObject.GetComponentsInChildren<ConnectedAreaTrigger>();
            foreach (ConnectedAreaTrigger trigger in triggers)
            {
                trigger.progressCircleImage = progressCircleImage;
            }
        }

        private void Update()
        {
            if (GameManager.Get().foundAreas == GameManager.Get().uniqueAreasAmount)
            {
                clearedLevelText.SetActive(true);
            }
        }
    }
}