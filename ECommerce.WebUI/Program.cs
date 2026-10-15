using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// 1. WebAPI Projesine �stek Atacak HttpClient Yap�land�rmas�
// PORT NUMARASINI 7192 OLARAK G�NCELL�YORUZ
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7192/api/");
});

//// 1. WebAPI Projesine �stek Atacak HttpClient Yap�land�rmas�
//builder.Services.AddHttpClient("ApiClient", client =>
//{
//    // appsettings.json dosyas�ndan okur, e�er bulamazsa canl� adresinizi baz al�r
//    var apiBaseUrl = builder.Configuration["ApiSettings:ApiBaseUrl"] ?? "https://api.demo.elaziglidyacicekcilik.com";
//    client.BaseAddress = new Uri(apiBaseUrl.EndsWith("/") ? apiBaseUrl : apiBaseUrl + "/");
//});

// HttpContext'e ve taray�c�daki Cookie'lere eri�ebilmek i�in eklenen servis
builder.Services.AddHttpContextAccessor();

// 2. Taray�c�da Oturum Y�netimi (Cookie Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Hesap/Giris";
        options.LogoutPath = "/Hesap/Cikis";
        options.AccessDeniedPath = "/Hesap/Giris";
        options.Cookie.Name = "ECommerceAuth";
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

app.UseAuthentication(); // Cookie'yi okumas� i�in eklendi
app.UseAuthorization();

// 3. Admin Paneli (Area) Y�nlendirmesi
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
