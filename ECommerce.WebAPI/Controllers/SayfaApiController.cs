using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/sayfalar")]
[ApiController]
public class SayfaApiController : ControllerBase
{
    private readonly ISayfaServisi _sayfaServisi;

    public SayfaApiController(ISayfaServisi sayfaServisi)
    {
        _sayfaServisi = sayfaServisi;
    }

    [HttpGet("{sayfaKodu}")]
    public async Task<IActionResult> GetirByKod(string sayfaKodu)
    {
        // Servisteki büyük/küçük harf duyarsız KodIleGetirAsync metodumuzu çağırıyoruz
        var sayfa = await _sayfaServisi.KodIleGetirAsync(sayfaKodu);

        if (sayfa == null || !sayfa.AktifMi)
        {
            return NotFound("Aradığınız sayfa bulunamadı veya yayından kaldırılmış.");
        }

        return Ok(sayfa);
    }

    [HttpGet("aktif-liste")]
    public async Task<IActionResult> AktifSayfalariGetir()
    {
        var sayfalar = await _sayfaServisi.TumSayfalariGetirAsync();
        var aktifSayfalar = sayfalar.Where(x => x.AktifMi).OrderBy(x => x.Baslik).ToList();
        return Ok(aktifSayfalar);
    }

    [HttpGet("detay-by-kod")]
    public async Task<IActionResult> GetirByQuery([FromQuery] string kod)
    {
        var sayfa = await _sayfaServisi.KodIleGetirAsync(kod);

        if (sayfa == null || !sayfa.AktifMi)
        {
            return NotFound("Aradığınız sayfa bulunamadı veya yayından kaldırılmış.");
        }

        return Ok(sayfa);
    }
}