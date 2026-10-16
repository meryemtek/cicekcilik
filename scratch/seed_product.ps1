$connectionString = "Server=(localdb)\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

# Önce mevcut mu kontrol et
$checkCmd = $connection.CreateCommand()
$checkCmd.CommandText = "SELECT COUNT(*) FROM Urunler WHERE SeoUrl = 'boutique-rosa-sakayik-senfonisi'"
$exists = [int]$checkCmd.ExecuteScalar()

if ($exists -eq 0) {
    $urunId = [guid]::NewGuid()
    $katId = [guid]'22d303f9-d3e5-4b19-9088-13a9d91392f9'
    $now = [DateTime]::UtcNow

    $insertCmd = $connection.CreateCommand()
    $insertCmd.CommandText = @"
INSERT INTO Urunler (Id, Ad, UrunKodu, SeoUrl, Aciklama, AiAciklama, Fiyat, StokMiktari, GorselUrl, AktifMi, Etiket, KategoriId, OlusturulmaTarihi)
VALUES (@Id, @Ad, @UrunKodu, @SeoUrl, @Aciklama, @AiAciklama, @Fiyat, @StokMiktari, @GorselUrl, @AktifMi, @Etiket, @KategoriId, @OlusturulmaTarihi)
"@
    $insertCmd.Parameters.AddWithValue("@Id", $urunId) | Out-Null
    $insertCmd.Parameters.AddWithValue("@Ad", "Boutique Rosa & Şakayık Senfonisi") | Out-Null
    $insertCmd.Parameters.AddWithValue("@UrunKodu", "HT-ROSA04") | Out-Null
    $insertCmd.Parameters.AddWithValue("@SeoUrl", "boutique-rosa-sakayik-senfonisi") | Out-Null
    $insertCmd.Parameters.AddWithValue("@Aciklama", "Fransız bahçe gülleri, pudra pembe şakayıklar, taze okaliptüs yaprakları ve atölyemize özel ipek ambalaj tanzimi ile zarafetin somut hali.") | Out-Null
    $insertCmd.Parameters.AddWithValue("@AiAciklama", "Bu aranjman, pastel pudra tonlarının dinginliğini okaliptüs yapraklarının taze yeşiliyle buluşturarak romantik ve sofistike bir atmosfer yaratmak üzere tasarlanmıştır.") | Out-Null
    $insertCmd.Parameters.AddWithValue("@Fiyat", [decimal]1450.00) | Out-Null
    $insertCmd.Parameters.AddWithValue("@StokMiktari", 25) | Out-Null
    $insertCmd.Parameters.AddWithValue("@GorselUrl", "https://images.unsplash.com/photo-1561181286-d3fee7d55364?auto=format&fit=crop&w=800&q=80") | Out-Null
    $insertCmd.Parameters.AddWithValue("@AktifMi", $true) | Out-Null
    $insertCmd.Parameters.AddWithValue("@Etiket", "Boutique Rosa and Peony Symphony") | Out-Null
    $insertCmd.Parameters.AddWithValue("@KategoriId", $katId) | Out-Null
    $insertCmd.Parameters.AddWithValue("@OlusturulmaTarihi", $now) | Out-Null
    $insertCmd.ExecuteNonQuery() | Out-Null

    # Varyantları ekle
    $varyantlar = @(
        @{ Ad = "Klasik"; Fark = [decimal]0.00; Stok = 15 },
        @{ Ad = "Deluxe"; Fark = [decimal]650.00; Stok = 10 },
        @{ Ad = "Grandiflora"; Fark = [decimal]1500.00; Stok = 5 }
    )

    foreach ($v in $varyantlar) {
        $vCmd = $connection.CreateCommand()
        $vCmd.CommandText = @"
INSERT INTO UrunVaryantlar (Id, UrunId, VaryantAdi, FiyatFarki, StokMiktari, AktifMi, OlusturulmaTarihi)
VALUES (@Id, @UrunId, @VaryantAdi, @FiyatFarki, @StokMiktari, @AktifMi, @OlusturulmaTarihi)
"@
        $vCmd.Parameters.AddWithValue("@Id", [guid]::NewGuid()) | Out-Null
        $vCmd.Parameters.AddWithValue("@UrunId", $urunId) | Out-Null
        $vCmd.Parameters.AddWithValue("@VaryantAdi", $v.Ad) | Out-Null
        $vCmd.Parameters.AddWithValue("@FiyatFarki", $v.Fark) | Out-Null
        $vCmd.Parameters.AddWithValue("@StokMiktari", $v.Stok) | Out-Null
        $vCmd.Parameters.AddWithValue("@AktifMi", $true) | Out-Null
        $vCmd.Parameters.AddWithValue("@OlusturulmaTarihi", $now) | Out-Null
        $vCmd.ExecuteNonQuery() | Out-Null
    }

    Write-Host "Urun ve varyantlar basariyla eklendi! Id: $urunId"
} else {
    Write-Host "Urun zaten mevcut."
}

$connection.Close()
