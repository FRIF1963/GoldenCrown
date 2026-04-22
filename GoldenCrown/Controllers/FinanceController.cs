using GoldenCrown.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeSevice;

        public FinanceController(IFinanceService financeSevice)
        {
            _financeSevice = financeSevice;
        }

        [HttpGet("balance")] // Register([FromBody] DTOs.RegisterRequest request
        public async Task<IActionResult> Balance([FromHeader(Name = "Token")] string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _financeSevice.GetBalance(token);
            if (result != null) return Ok(result);

            return BadRequest(ModelState);
        }
    }
}
