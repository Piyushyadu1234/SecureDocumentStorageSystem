using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecureDocumentStorageSystem.Data;
using SecureDocumentStorageSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SecureDocumentStorageSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> Register(string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new Exception("User already exists");

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) throw new Exception("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                throw new Exception("Invalid password");

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
