using MuzScrap.BaseContext;

namespace MuzScrap
{
    public class MuzScrapBdContext
    {
        private static MuzScrapDbContext? instance;
        public static MuzScrapDbContext GetInstance()
        {
            if (instance == null)
                instance = new MuzScrapDbContext();
            return instance;
        }
    }
}
