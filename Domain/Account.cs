using System;

namespace Domain
{
    public class Account
    {
        public decimal Balance { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsFrozen { get; set; }
        public string UserName { get; set; }

        public Account(string username, int userId)
        {
            UserName = username;
            UserId = userId;
        }
        public Account()
        {
        }
    }
}
