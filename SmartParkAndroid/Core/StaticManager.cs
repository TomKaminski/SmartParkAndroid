namespace SmartParkAndroid.Core
{
    public static class StaticManager
    {
        public static bool LoggedIn { get; set; }
        public static string UserName { get; set; }
        public static string UserHash { get; set; }
        public static int Charges { get; set; }
        public static string ImageId { get; set; }

        public static void InitBase(bool loggedIn, string username, string userhash, int charges, string imageId)
        {
            LoggedIn = loggedIn;
            UserName = username;
            UserHash = userhash;
            Charges = charges;
            ImageId = imageId;
        }
    }
}