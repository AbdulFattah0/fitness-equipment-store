using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CustomerController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        readonly IConfiguration configuration;

        public CustomerController(AppDbContext context, IConfiguration config)
        {
            _ctx = context;
            this.configuration = config;
        }

        [HttpPost("register")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Register(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx!);
            Customer? already = await dao.GetByEmail(helper.Email);
            if (already == null)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, helper.Password!);
                helper.Password = ""; 
                Customer dbUser = new()
                {
                    Firstname = helper.Firstname!,
                    Lastname = helper.Lastname!,
                    Email = helper.Email!,
                    Hash = hashSalt.Hash!,
                    Salt = hashSalt.Salt!
                };
                dbUser = await dao.Register(dbUser);
                helper.Token = dbUser.Id > 0 ? "Customer registered" : "Customer registration failed";
            }
            else
            {
                helper.Token = "Customer registration failed - email already in use";
            }
            return helper;
        }

        [HttpPost("login")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Login(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx!);
            Customer? customer = await dao.GetByEmail(helper.Email);
            if (customer != null)
            {
                if (VerifyPassword(helper.Password, customer.Hash!, customer.Salt!))
                {
                 
                    helper.Firstname = "";
                    helper.Lastname = "";
                    helper.Password = "";

                    var appSettings = configuration.GetSection("AppSettings").GetValue<string>("Secret");
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(appSettings);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, customer.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    helper.Token = tokenHandler.WriteToken(token);
                }
                else
                {
                    helper.Token = "Email or password invalid - login failed";
                }
            }
            else
            {
                helper.Token = "no such Customer - login failed";
            }
            return helper;
        }

        private static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            RandomNumberGenerator.Create().GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            var hash = Convert.ToBase64String(rfc2898.GetBytes(256));
            return new HashSalt { Hash = hash, Salt = salt };
        }

        public static bool VerifyPassword(string? enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898 = new Rfc2898DeriveBytes(enteredPassword!, saltBytes, 10000, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(rfc2898.GetBytes(256)) == storedHash;
        }

        public class HashSalt
        {
            public string? Hash { get; set; }
            public string? Salt { get; set; }
        }
    }
}
