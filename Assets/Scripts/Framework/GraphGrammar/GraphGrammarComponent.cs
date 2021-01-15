using System.Collections.Generic;
using System.Linq;
using Framework.GraphGrammar.EditorData;
using Framework.Util;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework.GraphGrammar
{
    /// <summary>
    /// Unity Component for using a graph grammar.
    /// Attach this to a GameObject and supply the rules and the mother as ScriptableObjects.
    /// </summary>
    public class GraphGrammarComponent : MonoBehaviour
    {
        /// <summary>
        /// Supplied rules as ScriptableObjects.
        /// Create the graphs for the rules via the Organica Wild > Create Graph Grammar Rule menu entry.
        /// </summary>
        public List<EditorGrammarRule> rules = new List<EditorGrammarRule>();
        
        /// <summary>
        /// Initial graph
        /// Create the graph via the via the Organica Wild > Create Graph Grammar Rule menu entry
        /// </summary>
        public EditorMissionGraph mother;
        
        private List<GrammarRule> internalRules;
        private MissionGraph internalMother;
        private GraphGrammar grammar;

        private IEnumerable<DrawableMissionVertex.Branch> positions =
            new List<DrawableMissionVertex.Branch>();

        private readonly Dictionary<MissionVertex, Vector3> dictionary = new Dictionary<MissionVertex, Vector3>(
            new IdentityEqualityComparer<MissionVertex>());

        /// <summary>
        /// Initializes grammar for use. Must be run, before it is possible to apply rules.
        /// </summary>
        public void Initialize()
        {
            internalRules = new List<GrammarRule>();
            foreach (GrammarRule rule in rules.Select(editorGrammarRule => editorGrammarRule.GetGrammarRule()))
            {
                internalRules.Add(rule);
            }

            internalMother = mother.DeserializeAndConvert();

            grammar = new GraphGrammar(internalRules, internalMother);

            DrawableMissionVertex motherAsDrawable = internalMother.Start as DrawableMissionVertex;
            //show in unity
            positions =
                motherAsDrawable.Paint(new Vector3(0, 0, 0),
                    new List<DrawableMissionVertex.Branch>(),
                    dictionary);
        }
        
        public void ApplyOneRule()
        {
            grammar.ApplyOneRule();
            positions =
                (internalMother.Start as DrawableMissionVertex).Paint(new Vector3(0, 0, 0),
                    new List<DrawableMissionVertex.Branch>(),
                    dictionary);
        }

        public void ApplyUntilNoRulesFitAnymore()
        {
            grammar.ApplyUntilNoRulesFitAnymore();
            positions =
                (internalMother.Start as DrawableMissionVertex).Paint(new Vector3(0, 0, 0),
                    new List<DrawableMissionVertex.Branch>(),
                    dictionary);
        }
        
        public MissionGraph GetLevel()
        {
            return grammar.GetLevel();
        }

        private void OnDrawGizmos()
        {
            Dictionary<string, Color> colors = new Dictionary<string, Color>();

            Dictionary<MissionVertex, Vector3> dict =
                new Dictionary<MissionVertex, Vector3>(
                    new IdentityEqualityComparer<MissionVertex>());
            foreach (DrawableMissionVertex.Branch position in positions)
            {
                Color color;
                if (colors.ContainsKey(position.t.Type))
                {
                    color = colors[position.t.Type];
                }
                else
                {
                    color = new Color(Random.value, Random.value, Random.value);
                    colors.Add(position.t.Type, color);
                }

                if (dict.ContainsKey(position.t))
                {
                    Gizmos.DrawLine(position.parent, dict[position.t]);
                }
                else
                {
                    Gizmos.color = color;
                    Gizmos.DrawCube(position.tPosition, new Vector3(0.1f, 0.1f, 0.1f));

                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(position.parent, position.tPosition);

                    GUIStyle guiStyle = new GUIStyle {fontSize = 30, fontStyle = FontStyle.Bold};

                    Handles.Label(position.tPosition, $"{position.t.Type}", guiStyle);
                    dict.Add(position.t, position.tPosition);
                }
            }
        }
    }
}