using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Services
{
    public class AuthServisi : IAuthServisi
    {
        private readonly UserManager<UygulamaKullanicisi> _userManager;
        private readonly SignInManager<UygulamaKullanicisi> _signInManager;
        private readonly ITokenServisi _tokenServisi;

        public AuthServisi(
            UserManager<UygulamaKullanicisi> userManager,
            SignInManager<UygulamaKullanicisi> signInManager,
            ITokenServisi tokenServisi)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServisi = tokenServisi;
        }

        public async Task<TokenDto> KayitOlAsync(KayitDto dto)
        {
            var mevcutKullanici = await _userManager.FindByEmailAsync(dto.Email);
            if (mevcutKullanici != null)
            {
                return new TokenDto
                {
                    Mesaj = "Bu e-posta adresiyle zaten bir kayıt bulunmaktadır."
                };
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
                var hatalar = string.Join(", ", sonuc.Errors.Select(e => e.Description));
                return new TokenDto
                {
                    Mesaj = $"Kayıt başarısız: {hatalar}"
                };
            }

            // Kayıt olduktan sonra otomatik JWT token üretip dönelim
            var token = await _tokenServisi.TokenUretAsync(yeniKullanici);

            return new TokenDto
            {
                Token = token,
                BitisTarihi = DateTime.UtcNow.AddDays(7),
                Mesaj = "Kayıt işlemi başarıyla tamamlandı."
            };
        }

        public async Task<TokenDto> GirisYapAsync(GirisDto dto)
        {
            var kullanici = await _userManager.FindByEmailAsync(dto.Email);
            if (kullanici == null)
            {
                return new TokenDto
                {
                    Mesaj = "E-posta veya şifre hatalı."
                };
            }

            var sifreKontrol = await _signInManager.CheckPasswordSignInAsync(kullanici, dto.Sifre, lockoutOnFailure: false);
            if (!sifreKontrol.Succeeded)
            {
                return new TokenDto
                {
                    Mesaj = "E-posta veya şifre hatalı."
                };
            }

            var token = await _tokenServisi.TokenUretAsync(kullanici);

            return new TokenDto
            {
                Token = token,
                BitisTarihi = DateTime.UtcNow.AddDays(7),
                Mesaj = "Giriş başarılı."
            };
        }
    }
}