using Demo.Pipeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Get().foundAreas = 0;
        GameManager.Get().uniqueAreasAmount = 0;
        SceneManager.LoadScene("StartScene");
    }
}