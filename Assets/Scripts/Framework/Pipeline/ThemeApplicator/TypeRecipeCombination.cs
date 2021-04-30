using System;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.ThemeApplicator.Recipe;
using Framework.Util.Miscellanous;

namespace Framework.Pipeline.ThemeApplicator
{
    [Serializable]
    public class TypeRecipeCombination
    {
        [ReadOnly]
        public string name;
        public GameWorldObjectRecipe recipe;

        public TypeRecipeCombination(string name)
        {
            this.name = name;
        }

        protected bool Equals(TypeRecipeCombination other)
        {
            return name == other.name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((TypeRecipeCombination) obj);
        }

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }
    }
}