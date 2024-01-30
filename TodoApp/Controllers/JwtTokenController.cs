using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtTokenController : ControllerBase
    {
        private IConfiguration _config;
        private readonly login _login;
        public JwtTokenController(IConfiguration config , ApplicationDbContext db)
        {
            _config = config;
            _login = new login(db);
        }

        [HttpPost]
        public IActionResult Post(string uname , string pass)
        {
            Users user = _login.LoginPage(uname, pass);
            //If login usrename and password are correct then proceed to generate token
            if(user == null)
            {
                return BadRequest("Bad Credentials");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("uid", user.UserId.ToString())
            };

            var Sectoken = new JwtSecurityToken(
              _config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return Ok(token);
        }
    }
}
