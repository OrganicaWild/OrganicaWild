namespace Framework.Util
{
    /// <summary>
    /// Union Find Data Structure after the Implementation of Robert Sedgewik et al. from Algorithms Fourth Edition.
    /// </summary>
    public class UnionFind
    {
        private int[] id;
        private int[] sz;
        private int count;
        
        public UnionFind(int n)
        {
            count = n;
            id = new int[n];
            for (int i = 0; i < n; i++)
            {
                id[i] = i;
            }

            sz = new int[n];
            for (int i = 0; i < n; i++)
            {
                sz[i] = 1;
            }
        }

        public void Union(int p, int q)
        {
            int i = Find(p);
            int j = Find(q);

            if (i == j) return;

            if (sz[i] < sz[j])
            {
                id[i] = j;
                sz[j] += sz[i];
            } else
            {
                id[j] = i;
                sz[i] += sz[j];
            }

            count--;
        }

        public int Find(int p)
        {
            while (p != id[p])
            {
                p = id[p];
            }

            return p;
        }

        public bool Connected(int p, int q)
        {
            return Find(p) == Find(q);
        }

        public int Count()
        {
            return count;
        }
    }
}