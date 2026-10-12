using System;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/sepet")]
[ApiController]
public class SepetController : ControllerBase
{
    private readonly ISepetServisi _sepetServisi;

    public SepetController(ISepetServisi sepetServisi)
    {
        _sepetServisi = sepetServisi;
    }

    [HttpGet("{sepetId}")]
    public async Task<IActionResult> Getir(string sepetId)
    {
        var sepet = await _sepetServisi.SepetGetirAsync(sepetId);
        return Ok(sepet);
    }

    [HttpPost("{sepetId}/ekle")]
    public async Task<IActionResult> UrunEkle(string sepetId, [FromBody] SepetKalemi kalem)
    {
        var sepet = await _sepetServisi.SepeteUrunEkleAsync(sepetId, kalem);
        return Ok(new { Mesaj = "Ürün başarıyla sepete eklendi.", Sepet = sepet });
    }

    [HttpDelete("{sepetId}/cikar/{urunId:guid}")]
    public async Task<IActionResult> UrunCikar(string sepetId, Guid urunId)
    {
        var sepet = await _sepetServisi.SepettenUrunCikarAsync(sepetId, urunId);
        return Ok(new { Mesaj = "Ürün başarıyla sepetten çıkarıldı.", Sepet = sepet });
    }

    [HttpPut("{sepetId}/guncelle/{urunId:guid}")]
    public async Task<IActionResult> MiktarGuncelle(string sepetId, Guid urunId, [FromBody] int miktar)
    {
        var sepet = await _sepetServisi.UrunMiktariGuncelleAsync(sepetId, urunId, miktar);
        return Ok(new { Mesaj = "Sepetteki ürün miktarı güncellendi.", Sepet = sepet });
    }

    [HttpDelete("{sepetId}/temizle")]
    public async Task<IActionResult> Temizle(string sepetId)
    {
        await _sepetServisi.SepetiTemizleAsync(sepetId);
        return Ok(new { Mesaj = "Sepet başarıyla temizlendi." });
    }

    [HttpPost("{sepetId}/ayir/{kalemId:guid}")]
    public async Task<IActionResult> UrunuAyir(string sepetId, Guid kalemId)
    {
        var sepet = await _sepetServisi.UrunuAyirAsync(sepetId, kalemId);
        return Ok(new { Mesaj = "Ürün başarıyla ayrıldı.", Sepet = sepet });
    }

    [HttpPost("{sepetId}/birlestir")]
    public async Task<IActionResult> SepetiBirlestir(string sepetId)
    {
        var sepet = await _sepetServisi.SepetUrunleriniBirlestirAsync(sepetId);
        return Ok(new { Mesaj = "Sepet birleştirildi.", Sepet = sepet });
    }

    [HttpPost("{sepetId}/kupon-uygula")]
    public async Task<IActionResult> KuponUygula(string sepetId, [FromBody] KuponIstekDto model)
    {
        try
        {
            var sepet = await _sepetServisi.KuponUygulaAsync(sepetId, model?.KuponKodu);
            return Ok(new { Mesaj = "Kupon başarıyla uygulandı.", Sepet = sepet });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Mesaj = ex.Message });
        }
    }

    [HttpDelete("{sepetId}/kupon-kaldir")]
    public async Task<IActionResult> KuponKaldir(string sepetId)
    {
        var sepet = await _sepetServisi.KuponKaldirAsync(sepetId);
        return Ok(new { Mesaj = "Kupon kaldırıldı.", Sepet = sepet });
    }

    public class KuponIstekDto
    {
        public string KuponKodu { get; set; }
    }
}