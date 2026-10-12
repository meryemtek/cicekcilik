using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. WebAPI Projesine Ýstek Atacak HttpClient Yapýlandýrmasý
// PORT NUMARASINI 7192 OLARAK GÜNCELLÝYORUZ
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7192/api/");
});

//// 1. WebAPI Projesine Ýstek Atacak HttpClient Yapýlandýrmasý
//builder.Services.AddHttpClient("ApiClient", client =>
//{
//    // appsettings.json dosyasýndan okur, eðer bulamazsa canlý adresinizi baz alýr
//    var apiBaseUrl = builder.Configuration["ApiSettings:ApiBaseUrl"] ?? "https://api.demo.elaziglidyacicekcilik.com";
//    client.BaseAddress = new Uri(apiBaseUrl.EndsWith("/") ? apiBaseUrl : apiBaseUrl + "/");
//});

// HttpContext'e ve tarayýcýdaki Cookie'lere eriþebilmek için eklenen servis
builder.Services.AddHttpContextAccessor();

// 2. Tarayýcýda Oturum Yönetimi (Cookie Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Auth/Login";
        options.LogoutPath = "/Admin/Auth/Logout";
        options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
        options.Cookie.Name = "ECommerceAdminAuth";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Cookie'yi okumasý için eklendi
app.UseAuthorization();

// 3. Admin Paneli (Area) Yönlendirmesi
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();