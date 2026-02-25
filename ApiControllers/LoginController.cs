using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.ApiControllers
{
    [Route("api")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ecommerceContext _ecommerceContext;
        public LoginController(ILogger<LoginController> logger, ecommerceContext ecommerceContext)
        {
            _logger = logger;
            _ecommerceContext = ecommerceContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            _logger.LogInformation("Login start");

            string account = req.account;

            Users user = (from a in _ecommerceContext.Users
                        where a.Account == account
                        select new Users
                        {
                            Account = a.Account,
                            UserName = a.UserName,
                            Email = a.Email,
                            PasswordHash = a.PasswordHash,
                            Role = a.Role,
                            CreateTime = a.CreateTime,
                            LastModifiedBy = a.LastModifiedBy,
                            LastModifiedTime = a.LastModifiedTime
                        }).SingleOrDefault();

            if (user == null)
            {
                return Ok(new { result = "fail" ,message = "無此帳號" });
            }

            string password = req.password;
            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            //$2a$11$32kTKusFa.64883vY1vocO4aQG18vGg1xdB0GceTtkgax1HoI6wg.
            //701014的hash值

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return Ok(new { result = "fail", message = "密碼錯誤" });
            }

            return Ok(new { result = "ok", userJson = user });
        }
    }
    public class LoginRequest
    {
        public string account { get; set; }
        public string password { get; set; }
    }
}
