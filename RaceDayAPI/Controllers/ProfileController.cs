using System.Security.Claims; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using RaceDayAPI.Data;
namespace RaceDayAPI.Controllers;
[ApiController, Route("api/profile"), Authorize]
public class ProfileController : ControllerBase
{
    private readonly RaceDayDbContext _db; public ProfileController(RaceDayDbContext db) => _db = db;
    private int CurrentId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    [HttpGet] public async Task<IActionResult> Get() { var u = await _db.Users.AsNoTracking().Where(x => x.UserID == CurrentId).Select(x => new { x.UserID, x.FirstName, x.LastName, x.Email, x.Phone, x.Role }).FirstOrDefaultAsync(); return u == null ? NotFound() : Ok(u); }
    public record ProfileUpdate(string FirstName, string LastName, string? Phone);
    [HttpPut] public async Task<IActionResult> Update(ProfileUpdate r) { if (string.IsNullOrWhiteSpace(r.FirstName) || string.IsNullOrWhiteSpace(r.LastName)) return BadRequest("First and last name are required."); var u = await _db.Users.FindAsync(CurrentId); if (u == null) return NotFound(); u.FirstName = r.FirstName.Trim(); u.LastName = r.LastName.Trim(); u.Phone = r.Phone?.Trim(); await _db.SaveChangesAsync(); return Ok(new { u.UserID, u.FirstName, u.LastName, u.Email, u.Phone, u.Role }); }
}
