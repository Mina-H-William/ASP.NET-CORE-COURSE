using Microsoft.AspNetCore.Mvc;

namespace CitiesManagerWebApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    // with apicontroller all action will handle model validation automatically no need to check ModelState.IsValid
    // and it will return 400 Bad Request if model is invalid with validation errors
    // With [ApiController] attribute JSON model binding becomes case-insensitive by default
    [ApiController]
    public class CustomControllerBase : ControllerBase
    {
    }
}
