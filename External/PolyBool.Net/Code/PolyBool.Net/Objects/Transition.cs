using System;

namespace Polybool.Net.Objects
{
    public class Transition
    {
        public Node After { get; set; }
        public Node Before { get; set; }

        public Func<Node, Node> Insert { get; set; }
    }
}