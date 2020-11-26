using UnityEngine;

namespace Framework.ShapeGrammar
{
    public class ShapeGrammarRuler : MonoBehaviour
    {
        public string type;
        public ScriptableConnections connection;

        private void OnDrawGizmos()
        {
            if (connection.entryHook != null)
            {
                Vector3 pos = connection.entryHook;
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(transform.position + pos, new Vector3(0.5f, 0.1f, 0.2f));
                Gizmos.DrawLine(Vector3.zero, pos);
            }

            for (int index = 0; index < connection.hooks.Length; index++)
            {
                Vector3 pos = connection.hooks[index];

                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.position + pos, new Vector3(0.5f, 0.1f, 0.2f));
                Gizmos.DrawLine(Vector3.zero, pos);
            }
        }
    }
}