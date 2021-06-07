using System;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using Unity.Collections;

namespace Framework.Pipeline.Standard.ThemeApplicator
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