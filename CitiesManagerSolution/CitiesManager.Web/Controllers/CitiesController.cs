using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.Web.Models;
using CitiesManager.Web.Data; 

namespace CitiesManager.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            if (_context.Cities == null) return NotFound();
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        // GET: api/City/5
        [HttpGet]
        [Route("{cityId}")]
        public async Task<ActionResult<City>> GetCity(Guid cityId)
        {
            var city = await _context.Cities.FindAsync(cityId);

            if (city == null)
            {
                return Problem(detail: "Invalid CityId", statusCode: 400, title: "City Search"); 
            }

            return city;
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(Guid? id,[Bind(nameof(City.Id),nameof(City.Name))] City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            var existingCity = await _context.Cities.FindAsync(id);
            if(existingCity == null)
            {
                return NotFound();
            }
            existingCity.Name = city.Name;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/City
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            if (_context.Cities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Citites' is null.");
            }

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { cityId = city.Id }, city);//api/cities/{cityId}
        }

        // DELETE: api/City/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid? id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid? id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}