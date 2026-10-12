using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Slider;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Services
{
    public class SliderServisi : ISliderServisi
    {
        private readonly IDepo<Slider> _sliderDeposu;

        public SliderServisi(IDepo<Slider> sliderDeposu)
        {
            _sliderDeposu = sliderDeposu;
        }

        public async Task<IReadOnlyList<SliderListeDto>> AktifSliderlariGetirAsync()
        {
            var sliderlar = await _sliderDeposu.GetirAsync(s => s.AktifMi);

            return sliderlar.Select(s => new SliderListeDto
            {
                Id = s.Id,
                Baslik = s.Baslik,
                AltMetin = s.AltMetin,
                ResimUrl = s.ResimUrl,
                LinkUrl = s.LinkUrl,
                ButonMetni = s.ButonMetni,
                SiraNo = s.SiraNo,
                AktifMi = s.AktifMi
            }).OrderBy(s => s.SiraNo).ToList();
        }

        public async Task<IReadOnlyList<SliderListeDto>> TumSliderlariGetirAsync()
        {
            var sliderlar = await _sliderDeposu.TumunuGetirAsync();

            return sliderlar.Select(s => new SliderListeDto
            {
                Id = s.Id,
                Baslik = s.Baslik,
                AltMetin = s.AltMetin,
                ResimUrl = s.ResimUrl,
                LinkUrl = s.LinkUrl,
                ButonMetni = s.ButonMetni,
                SiraNo = s.SiraNo,
                AktifMi = s.AktifMi
            }).OrderBy(s => s.SiraNo).ToList();
        }

        public async Task<SliderGuncelleDto?> IdIleGetirAsync(Guid id)
        {
            var slider = await _sliderDeposu.IdIleGetirAsync(id);
            if (slider == null) return null;

            return new SliderGuncelleDto
            {
                Id = slider.Id,
                Baslik = slider.Baslik,
                AltMetin = slider.AltMetin,
                ResimUrl = slider.ResimUrl,
                LinkUrl = slider.LinkUrl,
                ButonMetni = slider.ButonMetni,
                SiraNo = slider.SiraNo,
                AktifMi = slider.AktifMi
            };
        }

        public async Task<Guid> SliderEkleAsync(SliderEkleDto dto)
        {
            var yeniSlider = new Slider
            {
                Baslik = dto.Baslik,
                AltMetin = dto.AltMetin,
                ResimUrl = dto.ResimUrl,
                LinkUrl = dto.LinkUrl,
                ButonMetni = dto.ButonMetni,
                SiraNo = dto.SiraNo,
                AktifMi = dto.AktifMi
            };

            await _sliderDeposu.EkleAsync(yeniSlider);
            await _sliderDeposu.DegisiklikleriKaydetAsync();

            return yeniSlider.Id;
        }

        public async Task SliderGuncelleAsync(SliderGuncelleDto dto)
        {
            var slider = await _sliderDeposu.IdIleGetirAsync(dto.Id);
            if (slider != null)
            {
                slider.Baslik = dto.Baslik;
                slider.AltMetin = dto.AltMetin;
                slider.ResimUrl = dto.ResimUrl;
                slider.LinkUrl = dto.LinkUrl;
                slider.ButonMetni = dto.ButonMetni;
                slider.SiraNo = dto.SiraNo;
                slider.AktifMi = dto.AktifMi;

                // TemelVarlik sınıfından gelen özellik (Kayıtlı saat farkını önlemek için UtcNow kullanıldı)
                slider.GuncellenmeTarihi = DateTime.UtcNow;

                _sliderDeposu.Guncelle(slider);
                await _sliderDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task SliderSilAsync(Guid id)
        {
            var slider = await _sliderDeposu.IdIleGetirAsync(id);
            if (slider != null)
            {
                _sliderDeposu.Sil(slider);
                await _sliderDeposu.DegisiklikleriKaydetAsync();
            }
        }
    }
}