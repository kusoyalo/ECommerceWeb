using System.Diagnostics;
using System.Text.Json;
using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWeb.Controllers
{

    public class HomeController : Controller
    {
        private readonly ecommerceContext _ecommerceContext; //先在全域宣告資料庫物件

        public HomeController(ecommerceContext ecommerceContext) //這邊是依賴注入使用我們剛設定好的資料庫物件的寫法
        {
            _ecommerceContext = ecommerceContext;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("isLogin", "false");
            return View();
        }
        public IActionResult Login()
        {
            string? account = Request.Form["account"];
            string? password = Request.Form["password"];
            
            var user = (from a in _ecommerceContext.Users
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
                ViewData["Message"] = "無此帳號";
                return View("Index");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            //$2a$11$32kTKusFa.64883vY1vocO4aQG18vGg1xdB0GceTtkgax1HoI6wg.
            //701014的hash值

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) 
            {
                ViewData["Message"] = "密碼錯誤";
                return View("Index");
            }

            HttpContext.Session.SetString("userJson", JsonSerializer.Serialize(user));
            HttpContext.Session.SetString("isLogin", "true");

            var products = (from a in _ecommerceContext.Products
                            orderby Convert.ToInt32(a.ProductId) descending
                            select new Products
                        {
                            ProductId = a.ProductId,
                            ProductName = a.ProductName,
                            ProductCategory = a.ProductCategory,
                            Stock = a.Stock,
                            Status = a.Status,
                            ImageFile = a.ImageFile,
                            CreateTime = a.CreateTime,
                            LastModifiedBy = a.LastModifiedBy,
                            LastModifiedTime = a.LastModifiedTime
                        }).Take(10).ToList();

            ViewBag.products = products;

            return View("Menu", products);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("isLogin", "false");
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
