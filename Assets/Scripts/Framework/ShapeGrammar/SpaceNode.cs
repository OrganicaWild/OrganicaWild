using System.Collections.Generic;
using System.Linq;
using Demo.ShapeGrammar;
using Framework.GraphGrammar;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    public class SpaceNode
    {
        private readonly MeshCorner attachedToHook;
        private readonly MeshCorner attacherHook;
        private readonly GameObject toInstantiate;
        public readonly SpaceNode parentSpaceNode;
        public MissionVertex GrammarMissionVertex { get; }
        public GameObject InstantiatedReference { get; set; }

        private readonly List<MeshCorner> openHooks;

        public SpaceNode(MeshCorner attachedToHook, GameObject toInstantiate, ShapeGrammarRuleComponent ruleComponent,
            MissionVertex missionVertex, SpaceNode parentSpaceNode)
        {
            this.attachedToHook = attachedToHook;
            this.toInstantiate = toInstantiate;
            this.parentSpaceNode = parentSpaceNode;
            GrammarMissionVertex = missionVertex;
            
            List<MeshCorner> corners = ruleComponent.connection.corners.ToList();
            MeshCorner entryCorner = ruleComponent.connection.entryCorner;

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

        internal List<MeshCorner> GetOpenHooks()
        {
            return openHooks.ToList();
        }

        internal MeshCorner GetOpenHook()
        {
            int index = Random.Range(0, openHooks.Count);
            MeshCorner chosenHook = openHooks[index];
            //openHooks.Remove(chosenHook);
            return chosenHook;
        }

        internal void RemoveOpenHook(MeshCorner hook)
        {
            openHooks.Remove(hook);
        }
        
        internal int GetNumberOfOpenHooks()
        {
            return openHooks.Count;
        }

        internal MeshCorner GetHook()
        {
            return attachedToHook;
        }

        internal GameObject GetPrefab()
        {
            return toInstantiate;
        }

        internal MeshCorner GetEntryHook()
        {
            return attacherHook;
        }

        public Vector3 GetRotatedEntryHook()
        {
            return GetLocalRotation() * attacherHook.connectionPoint;
            // for (int index = 0; index < openHooks.Count; index++)
            // {
            //     openHooks[index] = rotateBy * openHooks[index];
            // }
        }

        internal Vector3 GetLocalPosition()
        {
            return GetHook().connectionPoint - GetRotatedEntryHook();
        }

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
            float newrotation = sign * Mathf.Acos(dot);
            Quaternion localRotation = Quaternion.Euler(0, newrotation * Mathf.Rad2Deg, 0);

            // Vector3 rotatedA = localRotation * v.GetEntryHook();
            // Debug.DrawRay(Vector3.zero, rotatedA, Color.green, 1000);
            return localRotation;
        }
    }
}