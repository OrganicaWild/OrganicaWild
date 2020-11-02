using UnityEngine;

namespace Framework.Evolutionary.Standard
{
    public class StandardGenoPhenotypeCombination2 : IGenoPhenoCombination
    {
        public int[,] map = new int[100,100];
        internal Vector2 StartLocation { get; set; }
        internal Vector2 EndLocation { get; set; }

        public StandardGenoPhenotypeCombination2(Vector2 start, Vector2 end)
        {
            StartLocation = start;
            EndLocation = end;
        }

        public void BuildPhenoType()
        {
            var playerX = (int)(StartLocation[0] * 100);
            var playerY = (int)(StartLocation[1] * 100);
            map[playerY, playerX] = 1;

            playerX = (int)(EndLocation[0] * 100);
            playerY = (int)(EndLocation[1] * 100);
            map[playerY, playerX] = 2;
        }

        public void Mutate()
        {
            
        }
    }
}