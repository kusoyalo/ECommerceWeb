using ECommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace ECommerceWeb.ApiControllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ecommerceContext _ecommerceContext;
        public ProductController(ILogger<ProductController> logger, ecommerceContext ecommerceContext)
        {
            _logger = logger;
            _ecommerceContext = ecommerceContext;
        }

        [HttpGet("queryDefault")]
        public IActionResult QueryDefault()
        {
            _logger.LogInformation("QueryDefault start");

            List<Products> products = (from a in _ecommerceContext.Products
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
                                       }).Take(3).ToList();

            return Ok(new { result = "ok", productsJson = products });
        }

        [HttpGet("queryProduct")]
        public IActionResult QueryProduct([FromQuery] QueryProductRequest req)
        {
            _logger.LogInformation("QueryProduct start");

            IQueryable<Products> query = _ecommerceContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(req.productId))
            {
                query = query.Where(a => a.ProductId == req.productId);
            }

            if (!string.IsNullOrEmpty(req.productName))
            {
                query = query.Where(a => a.ProductName.Contains(req.productName));
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

            List<Products> productList = query.Select(a => new Products
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
            }).ToList();

            return Ok(new { result = "ok", productsJson = productList });
        }
        [HttpGet("queryDetail")]
        public IActionResult QueryDetail([FromQuery] string productId)
        {
            _logger.LogInformation("QueryDetail start");

            Products product = (from a in _ecommerceContext.Products
                                where a.ProductId == productId
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
                                }).SingleOrDefault();

            return Ok(new { result = "ok", productsJson = product });
        }

        [HttpPost("productCreate")]
        public async Task<IActionResult> ProductCreate([FromForm] ProductCreateRequest req)
        {
            _logger.LogInformation("ProductCreate start");

            //處理圖片並轉為byte[]存入資料庫
            byte[] imageBytes = null;
            if (req.imageFile != null)
            {
                using MemoryStream ms = new MemoryStream();
                await req.imageFile.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            _ecommerceContext.Products.Add(new Products
            {
                ProductId = req.productId,
                ProductName = req.productName,
                ProductCategory = req.productCategory,
                Stock = req.stock,
                Status = req.status,
                ImageFile = imageBytes,
                CreateTime = DateTime.Now,
                LastModifiedBy = req.lastModifiedBy,
                LastModifiedTime = DateTime.Now
            });
            await _ecommerceContext.SaveChangesAsync();

            return Ok(new { result = "ok"});
        }
    }
    public class QueryProductRequest
    {
        public string? productId { get; set; }
        public string? productName { get; set; }
        public DateTime? lastModifiedTimeStart { get; set; }
        public DateTime? lastModifiedTimeEnd { get; set; }
    }
    public class ProductCreateRequest
    {
        public string? productId { get; set; }
        public string? productName { get; set; }
        public string? productCategory { get; set; }
        public int? stock { get; set; }
        public string? status { get; set; }
        public IFormFile? imageFile { get; set; }
        public string? lastModifiedBy { get; set; }
    }
}
