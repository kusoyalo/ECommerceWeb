using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.ApiControllers
{
    [Route("api")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly ILogger<LogoutController> _logger;
        private readonly ecommerceContext _ecommerceContext;
        public LogoutController(ILogger<LogoutController> logger, ecommerceContext ecommerceContext)
        {
            _logger = logger;
            _ecommerceContext = ecommerceContext;
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("Logout start");
            return Ok(new { message = "881 from Web API!" });
        }
    }
}
