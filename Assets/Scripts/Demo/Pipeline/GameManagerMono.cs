using System;
using System.Collections;
using Framework.Pipeline.Standard;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Demo.Pipeline
{
    public class GameManagerMono : MonoBehaviour
    {
        public GameObject clearedLevelText;

        public GameObject pipelineManagerObject;
        public Image progressCircleImage;

        private IEnumerator Start()
        {
            StandardPipelineManager manager = pipelineManagerObject.GetComponent<StandardPipelineManager>();
            ConnectedAreaTrigger.SetAllToFalse();
            manager.Seed = Environment.TickCount;
            manager.Setup();
            yield return StartCoroutine(manager.Generate());
            
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