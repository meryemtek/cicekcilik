using System.Security.Claims;
using ECommerce.Application.DTOs.Adres;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/hesap/adresler")]
public class KayitliAdresController : ControllerBase
{
    private readonly IKayitliAdresServisi _servis;

    public KayitliAdresController(IKayitliAdresServisi servis) => _servis = servis;

    private Guid KullaniciId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Listele() => Ok(await _servis.ListeleAsync(KullaniciId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var adres = await _servis.GetirAsync(KullaniciId, id);
        return adres == null ? NotFound() : Ok(adres);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] KayitliAdresKaydetDto dto)
    {
        var adres = await _servis.EkleAsync(KullaniciId, dto);
        return CreatedAtAction(nameof(Getir), new { id = adres.Id }, adres);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Guncelle(Guid id, [FromBody] KayitliAdresKaydetDto dto) =>
        await _servis.GuncelleAsync(KullaniciId, id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id) =>
        await _servis.SilAsync(KullaniciId, id) ? NoContent() : NotFound();

    [HttpPost("{id:guid}/varsayilan")]
    public async Task<IActionResult> VarsayilanYap(Guid id) =>
        await _servis.VarsayilanYapAsync(KullaniciId, id) ? NoContent() : NotFound();
}
