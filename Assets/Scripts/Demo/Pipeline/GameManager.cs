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
                return instance = new GameManager();
            }

            return instance;
        }
        
        public int uniqueAreasAmount;
        public int foundAreas;
    }
}