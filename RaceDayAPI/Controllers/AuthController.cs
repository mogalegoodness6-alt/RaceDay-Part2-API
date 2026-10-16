using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
namespace RaceDayAPI.Controllers;
[ApiController, Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db; private readonly IConfiguration _config; private readonly PasswordHasher<User> _hasher = new();
    public AuthController(RaceDayDbContext db, IConfiguration config) { _db = db; _config = config; }
    public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string? Phone);
    public record LoginRequest(string Email, string Password);
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.FirstName) || string.IsNullOrWhiteSpace(r.LastName) || string.IsNullOrWhiteSpace(r.Email) || string.IsNullOrWhiteSpace(r.Password)) return BadRequest("First name, last name, email and password are required.");
        if (r.Password.Length < 8) return BadRequest("Password must contain at least 8 characters.");
        var email = r.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(x => x.Email.ToLower() == email)) return Conflict("An account with this email already exists.");
        var u = new User { FirstName = r.FirstName.Trim(), LastName = r.LastName.Trim(), Email = email, Phone = r.Phone?.Trim(), Role = "Participant" };
        u.PasswordHash = _hasher.HashPassword(u, r.Password); _db.Users.Add(u); await _db.SaveChangesAsync();
        return Created("/api/Auth/login", PublicUser(u));
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Email) || string.IsNullOrWhiteSpace(r.Password)) return BadRequest("Email and password are required.");
        var email = r.Email.Trim().ToLowerInvariant(); var u = await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email);
        if (u == null || string.IsNullOrWhiteSpace(u.PasswordHash) || !u.PasswordHash.StartsWith("AQAAAA")) return Unauthorized("Invalid email or password. Seeded placeholder hashes cannot be used to log in; register a test account instead.");
        if (_hasher.VerifyHashedPassword(u, u.PasswordHash, r.Password) == PasswordVerificationResult.Failed) return Unauthorized("Invalid email or password.");
        var key = _config["Jwt:Key"]!; var claims = new[] { new Claim(ClaimTypes.NameIdentifier, u.UserID.ToString()), new Claim(ClaimTypes.Name, u.Email), new Claim(ClaimTypes.Role, u.Role) };
        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256); var expires = DateTime.UtcNow.AddHours(2);
        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: expires, signingCredentials: creds);
        return Ok(new { message = "Login successful.", token = new JwtSecurityTokenHandler().WriteToken(token), expiration = expires, user = PublicUser(u) });
    }
    private static object PublicUser(User u) => new { u.UserID, u.FirstName, u.LastName, u.Email, u.Phone, u.Role };
}
