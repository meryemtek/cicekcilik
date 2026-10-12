using ECommerce.Application.DTOs.Sepet;

public class SepetDto
{
    public List<SepetKalemDto> Kalemler { get; set; } = new List<SepetKalemDto>();
    public decimal ToplamTutar => Kalemler.Sum(k => k.ToplamFiyat);
}