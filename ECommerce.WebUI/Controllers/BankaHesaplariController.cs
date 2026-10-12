using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.DTOs.OdemeBildirimi;
using ECommerce.WebUI.Models;

namespace ECommerce.WebUI.Controllers
{
    public class BankaHesaplariController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BankaHesaplariController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("hesap-numaralarimiz")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("banka-hesaplari/aktif");

            var bankalar = new List<BankaHesabiPublicDto>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                bankalar = JsonSerializer.Deserialize<List<BankaHesabiPublicDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var model = new BankaHesaplariViewModel
            {
                Bankalar = bankalar,
                BildirimDto = new()
            };

            return View(model);
        }

        [HttpPost("hesap-numaralarimiz")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BankaHesaplariViewModel viewModel)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(viewModel.BildirimDto.SiparisNo ?? ""), "SiparisNo");
            formData.Add(new StringContent(viewModel.BildirimDto.AdSoyad ?? ""), "AdSoyad");
            formData.Add(new StringContent(viewModel.BildirimDto.Telefon ?? ""), "Telefon");
            formData.Add(new StringContent(viewModel.BildirimDto.Email ?? ""), "Email");
            formData.Add(new StringContent(viewModel.BildirimDto.BankaAdi ?? ""), "BankaAdi");
            formData.Add(new StringContent(viewModel.BildirimDto.Tutar.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Tutar");
            formData.Add(new StringContent(viewModel.BildirimDto.OdemeTarihi ?? ""), "OdemeTarihi"); // Direkt string gönderiliyor
            formData.Add(new StringContent(viewModel.BildirimDto.Aciklama ?? ""), "Aciklama");

            if (viewModel.BildirimDto.Dekont != null && viewModel.BildirimDto.Dekont.Length > 0)
            {
                var fileContent = new StreamContent(viewModel.BildirimDto.Dekont.OpenReadStream());
                formData.Add(fileContent, "Dekont", viewModel.BildirimDto.Dekont.FileName);
            }

            var response = await client.PostAsync("odeme-bildirimi", formData);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Ödeme bildiriminiz başarıyla gönderildi.";
                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda banka listesini tekrar doldur
            var bankResponse = await client.GetAsync("banka-hesaplari/aktif");
            if (bankResponse.IsSuccessStatusCode)
            {
                var content = await bankResponse.Content.ReadAsStringAsync();
                viewModel.Bankalar = JsonSerializer.Deserialize<List<BankaHesabiPublicDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            TempData["HataMesaji"] = "Bildirim gönderilirken bir hata oluştu.";
            return View(viewModel);
        }
    }
}