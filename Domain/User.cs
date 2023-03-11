namespace Domain
{
    public class User
    {
        public string Name { get; set; }
        public string Pin { get; set; }
        public int Id { get; set; }

        public User(string username, string pin)
        {
            Name = username;
            Pin = pin;
        }

        public User()
        {

        }
    }
}
