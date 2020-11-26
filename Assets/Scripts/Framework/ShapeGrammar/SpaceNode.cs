using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    public class SpaceNode
    {
        private Vector3 hook;
        private Vector3 entryHook;
        private GameObject prefab;

        public readonly List<SpaceNode> branches;
        private readonly List<Vector3> openHooks;

        public SpaceNode(Vector3 hook, GameObject prefab, ShapeGrammarRuler ruler)
        {
            this.hook = hook;
            this.prefab = prefab;

            branches = new List<SpaceNode>();
            openHooks = ruler.connection.hooks.ToList();
            entryHook = ruler.connection.entryHook;
        }

        internal Vector3 GetOpenHook()
        {
            int index = Random.Range(0, openHooks.Count);
            Vector3 chosenHook = openHooks[index];
            openHooks.Remove(chosenHook);
            return chosenHook;
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
            Vector3 a = GetEntryHook();
            Vector3 b = -GetHook();

            // Debug.DrawRay(Vector3.zero, a, Color.red, 1000);
            // Debug.DrawRay(Vector3.zero, b, Color.blue, 1000);
            // Debug.Log($"{b}");

            Vector3 cross = Vector3.Cross(a, b);

            float sign = Mathf.Sign(cross.y);

            float dot = Vector3.Dot(a, b);
            float newrotation = sign * Mathf.Acos(dot);
            Quaternion localRotation = Quaternion.Euler(0, newrotation * 180 / Mathf.PI, 0);

            // Vector3 rotatedA = localRotation * v.GetEntryHook();
            // Debug.DrawRay(Vector3.zero, rotatedA, Color.green, 1000);
            return localRotation;
        }
        
    }
}