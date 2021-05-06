using System;
using System.Collections.Generic;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.ThemeApplicator.Recipe;
using UnityEngine;

namespace Framework.Pipeline.ThemeApplicator
{
    public class ThemeApplicator : MonoBehaviour, IThemeApplicator
    {
        private GameObject root;

        public List<TypeRecipeCombination> recipes;

        private Dictionary<string, GameWorldObjectRecipe> cookbook;

        private string warningText;
        private bool hasWarning = false;
        public float layerDistance;
        public Vector3 positionOfWorld;
        private PipelineManager manager;
        public bool HasWarning => hasWarning;

        public GameObject Apply(GameWorld world)
        {
            this.manager = GetComponent<PipelineManager>();
            MakeCookBook();
            root = new GameObject();

            if (cookbook.ContainsKey(world.Root.Type))
            {
                GameObject cooked = CookGameWorldObject(0,world.Root);
                cooked.transform.parent = root.transform;
            }

            int childrenWithNoRecipeCount = InterpretLayer(world.Root, 1);

            if (childrenWithNoRecipeCount > 0)
            {
                hasWarning = true;
                warningText =
                    $"Number: {childrenWithNoRecipeCount} IGameWorldObjects with specified Type cannot be cooked since they have no recipe.";
            }
            else
            {
                hasWarning = false;
            }

            return root;
        }

        public void FindAllTypes()
        {
            manager = GetComponent<PipelineManager>();

            if (manager != null)
            {
                //setup first time
                if (recipes == null)
                {
                    recipes = new List<TypeRecipeCombination>();
                }

                GameWorld exampleGameWorld = manager.pipeLineRunner.Execute();

                if (exampleGameWorld.Root.Type != null)
                {
                    TypeRecipeCombination combination = new TypeRecipeCombination(exampleGameWorld.Root.Type);

                    if (!recipes.Contains(combination))
                    {
                        recipes.Add(combination);
                    }
                }

                FindTypeInLayer(exampleGameWorld.Root);
            }
            else
            {
                throw new Exception("This Object also needs a PipelineManager");
            }
        }

        private void FindTypeInLayer(IGameWorldObject parent)
        {
            foreach (IGameWorldObject child in parent.GetChildren())
            {
                if (child.Type != null)
                {
                    TypeRecipeCombination combination = new TypeRecipeCombination(child.Type);

                    if (!recipes.Contains(combination))
                    {
                        recipes.Add(combination);
                    }
                }

                FindTypeInLayer(child);
            }
        }

        private void MakeCookBook()
        {
            cookbook = new Dictionary<string, GameWorldObjectRecipe>();

            foreach (TypeRecipeCombination typeRecipeCombination in recipes)
            {
                cookbook.Add(typeRecipeCombination.name, typeRecipeCombination.recipe);
            }
        }

        private int InterpretLayer(IGameWorldObject parent, float depth)
        {
            int childrenWithNoRecipeCount = 0;
            foreach (IGameWorldObject child in parent.GetChildren())
            {
                childrenWithNoRecipeCount += InterpretLayer(child, depth + 1);

                //children that do not have a type are expected to not be drawn and do not show a warning
                if (child.Type == null)
                {
                    continue;
                }

                if (!cookbook.ContainsKey(child.Type))
                {
                    Debug.LogError($"The cook book does not contain a recipe for {child.Type}.");
                }

                if (cookbook[child.Type] == null)
                {
                    childrenWithNoRecipeCount++;
                    continue;
                }

                GameObject cooked = CookGameWorldObject(depth, child);
            }

            return childrenWithNoRecipeCount;
        }

        private GameObject CookGameWorldObject(float depth, IGameWorldObject child)
        {
            cookbook[child.Type].random = manager.pipeLineRunner.Random;
            GameObject cooked = cookbook[child.Type].Cook(child);
            cooked.transform.parent = root.transform;
            cooked.transform.position += new Vector3(0, 0, depth * layerDistance);
            return cooked;
        }

        public string GetWarning()
        {
            return warningText;
        }
    }
}