namespace Demo.Pipeline
{
    public class GameManager
    {
        private static GameManager instance;
    
        private GameManager()
        {
        
        }

        public static GameManager Get()
        {
            if (instance == null)
            {
                return new GameManager();
            }

            return instance;
        }
    
        public int points = 0;

        public void IncreasePoints(int amount)
        {
            points += amount;
        }
    }
}