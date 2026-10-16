using ECommerce.Application.DTOs.Kampanya;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization; // ÖNEMLİ: Yetkilendirme için
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Sınıf seviyesinde yetkilendirme (Admin girişi gerektirir)
    public class KampanyaController : ControllerBase
    {
        private readonly IKampanyaServisi _kampanyaServisi;

        public KampanyaController(IKampanyaServisi kampanyaServisi)
        {
            _kampanyaServisi = kampanyaServisi;
        }

        [HttpGet("liste")]
        [AllowAnonymous] // Ziyaretçilerin ve sitenin okuyabilmesi için yetki gerekmez
        public async Task<IActionResult> GetKampanyaUrunler()
        {
            var liste = await _kampanyaServisi.GetKampanyaUrunlerAsync();
            return Ok(liste);
        }

        [HttpGet("aktif-liste")]
        [AllowAnonymous] // Ziyaretçilerin ve sitenin okuyabilmesi için yetki gerekmez
        public async Task<IActionResult> GetAktifKampanyaUrunler()
        {
            var liste = await _kampanyaServisi.GetAktifKampanyaUrunlerAsync();
            return Ok(liste);
        }

        [HttpPost("ekle")]
        public async Task<IActionResult> Ekle([FromBody] KampanyaUrunEkleDto dto)
        {
            await _kampanyaServisi.AddUrunToKampanyaAsync(dto);
            return Ok(new { message = "Kampanya başarıyla oluşturuldu ve eklendi." });
        }

        [HttpPut("guncelle")]
        public async Task<IActionResult> Guncelle([FromBody] KampanyaUrunGuncelleDto dto)
        {
            await _kampanyaServisi.UpdateKampanyaUrunAsync(dto);
            return Ok(new { message = "Kampanya bilgileri güncellendi." });
        }

        [HttpDelete("sil/{id}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            await _kampanyaServisi.RemoveUrunFromKampanyaAsync(id);
            return Ok(new { message = "Kampanya sistemden kaldırıldı." });
        }

        [HttpPatch("durum-degistir/{id}")]
        [HttpPost("durum-degistir/{id}")]
        public async Task<IActionResult> DurumDegistir(Guid id, [FromBody] bool aktifMi)
        {
            await _kampanyaServisi.DurumDegistirAsync(id, aktifMi);
            return Ok(new { message = "Kampanya durumu başarıyla güncellendi." });
        }
    }
}