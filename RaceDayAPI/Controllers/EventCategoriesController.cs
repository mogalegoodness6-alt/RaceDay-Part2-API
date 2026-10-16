using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using RaceDayAPI.Data; using RaceDayAPI.Models;
namespace RaceDayAPI.Controllers;
[ApiController, Route("api/events/{eventId:int}/categories")]
public class EventCategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db; public EventCategoriesController(RaceDayDbContext db) => _db = db;
    [HttpGet] public async Task<IActionResult> Get(int eventId) { if (!await _db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound("Event not found."); var list = await (from ec in _db.EventCategories join c in _db.Categories on ec.CategoryID equals c.CategoryID where ec.EventID == eventId select c).AsNoTracking().ToListAsync(); return Ok(list); }
    public record LinkRequest(int CategoryID);
    [HttpPost, Authorize(Roles = "Organiser")] public async Task<IActionResult> Add(int eventId, LinkRequest r) { if (!await _db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound("Event not found."); if (!await _db.Categories.AnyAsync(c => c.CategoryID == r.CategoryID)) return NotFound("Category not found."); if (await _db.EventCategories.AnyAsync(x => x.EventID == eventId && x.CategoryID == r.CategoryID)) return Conflict("This category is already linked to the event."); _db.EventCategories.Add(new EventCategory { EventID = eventId, CategoryID = r.CategoryID }); await _db.SaveChangesAsync(); return Created($"/api/events/{eventId}/categories", new { eventId, r.CategoryID }); }
}
