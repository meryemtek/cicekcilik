using ECommerce.Application.DTOs.SiteAyari;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;

namespace ECommerce.Application.Services
{
    public class SiteAyariServisi : ISiteAyariServisi
    {
        private readonly IDepo<SiteAyari> _ayarDeposu;

        public SiteAyariServisi(IDepo<SiteAyari> ayarDeposu)
        {
            _ayarDeposu = ayarDeposu;
        }
        public async Task<SiteAyariDto> AyarlariGetirAsync()
        {
            var ayarlar = await _ayarDeposu.TumunuGetirAsync();
            var ayar = ayarlar.FirstOrDefault();

            if (ayar == null) return new SiteAyariDto();

            return new SiteAyariDto
            {
                Id = ayar.Id,
                FirmaAdi = ayar.FirmaAdi,
                Telefon1 = ayar.Telefon1,
                Telefon2 = ayar.Telefon2,
                Email = ayar.Email,
                Adres = ayar.Adres,
                LogoUrl = ayar.LogoUrl,
                TemaRengiHex = ayar.TemaRengiHex,
                FacebookUrl = ayar.FacebookUrl,
                InstagramUrl = ayar.InstagramUrl,
                TwitterUrl = ayar.TwitterUrl,
                GoogleMapsEmbedKodu = ayar.GoogleMapsEmbedKodu,
                UrunDetayDuyurulari = ayar.UrunDetayDuyurulari,
                VipTeslimatUcreti = ayar.VipTeslimatUcreti, // EKLENDİ
                AdminWhatsAppNumarasi = ayar.AdminWhatsAppNumarasi,
                NetGsmKullaniciAdi = ayar.NetGsmKullaniciAdi,
                NetGsmSifre = ayar.NetGsmSifre,
                NetGsmBaslik = ayar.NetGsmBaslik,
                PaytrMerchantId = ayar.PaytrMerchantId,
                PaytrMerchantKey = ayar.PaytrMerchantKey,
                PaytrMerchantSalt = ayar.PaytrMerchantSalt,
                OpenAiApiKey = ayar.OpenAiApiKey,
                HiCariEntegrasyonAktifMi = ayar.HiCariEntegrasyonAktifMi,
                HiCariApiUrl = ayar.HiCariApiUrl,
                HiCariApiKey = ayar.HiCariApiKey
            };
        }
        public async Task AyarlariGuncelleAsync(SiteAyariDto dto)
        {
            var ayarlar = await _ayarDeposu.TumunuGetirAsync();
            var mevcutAyar = ayarlar.FirstOrDefault();

            if (mevcutAyar == null)
            {
                var yeniAyar = new SiteAyari
                {
                    FirmaAdi = dto.FirmaAdi,
                    Telefon1 = dto.Telefon1,
                    Telefon2 = dto.Telefon2,
                    Email = dto.Email,
                    Adres = dto.Adres,
                    LogoUrl = dto.LogoUrl,
                    TemaRengiHex = dto.TemaRengiHex,
                    FacebookUrl = dto.FacebookUrl,
                    InstagramUrl = dto.InstagramUrl,
                    TwitterUrl = dto.TwitterUrl,
                    GoogleMapsEmbedKodu = dto.GoogleMapsEmbedKodu,
                    UrunDetayDuyurulari = dto.UrunDetayDuyurulari,
                    VipTeslimatUcreti = dto.VipTeslimatUcreti, // EKLENDİ
                    AdminWhatsAppNumarasi = dto.AdminWhatsAppNumarasi,
                    NetGsmKullaniciAdi = dto.NetGsmKullaniciAdi,
                    NetGsmSifre = dto.NetGsmSifre,
                    NetGsmBaslik = dto.NetGsmBaslik,
                    PaytrMerchantId = dto.PaytrMerchantId,
                    PaytrMerchantKey = dto.PaytrMerchantKey,
                    PaytrMerchantSalt = dto.PaytrMerchantSalt,
                    OpenAiApiKey = dto.OpenAiApiKey,
                    HiCariEntegrasyonAktifMi = dto.HiCariEntegrasyonAktifMi,
                    HiCariApiUrl = dto.HiCariApiUrl,
                    HiCariApiKey = dto.HiCariApiKey
                };

                await _ayarDeposu.EkleAsync(yeniAyar);
            }
            else
            {
                mevcutAyar.FirmaAdi = dto.FirmaAdi;
                mevcutAyar.Telefon1 = dto.Telefon1;
                mevcutAyar.Telefon2 = dto.Telefon2;
                mevcutAyar.Email = dto.Email;
                mevcutAyar.Adres = dto.Adres;
                mevcutAyar.LogoUrl = dto.LogoUrl;
                mevcutAyar.TemaRengiHex = dto.TemaRengiHex;
                mevcutAyar.FacebookUrl = dto.FacebookUrl;
                mevcutAyar.InstagramUrl = dto.InstagramUrl;
                mevcutAyar.TwitterUrl = dto.TwitterUrl;
                mevcutAyar.GoogleMapsEmbedKodu = dto.GoogleMapsEmbedKodu;
                mevcutAyar.UrunDetayDuyurulari = dto.UrunDetayDuyurulari;
                mevcutAyar.VipTeslimatUcreti = dto.VipTeslimatUcreti; // EKLENDİ
                mevcutAyar.AdminWhatsAppNumarasi = dto.AdminWhatsAppNumarasi;
                mevcutAyar.NetGsmKullaniciAdi = dto.NetGsmKullaniciAdi;
                mevcutAyar.NetGsmSifre = dto.NetGsmSifre;
                mevcutAyar.NetGsmBaslik = dto.NetGsmBaslik;
                mevcutAyar.PaytrMerchantId = dto.PaytrMerchantId;
                mevcutAyar.PaytrMerchantKey = dto.PaytrMerchantKey;
                mevcutAyar.PaytrMerchantSalt = dto.PaytrMerchantSalt;
                mevcutAyar.OpenAiApiKey = dto.OpenAiApiKey;
                mevcutAyar.GuncellenmeTarihi = DateTime.UtcNow;
                mevcutAyar.HiCariEntegrasyonAktifMi = dto.HiCariEntegrasyonAktifMi;
                mevcutAyar.HiCariApiUrl = dto.HiCariApiUrl;
                mevcutAyar.HiCariApiKey = dto.HiCariApiKey;
                _ayarDeposu.Guncelle(mevcutAyar);
            }
            await _ayarDeposu.DegisiklikleriKaydetAsync();
        }
    }
}
