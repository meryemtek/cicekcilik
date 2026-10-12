using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Infrastructure.Services
{
    public class TokenServisi : ITokenServisi
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UygulamaKullanicisi> _userManager; // UserManager eklendi

        public TokenServisi(IConfiguration configuration, UserManager<UygulamaKullanicisi> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> TokenUretAsync(UygulamaKullanicisi kullanici)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret anahtarı bulunamadı.");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireDays = Convert.ToInt32(jwtSettings["ExpireDays"] ?? "7");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            // Temel Bilgiler (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Email, kullanici.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{kullanici.Ad} {kullanici.Soyad}")
            };

            // KRİTİK: Kullanıcının rollerini veritabanından çekip token'a ekliyoruz
            var roller = await _userManager.GetRolesAsync(kullanici);
            foreach (var rol in roller)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expireDays),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}