using System;

namespace Polybool.Net.Objects
{
    public class Node : IEquatable<Node>
    {
        public Node Status { get; set; }

        public Node Other { get; set; }

        public Node Ev { get; set; }

        public Node Previous { get; set; }

        public Node Next { get; set; }

        public bool IsRoot { get; set; }

        public Action Remove { get; set; }

        public bool IsStart { get; set; }

        public Point Pt { get; set; }

        public Segment Seg { get; set; }

        public bool Primary { get; set; }

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Previous, other.Previous) && Equals(Next, other.Next) && IsRoot == other.IsRoot;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Previous != null ? Previous.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Next != null ? Next.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsRoot.GetHashCode();
                return hashCode;
            }
        }
    }
}