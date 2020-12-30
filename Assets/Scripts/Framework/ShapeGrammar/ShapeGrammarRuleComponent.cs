﻿using Demo.ShapeGrammar;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    /// <summary>
    /// Provides to ability to make any GameObject into a ShapeGrammarRule.
    /// The GameObject should also have a kinematic Rigidbody and a Collider for overlap detection.
    /// </summary>
    public class ShapeGrammarRuleComponent : MonoBehaviour
    {
        public string[] type;
        public ScriptableConnections connection;

        public virtual void Modify()
        {
        }

        private void OnDrawGizmos()
        {
            if (connection.entryCorner != null)
            {
                Vector3 pos = connection.entryCorner.connectionPoint + transform.position;
                Vector3 negativePosition = pos - connection.entryCorner.connectionDirection;
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(pos, new Vector3(0.1f, 0.1f, 0.1f));
                Gizmos.DrawLine(negativePosition, pos);
            }

            for (int index = 0; index < connection.corners.Count; index++)
            {
                SpaceNodeConnection conn = connection.corners[index];

                Vector3 pos = conn.connectionPoint + transform.position;
                Vector3 negativePosition = pos - conn.connectionDirection;
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(pos, new Vector3(0.1f, 0.1f, 0.1f));
                Gizmos.DrawLine(negativePosition, pos);
            }
        }
    }
}