using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.ShapeGrammar;
using UnityEngine;

public class CameraMover : MonoBehaviour
{

    public GameObject grammarRunner;
    private ShapeGrammar grammar;
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        grammar = grammarRunner.GetComponent<ShapeGrammar>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(nameof(NewLevel));
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(Vector3.zero, -Vector3.up, Time.deltaTime * 90f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * 90f);
        }
        
    }

    IEnumerator NewLevel()
    {
        grammar.ClearOldLevel();
        yield return null;
        grammar.Start();
    }
}
