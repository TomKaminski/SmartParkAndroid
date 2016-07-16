namespace SmartParkAndroid.Models
{
    public class User
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Charges { get; set; }
        public string PasswordHash { get; set; }
    }
}