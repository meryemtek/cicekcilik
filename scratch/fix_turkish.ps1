$connectionString = "Server=(localdb)\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

$cmd = $connection.CreateCommand()
$cmd.CommandText = @"
UPDATE Urunler 
SET Ad = @Ad, 
    Aciklama = @Aciklama, 
    AiAciklama = @AiAciklama, 
    Etiket = @Etiket
WHERE SeoUrl = 'boutique-rosa-sakayik-senfonisi'
"@

$cmd.Parameters.Add("@Ad", [System.Data.SqlDbType]::NVarChar).Value = "Boutique Rosa & Şakayık Senfonisi"
$cmd.Parameters.Add("@Aciklama", [System.Data.SqlDbType]::NVarChar).Value = "Fransız bahçe gülleri, pudra pembe şakayıklar, taze okaliptüs yaprakları ve atölyemize özel ipek ambalaj tanzimi ile zarafetin somut hali."
$cmd.Parameters.Add("@AiAciklama", [System.Data.SqlDbType]::NVarChar).Value = "Bu aranjman, pastel pudra tonlarının dinginliğini okaliptüs yapraklarının taze yeşiliyle buluşturarak romantik ve sofistike bir atmosfer yaratmak üzere tasarlanmıştır."
$cmd.Parameters.Add("@Etiket", [System.Data.SqlDbType]::NVarChar).Value = "Pembe Bahçe Gülleri & Şakayık Buketi"

$rows = $cmd.ExecuteNonQuery()
Write-Host "Guncellenen satir sayisi: $rows"

# Kontrol
$checkCmd = $connection.CreateCommand()
$checkCmd.CommandText = "SELECT Ad, Aciklama, Etiket FROM Urunler WHERE SeoUrl = 'boutique-rosa-sakayik-senfonisi'"
$r = $checkCmd.ExecuteReader()
while ($r.Read()) {
    Write-Host "Yeni Ad: $($r['Ad'])"
    Write-Host "Yeni Aciklama: $($r['Aciklama'])"
    Write-Host "Yeni Etiket: $($r['Etiket'])"
}

$connection.Close()
