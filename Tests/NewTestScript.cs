using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Pipeline;
using Framework.Pipeline.GameWorldObjects;
using Framework.Pipeline.NodeTree;
using Framework.Pipeline.Standard;
using Framework.Pipeline.Standard.PipeLineSteps.SmallSteps;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Packages.OrganicaWild.Tests
{
    public class NewTestScript
    {
        private class TestStep : IPipelineStep
        {
            public Random Rmg { get; set; }
            public Type[] RequiredGuarantees { get; }

            public List<GameWorldTypeSpecifier> NeededInputGameWorldObjects => new List<GameWorldTypeSpecifier>()
            {
                new(typeof(IGameWorldObject), AmountSpecifier.Many)
            };

            public List<GameWorldTypeSpecifier> ProvidedOutputGameWorldObjects => new List<GameWorldTypeSpecifier>()
            {
                new(typeof(IGameWorldObject), AmountSpecifier.Many)
            };

            public GameWorld Apply(GameWorld world)
            {
                return world;
            }

            public IPipelineStep[] ConnectedNextSteps { get; set; }
            public IPipelineStep[] ConnectedPreviousSteps { get; set; }
        }
        
        [Test]
        public void TestCycle0()
        {
            var node0 = new PipelineNode(new TestStep());
            var node1 = new PipelineNode(new TestStep());
            var node2 = new PipelineNode(new TestStep());

            var con0 = new NodeConnection(node0, 0, node1, 0);
            var con1 = new NodeConnection(node1, 0, node2, 0);

            var tree = new PipelineTree(node0);
            Assert.False(tree.IsCyclic());
        }

        [Test]
        public void TestCycle1()
        {
            var node0 = new PipelineNode(new TestStep());
            var node1 = new PipelineNode(new TestStep());
            var node2 = new PipelineNode(new TestStep());

            var con0 = new NodeConnection(node0, 0, node1, 0);
            var con1 = new NodeConnection(node1, 0, node2, 0);
            var con2 = new NodeConnection(node2, 0, node0, 0);

            var tree = new PipelineTree(node0);
            Assert.True(tree.IsCyclic());
        }
    }
}