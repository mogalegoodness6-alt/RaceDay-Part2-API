using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using RaceDayAPI.Data; using RaceDayAPI.Models;
namespace RaceDayAPI.Controllers;
[ApiController, Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db; public CategoriesController(RaceDayDbContext db) => _db = db;
    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _db.Categories.AsNoTracking().OrderBy(x => x.CategoryID).ToListAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var x = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryID == id); return x == null ? NotFound() : Ok(x); }
    public record CategoryRequest(string CategoryName, decimal DistanceKM, string ActivityType);
    [HttpPost, Authorize(Roles = "Organiser")] public async Task<IActionResult> Create(CategoryRequest r) { if (string.IsNullOrWhiteSpace(r.CategoryName) || string.IsNullOrWhiteSpace(r.ActivityType) || r.DistanceKM <= 0) return BadRequest("Category name, positive distance and activity type are required."); var x = new Category { CategoryName = r.CategoryName.Trim(), DistanceKM = r.DistanceKM, ActivityType = r.ActivityType.Trim() }; _db.Categories.Add(x); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = x.CategoryID }, x); }
    [HttpPut("{id:int}"), Authorize(Roles = "Organiser")] public async Task<IActionResult> Update(int id, CategoryRequest r) { var x = await _db.Categories.FindAsync(id); if (x == null) return NotFound(); if (string.IsNullOrWhiteSpace(r.CategoryName) || string.IsNullOrWhiteSpace(r.ActivityType) || r.DistanceKM <= 0) return BadRequest("Category name, positive distance and activity type are required."); x.CategoryName = r.CategoryName.Trim(); x.DistanceKM = r.DistanceKM; x.ActivityType = r.ActivityType.Trim(); await _db.SaveChangesAsync(); return Ok(x); }
    [HttpDelete("{id:int}"), Authorize(Roles = "Organiser")] public async Task<IActionResult> Delete(int id) { var x = await _db.Categories.FindAsync(id); if (x == null) return NotFound(); if (await _db.EventCategories.AnyAsync(e => e.CategoryID == id)) return Conflict("Category is linked to an event. Remove the event-category links first."); _db.Categories.Remove(x); await _db.SaveChangesAsync(); return NoContent(); }
}
