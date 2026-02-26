using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace ECommerceWeb.ApiControllers
{
    [Route("api/accountManage")]
    [ApiController]
    public class AccountManageController : ControllerBase
    {
        private readonly ILogger<AccountManageController> _logger;
        private readonly ecommerceContext _ecommerceContext;
        public AccountManageController(ILogger<AccountManageController> logger, ecommerceContext ecommerceContext)
        {
            _logger = logger;
            _ecommerceContext = ecommerceContext;
        }
        [HttpGet("queryAccount")]
        public IActionResult QueryAccount([FromQuery] QueryAccountRequest req)
        {
            _logger.LogInformation("QueryAccount start");

            IQueryable<Users> query = _ecommerceContext.Users.AsQueryable();

            if (!string.IsNullOrEmpty(req.account))
            {
                query = query.Where(a => a.Account.Contains(req.account));
            }

            if (!string.IsNullOrEmpty(req.email))
            {
                query = query.Where(a => a.Email.Contains(req.email));
            }

            if (req.lastModifiedTimeStart.HasValue)
            {
                query = query.Where(a => a.LastModifiedTime >= req.lastModifiedTimeStart.Value);
            }

            if (req.lastModifiedTimeEnd.HasValue)
            {
                DateTime nextDay = req.lastModifiedTimeEnd.Value.AddDays(1);
                query = query.Where(a => a.LastModifiedTime <= nextDay);
            }

            List<Users> userList = query.Select(a => new Users
            {
                Account = a.Account,
                UserName = a.UserName,
                Email = a.Email,
                Role = a.Role,
                LastModifiedBy = a.LastModifiedBy,
                LastModifiedTime = a.LastModifiedTime
            }).ToList();

            return Ok(new { result = "ok", accountsJson = userList });
        }
        [HttpPost("accountCreate")]
        public async Task<IActionResult> AccountCreate([FromForm] AccountCreateRequest req)
        {
            _logger.LogInformation("AccountCreate start");

            //將密碼轉成Hash
            string hash = BCrypt.Net.BCrypt.HashPassword(req.password);

            _ecommerceContext.Users.Add(new Users
            {
                Account = req.account,
                UserName = req.userName,
                Email = req.email,
                PasswordHash = hash,
                Role = req.role,
                CreateTime = DateTime.Now,
                LastModifiedBy = req.lastModifiedBy,
                LastModifiedTime = DateTime.Now
            });
            await _ecommerceContext.SaveChangesAsync();

            return Ok(new { result = "ok" });
        }
        [HttpGet("queryDetail")]
        public IActionResult QueryDetail([FromQuery] string account)
        {
            _logger.LogInformation("QueryDetail start");

            Users user = (from a in _ecommerceContext.Users
                            where a.Account == account
                                select new Users
                            {
                                Account = a.Account,
                                UserName = a.UserName,
                                Email = a.Email,
                                Role = a.Role
                            }).SingleOrDefault();

            return Ok(new { result = "ok", usersJson = user });
        }
        [HttpPost("accountUpdate")]
        public async Task<IActionResult> AccountUpdate([FromForm] AccountUpdateRequest req)
        {
            _logger.LogInformation("AccountUpdate start");

            //將密碼轉成Hash
            string hash = BCrypt.Net.BCrypt.HashPassword(req.password);

            Users user = _ecommerceContext.Users.Find(req.account);
            if (user == null)
            {
                return Ok(new { result = "fail", message = "查無使用者" });
            }

            user.UserName = req.userName;
            user.Email = req.email;
            user.PasswordHash = hash;
            user.Role = req.role;
            user.LastModifiedBy = req.lastModifiedBy;
            user.LastModifiedTime = DateTime.Now;

            await _ecommerceContext.SaveChangesAsync();

            return Ok(new { result = "ok" });
        }

        [HttpPost("accountDelete")]
        public async Task<IActionResult> AccountDelete([FromBody] AccountDeleteRequest req)
        {
            _logger.LogInformation("AccountDelete start");

            Users user = _ecommerceContext.Users.Find(req.account);
            if (user == null)
            {
                return Ok(new { result = "fail", message = "查無使用者" });
            }

             _ecommerceContext.Users.Remove(user);
            await _ecommerceContext.SaveChangesAsync();

            return Ok(new { result = "ok" });
        }
    }
    public class QueryAccountRequest
    {
        public string? account { get; set; }
        public string? email { get; set; }
        public DateTime? lastModifiedTimeStart { get; set; }
        public DateTime? lastModifiedTimeEnd { get; set; }
    }
    public class AccountCreateRequest
    {
        public string? account { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
        public string? lastModifiedBy { get; set; }
    }
    public class AccountUpdateRequest
    {
        public string? account { get; set; }
        public string? userName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
        public string? lastModifiedBy { get; set; }
    }
    public class AccountDeleteRequest
    {
        public string? account { get; set; }
    }
}
