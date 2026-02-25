using ECommerceWeb.Models;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

//初始化 NLog
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);

//核心設定：移除預設 Log 並改用 NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ecommerceContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ecommerceDatabase")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); //必要的快取支援
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); //設定逾時時間
    options.Cookie.HttpOnly = true; //安全性設定
});

//定義策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   //允許所有來源，開發環境方便使用
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); //啟用中間件(必須放在UseRouting之後，UseAuthorization之前)

//啟用CORS(必須放在UseRouting之後，UseAuthorization之前)
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//映射控制器路由，這會讓 [Route] 屬性生效
app.MapControllers();

app.Run();
