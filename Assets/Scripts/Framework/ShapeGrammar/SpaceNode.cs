using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    public class SpaceNode
    {
        private readonly Vector3 hook;
        private Vector3 entryHook;
        private readonly GameObject prefab;
        public readonly SpaceNode parent;
        public MissionVertex GrammarMissionVertex { get; }
        public GameObject Instantiated { get; set; }

        public readonly List<SpaceNode> branches;
        private readonly List<Vector3> openHooks;

        public SpaceNode(Vector3 hook, GameObject prefab, ShapeGrammarRuleComponent ruleComponent,
            MissionVertex missionVertex, SpaceNode parent)
        {
            this.hook = hook;
            this.prefab = prefab;
            this.parent = parent;
            GrammarMissionVertex = missionVertex;

            branches = new List<SpaceNode>();
            openHooks = ruleComponent.connection.hooks.ToList();
            entryHook = ruleComponent.connection.entryHook;
        }

        internal SpaceNode(GameObject instantiated)
        {
            Instantiated = instantiated;
        }

        internal SpaceNode GetParent()
        {
            return parent;
        }

        internal List<Vector3> GetRotatedOpenHooks()
        {
            Quaternion rotation = GetLocalRotation();
            return openHooks.Select(openHook =>openHook).ToList();
        }

        internal Vector3 GetOpenHook()
        {
            int index = Random.Range(0, openHooks.Count);
            Vector3 chosenHook = openHooks[index];
            //openHooks.Remove(chosenHook);
            return chosenHook;
        }

        internal void RemoveOpenHook(Vector3 hook)
        {
            openHooks.Remove(hook);
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

        public Vector3 GetRotatedEntryHook()
        {
            return GetLocalRotation() * entryHook;
            // for (int index = 0; index < openHooks.Count; index++)
            // {
            //     openHooks[index] = rotateBy * openHooks[index];
            // }
        }

        internal Vector3 GetLocalPosition()
        {
            return GetHook() - GetRotatedEntryHook();
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