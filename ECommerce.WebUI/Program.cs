using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var apiBaseUrl = builder.Configuration["ApiSettings:ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    if (builder.Environment.IsProduction())
    {
        throw new InvalidOperationException("API adresini ApiSettings__ApiBaseUrl ortam değişkeniyle tanımlayın.");
    }

    apiBaseUrl = "https://localhost:7192";
}

apiBaseUrl = apiBaseUrl.TrimEnd('/');

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri($"{apiBaseUrl}/api/");
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Hesap/Giris";
        options.LogoutPath = "/Hesap/Cikis";
        options.AccessDeniedPath = "/Hesap/Giris";
        options.Cookie.Name = "ECommerceAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsProduction()
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
