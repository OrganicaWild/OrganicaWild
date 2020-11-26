using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    public class SpaceNode
    {
        private Vector3 hook;
        private Vector3 entryHook;
        private GameObject prefab;
        private SpaceNode parent;
        public Vertex GrammarVertex { get; }

        public readonly List<SpaceNode> branches;
        private readonly List<Vector3> openHooks;

        public SpaceNode(Vector3 hook, GameObject prefab, ShapeGrammarRuleComponent ruleComponent, Vertex vertex,SpaceNode parent)
        {
            this.hook = hook;
            this.prefab = prefab;
            this.parent = parent;
            GrammarVertex = vertex;

            branches = new List<SpaceNode>();
            openHooks = ruleComponent.connection.hooks.ToList();
            entryHook = ruleComponent.connection.entryHook;
        }

        internal SpaceNode GetParent()
        {
            return parent;
        }
        
        internal Vector3 GetOpenHook()
        {
            int index = Random.Range(0, openHooks.Count);
            Vector3 chosenHook = openHooks[index];
            openHooks.Remove(chosenHook);
            return chosenHook;
        }
        
        internal void ResetHook(Vector3 newHook, SpaceNode newParent)
        {
            parent.branches.Remove(this); //remove this node as child from parent
            parent.openHooks.Add(hook); //return occupied hook to parent
            
            hook = newHook;
            parent = newParent;
        }

        internal void AddBranch(SpaceNode leaf)
        {
            branches.Add(leaf);
        }

        internal int GetNumberOfOpenHooks()
        {
            return openHooks.Count;
        }

        internal Vector3 GetHook()
        {
            return hook;
        }

        internal GameObject GetPrefab()
        {
            return prefab;
        }

        internal Vector3 GetEntryHook()
        {
            return entryHook;
        }

        internal void RotateHooks(Quaternion rotateBy)
        {
            //hook = rotateBy * hook;
            entryHook = rotateBy * entryHook;
        }
        
        internal Quaternion GetLocalRotation()
        {
            Vector3 a = GetEntryHook().normalized;
            Vector3 b = -GetHook().normalized;

            // Debug.DrawRay(Vector3.zero, a, Color.red, 1000);
            // Debug.DrawRay(Vector3.zero, b, Color.blue, 1000);
            // Debug.Log($"{b}");

            Vector3 cross = Vector3.Cross(a, b);

            float sign = Mathf.Sign(cross.y);

            float dot = Vector3.Dot(a, b);
            float newrotation = sign * Mathf.Acos(dot);
            Quaternion localRotation = Quaternion.Euler(0, newrotation * Mathf.Rad2Deg, 0);

            // Vector3 rotatedA = localRotation * v.GetEntryHook();
            // Debug.DrawRay(Vector3.zero, rotatedA, Color.green, 1000);
            return localRotation;
        }
        
    }
}