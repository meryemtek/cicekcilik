using System.Threading.Tasks;
using ECommerce.Application.DTOs.Odeme;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/odeme")]
[ApiController]
public class OdemeController : ControllerBase
{
    private readonly IOdemeServisi _odemeServisi;
    private readonly ISiparisServisi _siparisServisi;

    public OdemeController(IOdemeServisi odemeServisi, ISiparisServisi siparisServisi)
    {
        _odemeServisi = odemeServisi;
        _siparisServisi = siparisServisi;
    }

    [HttpPost("baslat")]
    public async Task<IActionResult> OdemeBaslat([FromBody] OdemeIstekDto istek)
    {
        var sonuc = await _odemeServisi.OdemeBaslatAsync(istek);
        if (!sonuc.BasariliMi)
        {
            return BadRequest(new { Hata = sonuc.HataMesaji });
        }

        return Ok(sonuc);
    }

    [AllowAnonymous]
    [HttpPost("bildirim")]
    public async Task<IActionResult> Bildirim(
        [FromForm(Name = "merchant_oid")] string merchantOid,
        [FromForm(Name = "status")] string status,
        [FromForm(Name = "total_amount")] string totalAmount,
        [FromForm(Name = "hash")] string hash)
    {
        if (!await _odemeServisi.PaytrBildirimHashGecerliMiAsync(merchantOid, status, totalAmount, hash))
        {
            return BadRequest("PAYTR notification failed: bad hash");
        }

        if (!long.TryParse(totalAmount, System.Globalization.NumberStyles.None,
            System.Globalization.CultureInfo.InvariantCulture, out var parsedTotalAmount))
        {
            return BadRequest("PAYTR notification failed: invalid amount");
        }

        if (!await _siparisServisi.PaytrSonucunuIsleAsync(merchantOid, status, parsedTotalAmount))
        {
            return BadRequest("PAYTR notification failed: order not found or amount mismatch");
        }

        return Content("OK", "text/plain");
    }
}
