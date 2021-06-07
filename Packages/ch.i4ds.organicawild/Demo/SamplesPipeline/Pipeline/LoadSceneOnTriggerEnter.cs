using Demo.Pipeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.foundAreas = 0;
        GameManager.uniqueAreasAmount = 0;
        SceneManager.LoadScene("StartScene");
    }
}