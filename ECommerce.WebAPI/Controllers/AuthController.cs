using System.Net;
using System.Security.Claims;
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
    private readonly IEmailServisi _emailServisi;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<UygulamaKullanicisi> userManager,
        SignInManager<UygulamaKullanicisi> signInManager,
        ITokenServisi tokenServisi,
        IEmailServisi emailServisi,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenServisi = tokenServisi;
        _emailServisi = emailServisi;
        _configuration = configuration;
    }

    [Authorize]
    [HttpGet("benim-profilim")]
    public async Task<IActionResult> BenimProfilim()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var kullaniciId)) return Unauthorized();
        var kullanici = await _userManager.FindByIdAsync(kullaniciId.ToString());
        if (kullanici == null) return Unauthorized();
        return Ok(new HesapProfilDto
        {
            AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}".Trim(),
            Email = kullanici.Email ?? string.Empty
        });
    }

    [Authorize]
    [HttpPost("sifre-degistir")]
    public async Task<IActionResult> SifreDegistir([FromBody] SifreDegistirDto dto)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var kullaniciId)) return Unauthorized();
        var kullanici = await _userManager.FindByIdAsync(kullaniciId.ToString());
        if (kullanici == null) return Unauthorized();
        var sonuc = await _userManager.ChangePasswordAsync(kullanici, dto.MevcutSifre, dto.YeniSifre);
        return sonuc.Succeeded ? NoContent() : BadRequest(new { mesaj = "Mevcut şifre hatalı veya yeni şifre geçersiz." });
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

        // Yeni kayıt olan herkese otomatik "Musteri" rolü atanır
        await _userManager.AddToRoleAsync(yeniKullanici, "Musteri");

        return Ok(new { mesaj = "Kullanıcı başarıyla kaydedildi." });
    }

    [AllowAnonymous] // Herkes giriş yapabilir
    [HttpPost("login")]
    public async Task<IActionResult> GirisYap([FromBody] GirisDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Sifre))
        {
            return BadRequest("Formdan e-posta veya şifre boş ulaştı.");
        }

        var kullanici = await _userManager.FindByEmailAsync(dto.Email);
        if (kullanici == null)
        {
            return Unauthorized("E-posta adresi veya şifre hatalı.");
        }

        var sifreKontrol = await _signInManager.CheckPasswordSignInAsync(kullanici, dto.Sifre, false);
        if (!sifreKontrol.Succeeded)
        {
            return Unauthorized("E-posta adresi veya şifre hatalı.");
        }

        var tokenStr = await _tokenServisi.TokenUretAsync(kullanici);

        var yanit = new TokenDto
        {
            Token = tokenStr,
            BitisTarihi = DateTime.UtcNow.AddDays(7),
            Mesaj = "Giriş başarılı."
        };

        return Ok(yanit);
    }

    [AllowAnonymous]
    [HttpPost("sifremi-unuttum")]
    public async Task<IActionResult> SifremiUnuttum([FromBody] SifremiUnuttumDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Email))
        {
            return BadRequest(new { mesaj = "Lütfen geçerli bir e-posta adresi giriniz." });
        }

        var kullanici = await _userManager.FindByEmailAsync(dto.Email);
        if (kullanici == null)
        {
            // Kullanıcı yoksa da bilgi sızıntısını önlemek veya net bilgilendirmek için
            return Ok(new { mesaj = "Eğer sistemde kayıtlı bir hesabınız varsa, şifre sıfırlama bağlantısı e-posta adresinize gönderilmiştir." });
        }

        // ASP.NET Core Identity Güvenli Token Üretimi
        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(kullanici);
        var encodedToken = WebUtility.UrlEncode(rawToken);

        var webUiUrl = _configuration["ClientSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Şifre sıfırlama bağlantıları için ClientSettings:BaseUrl ayarlanmalıdır.");
        var resetLink = $"{webUiUrl.TrimEnd('/')}/Hesap/SifreSifirla?email={Uri.EscapeDataString(kullanici.Email!)}&token={encodedToken}";

        // Botanik Tasarım Diliyle E-Posta Şablonu
        var adSoyad = string.IsNullOrWhiteSpace(kullanici.Ad) ? "Değerli Müşterimiz" : $"{kullanici.Ad} {kullanici.Soyad}".Trim();
        var emailHtml = $@"
        <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #fcf9f8; padding: 40px 20px; color: #1b1b1c;"">
            <div style=""max-width: 580px; margin: 0 auto; background: #ffffff; border-radius: 20px; border: 1px solid #ffd9de; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.05);"">
                <div style=""background: #1a3a2f; padding: 30px 25px; text-align: center; color: #ffffff;"">
                    <h2 style=""margin: 0; font-size: 24px; font-family: Georgia, serif; letter-spacing: 1px;"">Lidya Çiçekçilik</h2>
                    <p style=""margin: 6px 0 0; font-size: 13px; color: #c7ebda; text-transform: uppercase; letter-spacing: 2px;"">Şifre Sıfırlama Talebi</p>
                </div>
                <div style=""padding: 35px 30px; line-height: 1.6;"">
                    <p style=""font-size: 16px; margin-top: 0;"">Merhaba <strong>{adSoyad}</strong>,</p>
                    <p style=""font-size: 14px; color: #414845;"">
                        Lidya Çiçekçilik hesabınız için bir şifre sıfırlama talebinde bulundunuz. Yeni şifrenizi belirlemek için aşağıdaki butona tıklayabilirsiniz:
                    </p>
                    <div style=""text-align: center; margin: 35px 0;"">
                        <a href=""{resetLink}"" style=""background-color: #1a3a2f; color: #ffffff; text-decoration: none; padding: 14px 32px; border-radius: 50px; font-weight: bold; font-size: 14px; letter-spacing: 1px; display: inline-block; box-shadow: 0 4px 12px rgba(26,58,47,0.25);"">
                            ŞİFREMİ YENİLE
                        </a>
                    </div>
                    <p style=""font-size: 13px; color: #727974; border-top: 1px solid #f0eded; padding-top: 20px;"">
                        Eğer butona tıklayamıyorsanız, aşağıdaki bağlantıyı tarayıcınızın adres çubuğuna yapıştırabilirsiniz:<br/>
                        <a href=""{resetLink}"" style=""color: #984255; word-break: break-all; font-size: 12px;"">{resetLink}</a>
                    </p>
                    <p style=""font-size: 12px; color: #999999; margin-bottom: 0;"">
                        * Bu talep sizin tarafınızdan yapılmadıysa, bu e-postayı güvenle yok sayabilirsiniz. Şifreniz değişmeyecektir.
                    </p>
                </div>
                <div style=""background: #f6f3f2; padding: 15px; text-align: center; font-size: 12px; color: #727974;"">
                    © 2026 Lidya Çiçekçilik | Elazığ Çiçek Siparişi
                </div>
            </div>
        </div>";

        await _emailServisi.EpostaGonderAsync(kullanici.Email, "Lidya Çiçekçilik - Şifre Sıfırlama Bağlantısı", emailHtml);

        return Ok(new { mesaj = "Şifre sıfırlama bağlantısı e-posta adresinize başarıyla gönderildi. Lütfen gelen kutunuzu (ve gerekiyorsa spam klasörünü) kontrol ediniz." });
    }

    [AllowAnonymous]
    [HttpPost("sifre-sifirla")]
    public async Task<IActionResult> SifreSifirla([FromBody] SifreSifirlaDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.YeniSifre))
        {
            return BadRequest(new { mesaj = "Gerekli alanlar eksik gönderildi." });
        }

        if (dto.YeniSifre != dto.YeniSifreTekrar)
        {
            return BadRequest(new { mesaj = "Girdiğiniz yeni şifreler birbiriyle uyuşmuyor." });
        }

        var kullanici = await _userManager.FindByEmailAsync(dto.Email);
        if (kullanici == null)
        {
            return BadRequest(new { mesaj = "Kullanıcı bulunamadı veya bağlantı geçersiz." });
        }

        var sonuc = await _userManager.ResetPasswordAsync(kullanici, dto.Token, dto.YeniSifre);
        if (!sonuc.Succeeded)
        {
            var hataMesaji = string.Join(" ", sonuc.Errors.Select(e => e.Description));
            return BadRequest(new { mesaj = $"Şifre sıfırlanamadı: {hataMesaji}" });
        }

        return Ok(new { mesaj = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz." });
    }
}
