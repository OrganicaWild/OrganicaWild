using System.Collections.Generic;
using Demo.ShapeGrammar;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    [CreateAssetMenu]
    public class ScriptableConnections : ScriptableObject
    {
        public MeshCorner entryCorner;
        public List<MeshCorner> corners;
    }
}
