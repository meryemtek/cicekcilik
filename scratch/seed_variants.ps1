$connectionString = "Server=(localdb)\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

$urunCmd = $connection.CreateCommand()
$urunCmd.CommandText = "SELECT Id FROM Urunler WHERE SeoUrl = 'boutique-rosa-sakayik-senfonisi'"
$urunId = [guid]$urunCmd.ExecuteScalar()

$now = [DateTime]::UtcNow
$varyantlar = @(
    @{ Ad = "Klasik"; Fark = [decimal]0.00; Stok = 15 },
    @{ Ad = "Deluxe"; Fark = [decimal]650.00; Stok = 10 },
    @{ Ad = "Grandiflora"; Fark = [decimal]1500.00; Stok = 5 }
)

foreach ($v in $varyantlar) {
    $vCmd = $connection.CreateCommand()
    $vCmd.CommandText = @"
INSERT INTO UrunVaryantlari (Id, UrunId, VaryantAdi, FiyatFarki, StokMiktari, AktifMi, OlusturulmaTarihi)
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

Write-Host "UrunVaryantlari basariyla eklendi!"
$connection.Close()
