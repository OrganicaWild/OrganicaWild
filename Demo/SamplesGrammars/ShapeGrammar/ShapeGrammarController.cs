using UnityEngine;

namespace Demo.ShapeGrammar
{
    public class ShapeGrammarController : MonoBehaviour
    {
        private Framework.ShapeGrammar.ShapeGrammar grammar;
        // Start is called before the first frame update
        private void Start()
        {
            grammar = GetComponent<Framework.ShapeGrammar.ShapeGrammar>();
        }

        private bool cleared = true;

        // Update is called once per frame
        private void Update()
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
}
