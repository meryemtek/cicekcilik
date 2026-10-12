using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Context;
using ECommerce.Infrastructure.Repositories;
using ECommerce.WebAPI.Middlewares;
using ECommerce.WebAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ECommerce.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantý Dizesini Al ve DbContext Servisini Kaydet
var baglantiDizesi = builder.Configuration.GetConnectionString("VarsayilanBaglanti")
    ?? throw new InvalidOperationException("Baðlantý dizesi 'VarsayilanBaglanti' bulunamadý.");

builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(baglantiDizesi));

// 2. ASP.NET Core Identity (Kimlik Doðrulama ve Yetkilendirme) Yapýlandýrmasý
builder.Services.AddIdentity<UygulamaKullanicisi, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UygulamaDbContext>()
.AddDefaultTokenProviders();

// 3. JWT Authentication (Bearer) Yapýlandýrmasý
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret anahtarý bulunamadý.");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Alýþveriþ Sepeti Ýçin Distributed Memory Cache Kaydý
builder.Services.AddDistributedMemoryCache();

// 5. CORS POLÝTÝKASI (Dýþarýdan WebUI projesinin baðlanabilmesi için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 6. Baðýmlýlýk Enjeksiyonu (Dependency Injection - DI) Kayýtlarý
builder.Services.AddScoped(typeof(IDepo<>), typeof(Depo<>));

builder.Services.AddScoped<IDosyaServisi, DosyaServisi>();
builder.Services.AddScoped<ITokenServisi, TokenServisi>();
builder.Services.AddScoped<ISepetServisi, SepetServisi>();
builder.Services.AddScoped<ISayfaServisi, SayfaServisi>();
builder.Services.AddScoped<ISiteAyariServisi, SiteAyariServisi>();
builder.Services.AddScoped<ISliderServisi, SliderServisi>();
builder.Services.AddScoped<IKategoriServisi, KategoriServisi>();
builder.Services.AddScoped<IUrunServisi, UrunServisi>();
builder.Services.AddScoped<IBankaHesabiServisi, BankaHesabiServisi>();
builder.Services.AddScoped<ISssServisi, SssServisi>();
builder.Services.AddScoped<IBlogServisi, BlogServisi>();
builder.Services.AddScoped<IKuponServisi, KuponServisi>();
builder.Services.AddScoped<IUrunYorumServisi, UrunYorumServisi>();
builder.Services.AddScoped<IIletisimMesajiServisi, IletisimMesajiServisi>();
builder.Services.AddScoped<ISiparisServisi, SiparisServisi>();
builder.Services.AddScoped<IUrunVaryantServisi, UrunVaryantServisi>();
builder.Services.AddScoped<ISemtTeslimatServisi, SemtTeslimatServisi>();
builder.Services.AddScoped<ITopluUrunYonetimServisi, TopluUrunYonetimServisi>();
builder.Services.AddScoped<IKartNotuSablonuServisi, KartNotuSablonuServisi>();
builder.Services.AddScoped<ISiparisGorselOnayServisi, SiparisGorselOnayServisi>();
builder.Services.AddScoped<IAnasayfaKategoriServisi, AnasayfaKategoriServisi>();
builder.Services.AddScoped<IOneCikanServisi, OneCikanServisi>();
builder.Services.AddScoped<IDuyuruServisi, DuyuruServisi>();
builder.Services.AddScoped<IMenuServisi, MenuServisi>();
builder.Services.AddScoped<IOdemeServisi, PaytrOdemeServisi>();
builder.Services.AddHttpClient<ISmsServisi, NetGsmSmsServisi>();
builder.Services.AddScoped<IHiCariEntegrasyonServisi, HiCariTakipServisi>();
builder.Services.AddScoped<IAiServisi, OpenAiServisi>();
builder.Services.AddScoped<IKategoriBannerServisi, KategoriBannerServisi>();
builder.Services.AddScoped<IKampanyaServisi, KampanyaServisi>();
builder.Services.AddScoped<IHakkimizdaServisi, HakkimizdaServisi>();
builder.Services.AddScoped<IOdemeBildirimiServisi, OdemeBildirimiServisi>();
builder.Services.AddScoped<ITeslimatSaatiServisi, TeslimatSaatiServisi>();
builder.Services.AddScoped<ISiparisDeposu, SiparisDeposu>();
builder.Services.AddScoped<IFavoriServisi, FavoriServisi>();
builder.Services.AddScoped<IAuthServisi, AuthServisi>();

// Adres Seed Servisi eklendi
builder.Services.AddSingleton<IAdresHavuzuServisi, AdresHavuzuServisi>();

// 7. Controller ve Swagger / OpenAPI Yapýlandýrmasý (Kilit Butonu Eklendi)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token giriniz."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 8. GLOBAL EXCEPTION MIDDLEWARE (Sistem Çökmelerini Engeller - En baþta olmalý)
app.UseMiddleware<ExceptionMiddleware>();

// 9. HTTP Ýstek Hattý (Middleware Pipeline) Yapýlandýrmasý
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Statik dosyalarý (wwwroot resimlerini) dýþa açar
app.UseStaticFiles();

// CORS politikasýný aktifleþtirir
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 10. ROL SEEDING (Uygulama ilk ayaða kalktýðýnda rollerin otomatik oluþturulmasý)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    string[] roller = { "Admin", "Musteri" };
    foreach (var rol in roller)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(rol));
        }
    }
}

// 11. SEED DATA ÇALIÞTIRICISI
await app.SeedVerileriOlusturAsync();

// 12. EXCEL'den ADRES VERÝSÝ AKTARIMI
var env = app.Services.GetRequiredService<IWebHostEnvironment>();

var dosyaYolu = Path.Combine(
    env.WebRootPath,
    "data",
    "tum_adresler.xlsx"
);

var adresHavuzu = app.Services.GetRequiredService<IAdresHavuzuServisi>();
adresHavuzu.AdresleriYukle(dosyaYolu);

app.Run();