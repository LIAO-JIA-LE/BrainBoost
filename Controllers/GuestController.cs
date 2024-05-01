using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Services;

namespace BrainBoost.Controllers
{
    [Route("[controller]")]
    public class GuestController(GuestService _guestService) : Controller
    {
        readonly GuestService guestService = _guestService;
        
        // [HttpPost]
        // [Route("")]
        // public IActionResult GuestLogin(Guest data){
        //     return Ok();
        // }
    }
}