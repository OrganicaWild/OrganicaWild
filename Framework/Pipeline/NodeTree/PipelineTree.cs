using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Pipeline.NodeTree
{
    public class PipelineTree
    {
        public PipelineNode Root { get; }

        public PipelineTree(PipelineNode root)
        {
            Root = root;
        }

        public List<PipelineNode> Traverse()
        {
            var list = new List<PipelineNode>();
            var q = new Queue<PipelineNode>();
            q.Enqueue(Root);

            while (q.Count > 0)
            {
                var e = q.Dequeue();
                list.Add(e);

                foreach (var next in e.Next)
                {
                    if (next == null) continue;
                    q.Enqueue(next.To);
                }
            }

            return list;
        }

        public bool IsCyclic()
        {
            
            var s = new Stack<Tuple<int, PipelineNode>>();
            s.Push(new Tuple<int, PipelineNode>(0, Root));

            while (s.Count > 0)
            {
                var (act, node) = s.Pop();

                if (act == 1) //exit
                {
                    node.color = 2;
                }
                else
                {                            
                    node.color = 1; //grey
                    s.Push(new Tuple<int, PipelineNode>(1, node));

                    foreach (var next in node.Next)
                    {
                        if (next == null) continue;

                        if (next.To.color == 1) //if color is grey, it has been visited
                        {
                            return true;
                        }

                        if (next.To.color == 0)
                        {
                            s.Push(new Tuple<int, PipelineNode>(0, next.To));
                        }
                    }
                }
            }

            return false;
        }
    }
}