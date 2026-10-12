using ECommerce.Application.Interfaces;
using ExcelDataReader;
using System.Text;

namespace ECommerce.Application.Services;

public class AdresHavuzuServisi : IAdresHavuzuServisi
{
    // İl
    //   -> İlçe
    //       -> Mahalleler
    private readonly Dictionary<string, Dictionary<string, List<string>>> _adresAgaci =
        new(StringComparer.OrdinalIgnoreCase);

    private bool _yuklendiMi = false;

    public void AdresleriYukle(string dosyaYolu)
    {
        if (_yuklendiMi)
            return;

        if (!File.Exists(dosyaYolu))
            throw new FileNotFoundException(
                $"Adres Excel dosyası bulunamadı: {dosyaYolu}");

        Encoding.RegisterProvider(
            CodePagesEncodingProvider.Instance);

        using var stream = File.Open(
            dosyaYolu,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);

        using var reader = ExcelReaderFactory.CreateReader(stream);

        // İlk satır başlık
        if (!reader.Read())
            throw new InvalidOperationException(
                "Excel dosyası boş.");

        while (reader.Read())
        {
            var il = reader.GetValue(0)?.ToString()?.Trim();
            var ilce = reader.GetValue(1)?.ToString()?.Trim();
            var mahalle = reader.GetValue(2)?.ToString()?.Trim();

            // Eksik satırları geç
            if (string.IsNullOrWhiteSpace(il) ||
                string.IsNullOrWhiteSpace(ilce) ||
                string.IsNullOrWhiteSpace(mahalle))
            {
                continue;
            }

            // İl yoksa oluştur
            if (!_adresAgaci.TryGetValue(il, out var ilceler))
            {
                ilceler = new Dictionary<string, List<string>>(
                    StringComparer.OrdinalIgnoreCase);

                _adresAgaci[il] = ilceler;
            }

            // İlçe yoksa oluştur
            if (!ilceler.TryGetValue(ilce, out var mahalleler))
            {
                mahalleler = new List<string>();
                ilceler[ilce] = mahalleler;
            }

            // Mahalle yoksa ekle
            if (!mahalleler.Contains(
                    mahalle,
                    StringComparer.OrdinalIgnoreCase))
            {
                mahalleler.Add(mahalle);
            }
        }

        // Mahalleleri alfabetik sırala
        foreach (var il in _adresAgaci.Keys.ToList())
        {
            foreach (var ilce in _adresAgaci[il].Keys.ToList())
            {
                _adresAgaci[il][ilce] =
                    _adresAgaci[il][ilce]
                        .OrderBy(x => x)
                        .ToList();
            }
        }

        _yuklendiMi = true;
    }

    public List<string> IlleriGetir()
    {
        return _adresAgaci
            .Keys
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> IlceleriGetir(string ilAdi)
    {
        if (string.IsNullOrWhiteSpace(ilAdi))
            return new List<string>();

        if (!_adresAgaci.TryGetValue(ilAdi, out var ilceler))
            return new List<string>();

        return ilceler
            .Keys
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> MahalleleriGetir(
        string ilAdi,
        string ilceAdi)
    {
        if (string.IsNullOrWhiteSpace(ilAdi) ||
            string.IsNullOrWhiteSpace(ilceAdi))
        {
            return new List<string>();
        }

        if (!_adresAgaci.TryGetValue(
                ilAdi,
                out var ilceler))
        {
            return new List<string>();
        }

        if (!ilceler.TryGetValue(
                ilceAdi,
                out var mahalleler))
        {
            return new List<string>();
        }

        return mahalleler.ToList();
    }
}