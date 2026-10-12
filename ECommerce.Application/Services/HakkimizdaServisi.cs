using System.Text.Json;
using ECommerce.Application.DTOs.Hakkimizda;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class HakkimizdaServisi : IHakkimizdaServisi
    {
        private readonly IDepo<Hakkimizda> _hakkimizdaDeposu;

        public HakkimizdaServisi(IDepo<Hakkimizda> hakkimizdaDeposu)
        {
            _hakkimizdaDeposu = hakkimizdaDeposu;
        }

        public async Task<HakkimizdaAyariDto> GetirAsync()
        {
            var kayitlar = await _hakkimizdaDeposu.TumunuGetirAsync();
            var ayar = kayitlar.FirstOrDefault();

            if (ayar == null)
            {
                return new HakkimizdaAyariDto(); // Henüz kayıt yoksa boş DTO döner
            }

            return new HakkimizdaAyariDto
            {
                HikayemizAktifMi = ayar.HikayemizAktifMi,
                HikayemizBaslik = ayar.HikayemizBaslik,
                HikayemizMetni = ayar.HikayemizMetni,
                GorselUrl = ayar.GorselUrl, // Eklendi
                NedenBiziAktifMi = ayar.NedenBiziAktifMi,
                DegerlerimizAktifMi = ayar.DegerlerimizAktifMi,
                HizmetBolgeleriAktifMi = ayar.HizmetBolgeleriAktifMi,
                SeoTitle = ayar.SeoTitle,
                SeoDescription = ayar.SeoDescription,

                // Neden Bizi JSON verisini güvenli bir şekilde Listeye dönüştürüyoruz
                NedenBiziItems = string.IsNullOrEmpty(ayar.NedenBiziJson)
                    ? new()
                    : (JsonSerializer.Deserialize<List<NedenBiziItemDto>>(ayar.NedenBiziJson) ?? new()),

                // Değerlerimiz JSON verisini güvenli bir şekilde Listeye dönüştürüyoruz
                Degerlerimiz = string.IsNullOrEmpty(ayar.DegerlerimizJson)
                    ? new()
                    : (JsonSerializer.Deserialize<List<DegerItemDto>>(ayar.DegerlerimizJson) ?? new()),

                // Hizmet Bölgeleri JSON verisini güvenli bir şekilde Listeye dönüştürüyoruz
                HizmetBolgeleri = string.IsNullOrEmpty(ayar.HizmetBolgeleriJson)
                    ? new()
                    : (JsonSerializer.Deserialize<List<string>>(ayar.HizmetBolgeleriJson) ?? new())
            };
        }

        public async Task GuncelleAsync(HakkimizdaAyariDto dto)
        {
            var kayitlar = await _hakkimizdaDeposu.TumunuGetirAsync();
            var ayar = kayitlar.FirstOrDefault();

            if (ayar == null)
            {
                // Eğer daha önce hiç kayıt oluşturulmadıysa yeni ekliyoruz
                ayar = new Hakkimizda
                {
                    HikayemizAktifMi = dto.HikayemizAktifMi,
                    HikayemizBaslik = dto.HikayemizBaslik,
                    HikayemizMetni = dto.HikayemizMetni,
                    GorselUrl = dto.GorselUrl, // Eklendi
                    NedenBiziAktifMi = dto.NedenBiziAktifMi,
                    DegerlerimizAktifMi = dto.DegerlerimizAktifMi,
                    HizmetBolgeleriAktifMi = dto.HizmetBolgeleriAktifMi,
                    SeoTitle = dto.SeoTitle,
                    SeoDescription = dto.SeoDescription,
                    NedenBiziJson = JsonSerializer.Serialize(dto.NedenBiziItems ?? new()),
                    DegerlerimizJson = JsonSerializer.Serialize(dto.Degerlerimiz ?? new()),
                    HizmetBolgeleriJson = JsonSerializer.Serialize(dto.HizmetBolgeleri ?? new())
                };

                await _hakkimizdaDeposu.EkleAsync(ayar);
            }
            else
            {
                // Varsa güncelliyoruz
                ayar.HikayemizAktifMi = dto.HikayemizAktifMi;
                ayar.HikayemizBaslik = dto.HikayemizBaslik;
                ayar.HikayemizMetni = dto.HikayemizMetni;
                ayar.GorselUrl = dto.GorselUrl; // Eklendi
                ayar.NedenBiziAktifMi = dto.NedenBiziAktifMi;
                ayar.DegerlerimizAktifMi = dto.DegerlerimizAktifMi;
                ayar.HizmetBolgeleriAktifMi = dto.HizmetBolgeleriAktifMi;
                ayar.SeoTitle = dto.SeoTitle;
                ayar.SeoDescription = dto.SeoDescription;

                // Tüm listeleri JSON formatına çevirip kaydediyoruz
                ayar.NedenBiziJson = JsonSerializer.Serialize(dto.NedenBiziItems ?? new());
                ayar.DegerlerimizJson = JsonSerializer.Serialize(dto.Degerlerimiz ?? new());
                ayar.HizmetBolgeleriJson = JsonSerializer.Serialize(dto.HizmetBolgeleri ?? new());

                _hakkimizdaDeposu.Guncelle(ayar);
            }

            await _hakkimizdaDeposu.DegisiklikleriKaydetAsync();
        }
    }
}