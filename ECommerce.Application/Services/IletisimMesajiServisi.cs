using ECommerce.Application.DTOs.IletisimMesaji;
using ECommerce.Application.DTOs.Menu;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class IletisimMesajiServisi : IIletisimMesajiServisi
    {
        private readonly IDepo<IletisimMesaji> _depo;

        public IletisimMesajiServisi(IDepo<IletisimMesaji> depo)
        {
            _depo = depo;
        }

        public async Task<IReadOnlyList<IletisimMesajiListeDto>> TumMesajlariGetirAsync()
        {
            var mesajlar = await _depo.TumunuGetirAsync();
            return mesajlar.OrderByDescending(m => m.OlusturulmaTarihi).Select(m => new IletisimMesajiListeDto
            {
                Id = m.Id,
                GonderenAdSoyad = m.GonderenAdSoyad,
                GonderenEmail = m.GonderenEmail,
                Konu = m.Konu,
                OlusturulmaTarihi = m.OlusturulmaTarihi,
                OkunduMu = m.OkunduMu
            }).ToList();
        }

        public async Task<IletisimMesajiDetayDto?> MesajOkuAsync(Guid id)
        {
            var mesaj = await _depo.IdIleGetirAsync(id);
            if (mesaj == null) return null;

            if (!mesaj.OkunduMu)
            {
                mesaj.OkunduMu = true;
                _depo.Guncelle(mesaj);
                await _depo.DegisiklikleriKaydetAsync();
            }

            return new IletisimMesajiDetayDto
            {
                Id = mesaj.Id,
                GonderenAdSoyad = mesaj.GonderenAdSoyad,
                GonderenEmail = mesaj.GonderenEmail,
                Telefon = mesaj.Telefon,
                Konu = mesaj.Konu,
                Mesaj = mesaj.Mesaj,
                OlusturulmaTarihi = mesaj.OlusturulmaTarihi,
                OkunduMu = mesaj.OkunduMu
            };
        }

        public async Task<Guid> EkleAsync(IletisimMesajiEkleDto dto)
        {
            var mesaj = new IletisimMesaji
            {
                GonderenAdSoyad = dto.GonderenAdSoyad,
                GonderenEmail = dto.GonderenEmail,
                Telefon = dto.Telefon,
                Konu = dto.Konu,
                Mesaj = dto.Mesaj,
                OkunduMu = false
            };
            await _depo.EkleAsync(mesaj);
            await _depo.DegisiklikleriKaydetAsync();
            return mesaj.Id;
        }

        public async Task SilAsync(Guid id)
        {
            var mesaj = await _depo.IdIleGetirAsync(id);
            if (mesaj != null)
            {
                _depo.Sil(mesaj);
                await _depo.DegisiklikleriKaydetAsync();
            }
        }

        public async Task<int> OkunmamisMesajSayisiGetirAsync()
        {
            var mesajlar = await _depo.GetirAsync(m => !m.OkunduMu);
            return mesajlar.Count;
        }
    }
}