using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.ShapeGrammar;
using UnityEngine;

public class ShapeGrammarRuleProvider : MonoBehaviour
{
    private ShapeGrammar grammar;

    public void Start()
    {
        grammar = gameObject.GetComponent(typeof(ShapeGrammar)) as ShapeGrammar;
        IEnumerable<GameObject> emptyRules = grammar?.rules.Where(rule => rule == null);
        if (emptyRules?.Count() == 0) return;

        for (int i = 0; i < (grammar?.rules).Length; i++)
        {
            if (grammar?.rules[i] != null) continue;

            GameObject newRule = GameObject.CreatePrimitive(PrimitiveType.Plane);

            // Make CA
            // TODO: Randomize Parameters
            CARuleNetwork CANetwork = new CARuleNetwork(100, 100, 0.45f);
            CANetwork.Run(1);

            MeshRenderer meshRenderer = newRule.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            meshRenderer.sharedMaterial.mainTexture = CANetwork.Convert();
            Material mapMaterial = new Material(Shader.Find("Unlit/Texture"));
            meshRenderer.materials = new[] {mapMaterial};
            meshRenderer.transform.localScale = new Vector3(CANetwork.Width, 1, CANetwork.Height);

            // TODO: Analyse where connections would go
            // TODO: Analyse whether connections are good enough

            // Make Connection Object
            ShapeGrammarRuleComponent shapeGrammarRuleComponent =
                newRule.AddComponent(typeof(ShapeGrammarRuleComponent)) as ShapeGrammarRuleComponent;
            // TODO: try to detect all the possible types instead
            shapeGrammarRuleComponent.type = new[]
            {
                "TestItem", "TestSecret", "Key", "Lock", "KeyMulti", "ItemBonus", "ItemQuest", "KeyFinal",
                "KeyMultiPiece"
            };

            // TODO: Apply results from analysis
            shapeGrammarRuleComponent.connection = ScriptableObject.CreateInstance<ScriptableConnections>();
            shapeGrammarRuleComponent.connection.entryCorner = new SpaceNodeConnection();
            shapeGrammarRuleComponent.connection.entryCorner.connectionPoint = new Vector3(-CANetwork.Width / 2, 1, 0);
            shapeGrammarRuleComponent.connection.entryCorner.connectionDirection = new Vector3(1, 0, 0);

            List<SpaceNodeConnection> corners = new List<SpaceNodeConnection>();
            SpaceNodeConnection connection = new SpaceNodeConnection();
            connection.connectionPoint = new Vector3(0, 1, CANetwork.Width / 2);
            connection.connectionDirection = new Vector3(0, 0, -1);
            corners.Add(connection);
            shapeGrammarRuleComponent.connection.corners = corners;


            grammar.rules[i] = newRule;
        }
    }
}