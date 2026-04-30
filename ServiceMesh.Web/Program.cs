using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using ServiceMesh.Web.Service;
using ServiceMesh.Web.Service.IService;
using ServiceMesh.Web.Utility;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "SERVICEMESH_WEB__");
var indianCulture = new CultureInfo("en-IN");

CultureInfo.DefaultThreadCurrentCulture = indianCulture;
CultureInfo.DefaultThreadCurrentUICulture = indianCulture;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(indianCulture);
    options.SupportedCultures = new[] { indianCulture };
    options.SupportedUICultures = new[] { indianCulture };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IProductService,ProductService>();
builder.Services.AddHttpClient<ICouponService,CouponService>();
builder.Services.AddHttpClient<ICartService,CartService>();
builder.Services.AddHttpClient<IAuthService,AuthService>();
builder.Services.AddHttpClient<IOrderService,OrderService>();
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService,BaseService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICartService,CartService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ICouponService,CouponService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(10);
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
