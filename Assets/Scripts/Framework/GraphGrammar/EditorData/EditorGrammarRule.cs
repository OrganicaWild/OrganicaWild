using System;

namespace Framework.GraphGrammar.EditorData
{
    [Serializable]
    public class EditorGrammarRule
    {
        public EditorMissionGraph left;
        public EditorMissionGraph right;

        public GrammarRule GetGrammarRule()
        {
            MissionGraph l = left.DeserializeAndConvert();
            MissionGraph r = right.DeserializeAndConvert();

            return new GrammarRule(l, r);
        }
    }
}