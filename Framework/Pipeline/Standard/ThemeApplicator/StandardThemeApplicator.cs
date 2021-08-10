using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.Standard.ThemeApplicator.Recipe;
using Framework.Pipeline.ThemeApplicator;
using Framework.Util;
using UnityEngine;

namespace Framework.Pipeline.Standard.ThemeApplicator
{
    public class StandardThemeApplicator : MonoBehaviour, IThemeApplicator
    {
        private GameObject root;
        public List<TypeRecipeCombination> recipes;
        private Dictionary<string, GameWorldObjectRecipe> cookbook;

        private string warningText;
        private bool hasWarning = false;

        public Vector3 layerDistance;
        public Vector3 positionOfWorld;
        public StandardPipelineManager manager;
        public bool flipYAndZ;
        public bool HasWarning => hasWarning;
        private int alreadyCooked;

        private void Start()
        {
            manager = GetComponent<StandardPipelineManager>();
        }

        public IEnumerator ApplyTheme(GameWorld world)
        {
            MakeCookBook();

            root = new GameObject("WorldRoot");
            root.transform.parent = transform;

            GameObjectCreation.YtoZ = flipYAndZ;
            alreadyCooked = 0;

            //cook each GameObject with its given recipe
            Queue<Tuple<IGameWorldObject, Transform>> cookingQueue = new Queue<Tuple<IGameWorldObject, Transform>>();
            int childrenWithNoRecipeCount = 0;

            //start queue
            cookingQueue.Enqueue(new Tuple<IGameWorldObject, Transform>(world.Root, root.transform));

            while (cookingQueue.Any())
            {
                yield return null;
                (IGameWorldObject next, Transform parentTransform) = cookingQueue.Dequeue();

                if (next.Identifier != null)
                {
                    if (!cookbook.ContainsKey(next.Identifier) || cookbook[next.Identifier] == null)
                    {
                        childrenWithNoRecipeCount++;
                        Debug.LogWarning($"The cook book does not contain a recipe for {next.Identifier}.");
                    }
                    else
                    {
                        //cook object
                        GameObject cooked = CookGameWorldObject(next);
                        cooked.transform.parent = parentTransform;
                        parentTransform = cooked.transform;
                        alreadyCooked++;
                    }
                }

                foreach (IGameWorldObject child in next.GetChildren())
                {
                    cookingQueue.Enqueue(new Tuple<IGameWorldObject, Transform>(child, parentTransform));
                }
            }

            yield return null;
            
            //set warning
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
        }
        
         public void ApplyThemeBlocking(GameWorld world)
        {
            MakeCookBook();

            root = new GameObject("WorldRoot");
            root.transform.parent = transform;

            GameObjectCreation.YtoZ = flipYAndZ;
            alreadyCooked = 0;

            //cook each GameObject with its given recipe
            Queue<Tuple<IGameWorldObject, Transform>> cookingQueue = new Queue<Tuple<IGameWorldObject, Transform>>();
            int childrenWithNoRecipeCount = 0;

            //start queue
            cookingQueue.Enqueue(new Tuple<IGameWorldObject, Transform>(world.Root, root.transform));

            while (cookingQueue.Any())
            {
                (IGameWorldObject next, Transform parentTransform) = cookingQueue.Dequeue();

                if (next.Identifier != null)
                {
                    if (!cookbook.ContainsKey(next.Identifier) || cookbook[next.Identifier] == null)
                    {
                        childrenWithNoRecipeCount++;
                        Debug.LogWarning($"The cook book does not contain a recipe for {next.Identifier}.");
                    }
                    else
                    {
                        //cook object
                        GameObject cooked = CookGameWorldObject(next);
                        cooked.transform.parent = parentTransform;
                        parentTransform = cooked.transform;
                        alreadyCooked++;
                    }
                }

                foreach (IGameWorldObject child in next.GetChildren())
                {
                    cookingQueue.Enqueue(new Tuple<IGameWorldObject, Transform>(child, parentTransform));
                }
            }
            
            //set warning
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
        }

        public void StartFindAllTypes()
        {
            FindAllTypes();
        }

        private void FindAllTypes()
        {
            manager = GetComponent<StandardPipelineManager>();

            if (manager != null)
            {
                //setup first time
                if (recipes == null)
                {
                    recipes = new List<TypeRecipeCombination>();
                }

                if (manager.standardPipelineRunner == null)
                {
                    manager.Setup();
                }
                
                GameWorld exampleGameWorld = manager.standardPipelineRunner.ExecuteBlocking();

                //stop if root is null
                if (exampleGameWorld.Root == null)
                {
                    Debug.LogError("The generated Game World is null. Check on the Pipeline Steps");
                }

                //BFS through tree to find all types
                Stack<IGameWorldObject> childrenToTest = new Stack<IGameWorldObject>();
                childrenToTest.Push(exampleGameWorld.Root);

                while (childrenToTest.Any())
                {
                    IGameWorldObject top = childrenToTest.Pop();
                    TypeRecipeCombination combination = new TypeRecipeCombination(top.Identifier);

                    if (!recipes.Contains(combination))
                    {
                        recipes.Add(combination);
                    }


                    foreach (IGameWorldObject child in top.GetChildren())
                    {
                        childrenToTest.Push(child);
                    }
                    
                }
            }
            else
            {
                Debug.LogError("This Object also needs a PipelineManager");
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

        private GameObject CookGameWorldObject(IGameWorldObject child)
        {
            cookbook[child.Identifier].random = manager.standardPipelineRunner.Random;
            GameObject cooked = cookbook[child.Identifier].Cook(child);
            cooked.transform.parent = root.transform;
            cooked.transform.position += alreadyCooked * layerDistance;
            alreadyCooked++;
            return cooked;
        }

        public string GetWarning()
        {
            return warningText;
        }
    }
}