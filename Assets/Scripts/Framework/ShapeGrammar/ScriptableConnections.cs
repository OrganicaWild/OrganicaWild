using System.Collections.Generic;
using UnityEngine;

namespace Framework.ShapeGrammar
{
    /// <summary>
    /// Provides an interface to add hooks and connections to ShapeGrammarRuleComponents.
    /// </summary>
    [CreateAssetMenu]
    public class ScriptableConnections : ScriptableObject
    {
        public SpaceNodeConnection entryCorner;
        public List<SpaceNodeConnection> corners;
    }
}
