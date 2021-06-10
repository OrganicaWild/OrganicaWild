using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    
    internal class SpaceNode
    {
        private readonly SpaceNodeConnection attachedToHook;
        private readonly SpaceNodeConnection attacherHook;
        private readonly GameObject prefab;
        private readonly MissionVertex missionVertex;
        internal readonly SpaceNode parentSpaceNode;
        public GameObject InstantiatedReference { get; set; }

        private readonly List<SpaceNodeConnection> openHooks;

        internal SpaceNode(SpaceNodeConnection attachedToHook, GameObject prefab, ShapeGrammarRuleComponent ruleComponent,
            MissionVertex missionVertex, SpaceNode parentSpaceNode)
        {
            this.attachedToHook = attachedToHook;
            this.prefab = prefab;
            this.missionVertex = missionVertex;
            this.parentSpaceNode = parentSpaceNode;

            List<SpaceNodeConnection> corners = ruleComponent.connection.corners.ToList();
            SpaceNodeConnection entryCorner = ruleComponent.connection.entryCorner;

            openHooks = corners;
            attacherHook = entryCorner;
        }

        internal SpaceNode(GameObject instantiatedReference)
        {
            InstantiatedReference = instantiatedReference;
        }

        internal SpaceNode GetParent()
        {
            return parentSpaceNode;
        }

        internal IEnumerable<SpaceNodeConnection> GetOpenHooks()
        {
            return openHooks.ToList();
        }

        internal SpaceNodeConnection GetOpenHook()
        {
            int index = Random.Range(0, openHooks.Count);
            SpaceNodeConnection chosenHook = openHooks[index];
            //openHooks.Remove(chosenHook);
            return chosenHook;
        }

        internal void RemoveOpenHook(SpaceNodeConnection hook)
        {
            openHooks.Remove(hook);
        }
        
        internal int GetNumberOfOpenHooks()
        {
            return openHooks.Count;
        }

        private SpaceNodeConnection GetHook()
        {
            return attachedToHook;
        }

        internal GameObject GetPrefab()
        {
            return prefab;
        }

        private SpaceNodeConnection GetEntryHook()
        {
            return attacherHook;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRotatedEntryHook()
        {
            return GetLocalRotation() * attacherHook.connectionPoint;
        }

        internal Vector3 GetLocalPosition()
        {
            return GetHook().connectionPoint - GetRotatedEntryHook();
        }

        /// <summary>
        /// Returns the localRotation, which needs to be applied to the SpaceNode when being placed in the GameWorld
        /// The Rotation is calculated by using the orientation of the hook that the new SpaceNode is getting attached to
        /// and also the orientation of the entry hook of the new SpaceNode
        /// </summary>
        /// <returns>LocalRotation for this SpaceNode as a Quaternion</returns>
        internal Quaternion GetLocalRotation()
        {
            Vector3 a = GetEntryHook().connectionDirection.normalized;
            Vector3 b = -GetHook().connectionDirection.normalized;

            // Debug.DrawRay(Vector3.zero, a, Color.red, 1000);
            // Debug.DrawRay(Vector3.zero, b, Color.blue, 1000);
            // Debug.Log($"{b}");

            Vector3 cross = Vector3.Cross(a, b);

            float sign = Mathf.Sign(cross.y);

            float dot = Vector3.Dot(a, b);
            float newRotation = sign * Mathf.Acos(dot);
            Quaternion localRotation = Quaternion.Euler(0, newRotation * Mathf.Rad2Deg, 0);

            // Vector3 rotatedA = localRotation * v.GetEntryHook();
            // Debug.DrawRay(Vector3.zero, rotatedA, Color.green, 1000);
            return localRotation;
        }
    }
}