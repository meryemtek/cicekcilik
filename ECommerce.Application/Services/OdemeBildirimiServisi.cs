using ECommerce.Application.DTOs.OdemeBildirimi;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class OdemeBildirimiServisi : IOdemeBildirimiServisi
    {
        private readonly IDepo<OdemeBildirimi> _depo;

        public OdemeBildirimiServisi(IDepo<OdemeBildirimi> depo)
        {
            _depo = depo;
        }

        public async Task<IReadOnlyList<OdemeBildirimiListeDto>> TumBildirimleriGetirAsync()
        {
            var liste = await _depo.TumunuGetirAsync();
            return liste.OrderByDescending(x => x.OlusturulmaTarihi).Select(x => new OdemeBildirimiListeDto
            {
                Id = x.Id,
                SiparisNo = x.SiparisNo,
                AdSoyad = x.AdSoyad,
                BankaAdi = x.BankaAdi,
                Tutar = x.Tutar,
                OdemeTarihi = x.OdemeTarihi,
                OlusturulmaTarihi = x.OlusturulmaTarihi,
                OkunduMu = x.OkunduMu
            }).ToList();
        }

        public async Task<OdemeBildirimiDetayDto?> BildirimOkuAsync(Guid id)
        {
            var entity = await _depo.IdIleGetirAsync(id);
            if (entity == null) return null;

            if (!entity.OkunduMu)
            {
                entity.OkunduMu = true;
                _depo.Guncelle(entity);
                await _depo.DegisiklikleriKaydetAsync();
            }

            return new OdemeBildirimiDetayDto
            {
                Id = entity.Id,
                SiparisNo = entity.SiparisNo,
                AdSoyad = entity.AdSoyad,
                Telefon = entity.Telefon,
                Email = entity.Email,
                BankaAdi = entity.BankaAdi,
                Tutar = entity.Tutar,
                OdemeTarihi = entity.OdemeTarihi,
                DekontYolu = entity.DekontYolu,
                Aciklama = entity.Aciklama,
                OlusturulmaTarihi = entity.OlusturulmaTarihi,
                OkunduMu = entity.OkunduMu
            };
        }

        public async Task<Guid> EkleAsync(OdemeBildirimiEkleDto dto, string webRootPath)
        {
            string? dekontYolu = null;

            if (dto.Dekont != null && dto.Dekont.Length > 0)
            {
                var uploadsFolder = Path.Combine(webRootPath, "uploads", "dekontlar");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Dekont.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Dekont.CopyToAsync(stream);
                }

                dekontYolu = "/uploads/dekontlar/" + uniqueFileName;
            }

            // Tarih dönüşüm güvenliği
            DateTime gecerliTarih = DateTime.Now;
            if (!string.IsNullOrEmpty(dto.OdemeTarihi) && DateTime.TryParse(dto.OdemeTarihi, out var parsedDate))
            {
                gecerliTarih = parsedDate;
            }

            var entity = new OdemeBildirimi
            {
                SiparisNo = dto.SiparisNo,
                AdSoyad = dto.AdSoyad,
                Telefon = dto.Telefon,
                Email = dto.Email,
                BankaAdi = dto.BankaAdi,
                Tutar = dto.Tutar,
                OdemeTarihi = gecerliTarih, // Güvenli dönüştürülen tarih atanıyor
                DekontYolu = dekontYolu,
                Aciklama = dto.Aciklama,
                OkunduMu = false
            };

            await _depo.EkleAsync(entity);
            await _depo.DegisiklikleriKaydetAsync();
            return entity.Id;
        }

        public async Task SilAsync(Guid id)
        {
            var entity = await _depo.IdIleGetirAsync(id);
            if (entity != null)
            {
                _depo.Sil(entity);
                await _depo.DegisiklikleriKaydetAsync();
            }
        }

        public async Task<int> OkunmamisBildirimSayisiGetirAsync()
        {
            var liste = await _depo.GetirAsync(x => !x.OkunduMu);
            return liste.Count;
        }
    }
}