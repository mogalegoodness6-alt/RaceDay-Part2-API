using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using RaceDayAPI.Data;
namespace RaceDayAPI.Controllers;
[ApiController, Route("api")]
public class ReferenceDataController : ControllerBase
{
    private readonly RaceDayDbContext _db; public ReferenceDataController(RaceDayDbContext db) => _db = db;
    [HttpGet("routes")] public async Task<IActionResult> Routes() => Ok(await _db.Routes.AsNoTracking().OrderBy(x => x.RouteID).ToListAsync());
    [HttpGet("routes/{id:int}")] public async Task<IActionResult> Route(int id) { var x = await _db.Routes.AsNoTracking().FirstOrDefaultAsync(x => x.RouteID == id); return x == null ? NotFound() : Ok(x); }
    [HttpGet("weather")] public async Task<IActionResult> Weather() => Ok(await _db.Weather.AsNoTracking().OrderBy(x => x.WeatherID).ToListAsync());
    [HttpGet("weather/{id:int}")] public async Task<IActionResult> WeatherById(int id) { var x = await _db.Weather.AsNoTracking().FirstOrDefaultAsync(x => x.WeatherID == id); return x == null ? NotFound() : Ok(x); }
}
