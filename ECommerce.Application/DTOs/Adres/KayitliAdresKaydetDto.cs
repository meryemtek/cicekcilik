using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Adres;

public class KayitliAdresKaydetDto
{
    [Required(ErrorMessage = "Adres başlığı gereklidir.")]
    [StringLength(60)]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Alıcı adı ve soyadı gereklidir.")]
    [StringLength(120)]
    public string AliciAdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon gereklidir.")]
    [StringLength(25)]
    public string Telefon { get; set; } = string.Empty;

    [Required(ErrorMessage = "İl gereklidir.")]
    [StringLength(80)]
    public string Il { get; set; } = string.Empty;

    [Required(ErrorMessage = "İlçe gereklidir.")]
    [StringLength(80)]
    public string Ilce { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açık adres gereklidir.")]
    [StringLength(500)]
    public string AcikAdres { get; set; } = string.Empty;

    public bool VarsayilanMi { get; set; }
}
