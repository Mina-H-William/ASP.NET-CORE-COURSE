using CitiesManager.Core.Entites;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitiesManagerWebApi.Controllers.v2
{
    // we adding this to customcontrllerbase to make it available for all controllers
    //[Route("api/[controller]")]
    //[ApiController]
    [ApiVersion("2.0")] // specify the version of this controller
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
        public async Task<ActionResult<IEnumerable<string?>>> GetCities()
        {
            return await _context.Cities.OrderBy(tmp => tmp.CityName).Select(tmp => tmp.CityName).ToListAsync();
        }

    }
}
