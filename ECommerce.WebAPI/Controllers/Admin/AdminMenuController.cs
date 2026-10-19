using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Menu;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize]
    [Route("api/admin/menuler")]
    [ApiController]
    public class AdminMenuController : ControllerBase
    {
        private readonly IMenuServisi _menuServisi;

        public AdminMenuController(IMenuServisi menuServisi)
        {
            _menuServisi = menuServisi;
        }

        // Ön Yüz (Storefront) için konuma göre menü getiren metot
        [AllowAnonymous]
        [HttpGet("konum/{konum}")]
        public async Task<IActionResult> KonumaGoreGetir(string konum)
        {
            var menuler = await _menuServisi.KonumaGoreMenuleriGetirAsync(konum);
            return Ok(menuler);
        }

        [AllowAnonymous]
        [HttpGet("footer-bolumleri")]
        public async Task<IActionResult> FooterBolumleriniGetir()
        {
            var bolumler = await _menuServisi.FooterBolumleriniGetirAsync();
            return Ok(bolumler);
        }

        [HttpGet("gruplar")]
        public async Task<IActionResult> GruplariGetir()
        {
            var gruplar = await _menuServisi.TumGruplariGetirAsync();
            return Ok(gruplar);
        }

        [HttpGet("gruplar/{id:guid}")]
        public async Task<IActionResult> GrupGetir(Guid id)
        {
            var grup = await _menuServisi.GrupIdIleGetirAsync(id);
            if (grup == null) return NotFound("Menü grubu bulunamadı.");
            return Ok(grup);
        }

        [HttpPost("gruplar")]
        public async Task<IActionResult> GrupEkle([FromBody] MenuGrupEkleDto dto)
        {
            var id = await _menuServisi.GrupEkleAsync(dto);
            return Ok(new { Id = id, Mesaj = "Menü grubu başarıyla oluşturuldu." });
        }

        [HttpPut("gruplar")]
        public async Task<IActionResult> GrupGuncelle([FromBody] MenuGrupGuncelleDto dto)
        {
            await _menuServisi.GrupGuncelleAsync(dto);
            return Ok(new { Mesaj = "Menü grubu güncellendi." });
        }

        [HttpDelete("gruplar/{id:guid}")]
        public async Task<IActionResult> GrupSil(Guid id)
        {
            await _menuServisi.GrupSilAsync(id);
            return Ok(new { Mesaj = "Menü grubu silindi." });
        }

        [HttpPatch("gruplar/durum-degistir/{id:guid}")]
        [HttpPost("gruplar/durum-degistir/{id:guid}")]
        public async Task<IActionResult> GrupDurumDegistir(Guid id, [FromQuery] bool aktifMi)
        {
            await _menuServisi.GrupDurumDegistirAsync(id, aktifMi);
            return Ok(new { Mesaj = "Menü grubu durumu güncellendi.", AktifMi = aktifMi });
        }

        [HttpGet("ogeler/{grupId:guid}")]
        public async Task<IActionResult> OgeleriGetir(Guid grupId)
        {
            var ogeler = await _menuServisi.GrubaAitMenuleriGetirAsync(grupId);
            return Ok(ogeler);
        }

        // Tekil menü öğesi getirme (Düzenleme ekranı için)
        [HttpGet("ogeler/detay/{id:guid}")]
        public async Task<IActionResult> OgeGetir(Guid id)
        {
            var item = await _menuServisi.MenuItemIdIleGetirAsync(id);
            if (item == null) return NotFound("Menü öğesi bulunamadı.");
            return Ok(item);
        }

        [HttpPost("ogeler")]
        public async Task<IActionResult> OgeEkle([FromBody] MenuItemEkleDto dto)
        {
            var id = await _menuServisi.MenuItemEkleAsync(dto);
            return Ok(new { Id = id, Mesaj = "Menü öğesi eklendi." });
        }

        // Menü öğesi güncelleme
        [HttpPut("ogeler")]
        public async Task<IActionResult> OgeGuncelle([FromBody] MenuItemGuncelleDto dto)
        {
            await _menuServisi.MenuItemGuncelleAsync(dto);
            return Ok(new { Mesaj = "Menü öğesi güncellendi." });
        }

        [HttpDelete("ogeler/{id:guid}")]
        public async Task<IActionResult> OgeSil(Guid id)
        {
            await _menuServisi.MenuItemSilAsync(id);
            return Ok(new { Mesaj = "Menü öğesi silindi." });
        }
    }
}
