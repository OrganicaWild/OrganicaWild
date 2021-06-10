using UnityEngine;

namespace Demo.Pipeline
{
    public class ActivateAllChildrenUponGameManagerStart : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (GameManager._GameHasStarted)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
