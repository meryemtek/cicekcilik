using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<UygulamaKullanicisi> _userManager;
    private readonly SignInManager<UygulamaKullanicisi> _signInManager;
    private readonly ITokenServisi _tokenServisi;

    public AuthController(
        UserManager<UygulamaKullanicisi> userManager,
        SignInManager<UygulamaKullanicisi> signInManager,
        ITokenServisi tokenServisi)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenServisi = tokenServisi;
    }

    [AllowAnonymous] // Herkes üye olabilir
    [HttpPost("kayit-ol")]
    public async Task<IActionResult> KayitOl([FromBody] KayitDto dto)
    {
        var mevcutKullanici = await _userManager.FindByEmailAsync(dto.Email);
        if (mevcutKullanici != null)
        {
            return BadRequest(new { mesaj = "Bu e-posta adresi ile zaten bir kayıt bulunmaktadır." });
        }

        var yeniKullanici = new UygulamaKullanicisi
        {
            UserName = dto.Email,
            Email = dto.Email,
            Ad = dto.Ad,
            Soyad = dto.Soyad,
            PhoneNumber = dto.Telefon,
            KayitTarihi = DateTime.UtcNow
        };

        var sonuc = await _userManager.CreateAsync(yeniKullanici, dto.Sifre);
        if (!sonuc.Succeeded)
        {
            return BadRequest(sonuc.Errors);
        }

        // PRO-LEVEL KURALI: Yeni kayıt olan herkese otomatik "Musteri" rolü atanır
        await _userManager.AddToRoleAsync(yeniKullanici, "Musteri");

        return Ok(new { mesaj = "Kullanıcı başarıyla kaydedildi." });
    }

    [AllowAnonymous] // Herkes giriş yapabilir
    [HttpPost("login")]
    public async Task<IActionResult> GirisYap([FromBody] GirisDto dto)
    {
        // 1. DTO KONTROLÜ
        if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Sifre))
        {
            return BadRequest("API DEDEKTİFİ: Formdan API'ye e-posta veya şifre boş ulaştı!");
        }

        // 2. KULLANICI KONTROLÜ
        var kullanici = await _userManager.FindByEmailAsync(dto.Email);
        if (kullanici == null)
        {
            return Unauthorized($"API DEDEKTİFİ: '{dto.Email}' adresi veritabanında bulunamadı.");
        }

        // 3. ŞİFRE KONTROLÜ
        var sifreKontrol = await _signInManager.CheckPasswordSignInAsync(kullanici, dto.Sifre, false);
        if (!sifreKontrol.Succeeded)
        {
            return Unauthorized("API DEDEKTİFİ: Şifre eşleşmiyor.");
        }

        // Rolleri de içeren JWT Token üretimi
        var tokenStr = await _tokenServisi.TokenUretAsync(kullanici);

        var yanit = new TokenDto
        {
            Token = tokenStr,
            BitisTarihi = DateTime.UtcNow.AddDays(7),
            Mesaj = "Giriş başarılı."
        };

        return Ok(yanit);
    }
}