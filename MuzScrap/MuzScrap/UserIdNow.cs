namespace MuzScrap
{
    public partial class UserIdNow
    {
        private int _userID;
        public int UserID
        {
            get => _userID;
            set { _userID = value; }
        }
        public UserIdNow(int userID)
        {
            UserID = userID;
        }
    }
}
