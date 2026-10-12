using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Duyuru;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize]
    [Route("api/admin/duyurular")]
    [ApiController]
    public class AdminDuyuruController : ControllerBase
    {
        private readonly IDuyuruServisi _duyuruServisi;

        public AdminDuyuruController(IDuyuruServisi duyuruServisi)
        {
            _duyuruServisi = duyuruServisi;
        }

        [HttpGet]
        public async Task<IActionResult> TumunuGetir()
        {
            var liste = await _duyuruServisi.TumDuyurulariGetirAsync();
            return Ok(liste);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Getir(Guid id)
        {
            var duyuru = await _duyuruServisi.IdIleGetirAsync(id);
            if (duyuru == null) return NotFound("Duyuru bulunamadı.");
            return Ok(duyuru);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] DuyuruEkleDto dto)
        {
            var id = await _duyuruServisi.DuyuruEkleAsync(dto);
            return Ok(new { Id = id, Mesaj = "Duyuru başarıyla eklendi." });
        }

        [HttpPut]
        public async Task<IActionResult> Guncelle([FromBody] DuyuruGuncelleDto dto)
        {
            await _duyuruServisi.DuyuruGuncelleAsync(dto);
            return Ok(new { Mesaj = "Duyuru güncellendi." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            await _duyuruServisi.DuyuruSilAsync(id);
            return Ok(new { Mesaj = "Duyuru silindi." });
        }
    }
}