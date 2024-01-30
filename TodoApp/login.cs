using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;

namespace TodoApp
{
    public class login
    {
        private readonly ApplicationDbContext _db;

        public login(ApplicationDbContext db)
        {
            _db = db;
        }

        public Users LoginPage(string uname, string pass)
        {
            Users user = _db.Users.FirstOrDefault(u => u.UserName == uname);

            if (user == null)
            {
                return null;
            }

            if (user.Password != pass)
            {
                return null;
            }

            return user;
        }
    }
}
