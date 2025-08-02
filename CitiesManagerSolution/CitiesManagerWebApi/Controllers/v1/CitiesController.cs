using CitiesManager.Core.Entites;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitiesManagerWebApi.Controllers.v1
{
    // we adding this to customcontrllerbase to make it available for all controllers
    //***********************
    //[Route("api/[controller]")]
    //[ApiController]
    //***********************

    // adding authorize filter globally to all controllers in prgoram.cs
    [ApiVersion("1.0")] // specify the version of this controller
    //[EnableCors("4100Client")]
    public class CitiesController : CustomControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        /// <summary>
        /// To get list of cities (including city ID and city Name) from cities table in database.
        /// </summary>
        /// <returns>list of cities found in database</returns>
        [HttpGet]
        //[Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.OrderBy(tmp => tmp.CityName).ToListAsync();
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(Guid id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(tmp => tmp.CityID == id);

            if (city == null)
            {
                //return NotFound();
                return Problem(detail: "Invalid CityID", statusCode: 400, title: "City Search");
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // fix overposting with [Bind] attribute (overposting: mean send not required data to model)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(Guid id, [Bind(nameof(City.CityID), nameof(City.CityName))] City city)
        {
            if (id != city.CityID)
            {
                return BadRequest();
            }

            var existCity = await _context.Cities.FindAsync(city.CityID);

            if (existCity == null)
            {
                return NotFound();
            }

            existCity.CityName = city.CityName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CityExists(id))
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

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // fix overposting with [Bind] attribute (overposting: mean send not required data to model)
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.CityID }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
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

        private async Task<bool> CityExists(Guid id)
        {
            return await _context.Cities.AnyAsync(e => e.CityID == id);
        }
    }
}
