using Framework.ShapeGrammar;
using UnityEngine;

public class ShapeGrammarController : MonoBehaviour
{
    private ShapeGrammar grammar;
    // Start is called before the first frame update
    void Start()
    {
        grammar = GetComponent<ShapeGrammar>();
    }

    private bool cleared = true;

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cleared)
            {
                grammar.GenerateGeometry();
                cleared = false;
            }
            else
            {
                grammar.ClearOldLevel();
                cleared = true;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            grammar.ClearOldLevel();
            grammar.GenerateGeometry();
        }
    }
}
