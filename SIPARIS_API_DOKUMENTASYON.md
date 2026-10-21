# Sipariş Oluşturma API Dokumentasyonu

**Endpoint:** `POST /api/siparis/olustur`

---

## 📋 İstek Gövdesi (SiparisOlusturDto)

```json
{
  "sepetId": "string (zorunlu)",
  "musteriAd": "string (zorunlu)",
  "musteriSoyad": "string (zorunlu)",
  "musteriEmail": "string (zorunlu)",
  "musteriTelefon": "string (zorunlu)",
  "faturaAdresi": "string (zorunlu)",
  "faturaTipi": "string (opsiyonel, varsayılan: 'Bireysel')",
  "firmaAdi": "string (sadece faturaTipi = 'Kurumsal' ise)",
  "vergiNo": "string (sadece faturaTipi = 'Kurumsal' ise)",
  "kullaniciId": "guid (otomatik - token'dan alınır)",
  "aliciAdSoyad": "string (zorunlu)",
  "aliciTelefon": "string (zorunlu)",
  "gonderimYeriAdi": "string (zorunlu)",
  "teslimatAdresi": "string (zorunlu)",
  "kartNotu": "string (opsiyonel - çiçek kart notu)",
  "gonderenGorunenAd": "string (opsiyonel - gönderen adı)",
  "odemeYontemi": "string (opsiyonel, varsayılan: 'KrediKarti')"
}
```

---

## 🔑 Zorunlu Alanlar

| Alan | Tip | Açıklama |
|------|-----|----------|
| **sepetId** | string | Kullanıcının sepet ID'si (cache/session'dan) |
| **musteriAd** | string | Müşterinin adı |
| **musteriSoyad** | string | Müşterinin soyadı |
| **musteriEmail** | string | Müşterinin e-posta adresi |
| **musteriTelefon** | string | Müşteri telefon numarası (SMS gönderimi için kullanılır) |
| **faturaAdresi** | string | Fatura gönderilecek adres |
| **aliciAdSoyad** | string | Ürünü alacak kişinin adı-soyadı |
| **aliciTelefon** | string | Ürünü alacak kişinin telefon numarası |
| **gonderimYeriAdi** | string | Gönderim yerinin adı (örn: "Ofis", "Ev") |
| **teslimatAdresi** | string | Teslimat yapılacak tam adres |

---

## ⚙️ Opsiyonel Alanlar

| Alan | Varsayılan | Mümkün Değerler | Açıklama |
|------|-----------|-----------------|----------|
| **faturaTipi** | `"Bireysel"` | `"Bireysel"` ✅ <br> `"Kurumsal"` ✅ | Fatura tipi seçimi |
| **odemeYontemi** | `"KrediKarti"` | `"KrediKarti"` ✅ <br> `"BankaTransferi"` ✅ <br> `"Nakit"` ✅ <br> `"Eft"` ✅ | Ödeme yöntemi |
| **kartNotu** | `null` | Herhangi string | Çiçekle birlikte gönderdilecek not |
| **gonderenGorunenAd** | `null` | Herhangi string | Kartın üzerinde çıkacak gönderen adı |
| **firmaAdi** | `null` | Herhangi string | **Kurumsal fatura seçince gerekli** |
| **vergiNo** | `null` | Herhangi string | **Kurumsal fatura seçince gerekli** |

---

## 👤 KullaniciId Nasıl Alınıyor?

**Otomatik olarak JWT Token'dan alınıyor:**

```csharp
var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var kullaniciId))
{
    dto.KullaniciId = kullaniciId;
}
else
{
    dto.KullaniciId = null; // Misafir siparişi
}
```

**Kurallar:**
- ✅ **Giriş yapan kullanıcı:** Token'dan `ClaimTypes.NameIdentifier` çıkarılır ve otomatik set edilir
- ✅ **Giriş yapmayan kullanıcı (Misafir):** `KullaniciId = null` kalır, sipariş yine oluşturulur
- ⚠️ **Request'te KullaniciId gönderirsen:** Token'daki değer tarafından **ezilir**

---

## 📤 Örnek İstek (cURL)

### 1. Giriş Yapan Kullanıcı

```bash
curl -X POST https://localhost:7192/api/siparis/olustur \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '{
    "sepetId": "sepet-123",
    "musteriAd": "Ayşe",
    "musteriSoyad": "Yılmaz",
    "musteriEmail": "ayse@example.com",
    "musteriTelefon": "05551234567",
    "faturaAdresi": "Elazığ, Merkez",
    "faturaTipi": "Bireysel",
    "aliciAdSoyad": "Fatih Yılmaz",
    "aliciTelefon": "05559876543",
    "gonderimYeriAdi": "Ofis",
    "teslimatAdresi": "Elazığ, Fırın Caddesi No:45",
    "kartNotu": "Seninle yaşamak harika!",
    "gonderenGorunenAd": "Ayşe",
    "odemeYontemi": "KrediKarti"
  }'
```

### 2. Misafir Sipariş (Giriş Olmadan)

```bash
curl -X POST https://localhost:7192/api/siparis/olustur \
  -H "Content-Type: application/json" \
  -d '{
    "sepetId": "sepet-456",
    "musteriAd": "Mehmet",
    "musteriSoyad": "Kaya",
    "musteriEmail": "mehmet@example.com",
    "musteriTelefon": "05551111111",
    "faturaAdresi": "İstanbul",
    "faturaTipi": "Kurumsal",
    "firmaAdi": "ABC Şirketi",
    "vergiNo": "1234567890",
    "aliciAdSoyad": "Can Kaya",
    "aliciTelefon": "05552222222",
    "gonderimYeriAdi": "Fabrika",
    "teslimatAdresi": "İstanbul, Sanayi Bölgesi",
    "odemeYontemi": "BankaTransferi"
  }'
```

### 3. Kurumsal Fatura ile Sipariş

```bash
curl -X POST https://localhost:7192/api/siparis/olustur \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '{
    "sepetId": "sepet-789",
    "musteriAd": "Zeynep",
    "musteriSoyad": "Demir",
    "musteriEmail": "zeynep@kurumsal.com",
    "musteriTelefon": "05553333333",
    "faturaAdresi": "Ankara, İkinci Cadde",
    "faturaTipi": "Kurumsal",
    "firmaAdi": "XYZ Ltd. Şti.",
    "vergiNo": "9876543210",
    "aliciAdSoyad": "Eşli Demir",
    "aliciTelefon": "05554444444",
    "gonderimYeriAdi": "Genel Müdürlük",
    "teslimatAdresi": "Ankara, Çankaya Mahallesi",
    "kartNotu": "Şirkete tebrikler!",
    "gonderenGorunenAd": "Zeynep Demir",
    "odemeYontemi": "Eft"
  }'
```

---

## ✅ Başarılı Yanıt (200 OK)

```json
{
  "siparisId": "a1b2c3d4-e5f6-4g7h-8i9j-0k1l2m3n4o5p",
  "mesaj": "Sipariş başarıyla oluşturuldu."
}
```

---

## ❌ Hata Yanıtları

### 1. Boş Sepet

```json
{
  "mesaj": "Sepetiniz boş."
}
```

### 2. Geçersiz Sepet ID

```json
{
  "mesaj": "Sepet bulunamadı."
}
```

### 3. Eksik Zorunlu Alan

```json
{
  "mesaj": "MusteriAd alanı boş olamaz."
}
```

---

## 🔐 FaturaTipi Değerleri

### Bireysel Fatura
```json
{
  "faturaTipi": "Bireysel",
  "firmaAdi": null,
  "vergiNo": null
}
```
- **Kullanılan alanlar:** Müşteri adı-soyadı
- **Kurumsal bilgiler:** İsteğe bağlı

### Kurumsal Fatura
```json
{
  "faturaTipi": "Kurumsal",
  "firmaAdi": "ABC Şirketi",
  "vergiNo": "1234567890"
}
```
- **Kurumsal bilgiler:** **ZORUNLU**
- **Müşteri bilgileri:** Yine de gerekli

---

## 💳 OdemeYontemi Değerleri

| Değer | Açıklama | API Entegrasyonu |
|-------|----------|------------------|
| `"KrediKarti"` | Kredi/Debit kartıyla ödeme | Paytr entegrasyonu var |
| `"BankaTransferi"` | Banka hesabına transfer | Manual kontrol gerekli |
| `"Nakit"` | Teslimat sırasında nakit | Manual kontrol gerekli |
| `"Eft"` | EFT/Havale yoluyla ödeme | Manual kontrol gerekli |

**Not:** `OdemeYontemi` sadece sipariş kaydı için saklanır, otomatik ödeme işlemi başlatmaz. Ödeme entegrasyonu ayrı endpoint'lerde yapılır (örn: Paytr).

---

## 📊 Sipariş Durumları (SiparisDurumu)

Sipariş oluşturulduğunda otomatik olarak `Beklemede` durumuna geçer:

| Durum | Değer | Açıklama |
|-------|-------|----------|
| **Beklemede** | 0 | Sipariş yeni oluşturuldu, ödeme bekleniyor |
| **İşlemde** | 1 | Ödeme onaylandı, çiçek hazırlanıyor |
| **Kargoda** | 2 | Çiçek kargo/kurya'ya verildi |
| **TeslimEdildi** | 3 | Alıcıya teslim edildi |
| **İptalEdildi** | 4 | Sipariş iptal edildi |

---

## 🔄 API Akışı

```
1. Müşteri sepetin içini doldurur → SepetId elde edilir
2. Müşteri sipariş bilgilerini gönderir → POST /api/siparis/olustur
3. API:
   - Token'dan KullaniciId çıkarır (varsa)
   - Sepeti getirir ve kontrol eder
   - Tutarları hesaplar
   - Sipariş kaydını oluşturur (Durum = Beklemede)
   - SMS gönderir
   - Hi-Cari entegrasyonunu tetikler
   - Sepeti temizler
4. SiparisId döndürülür
5. Müşteri ödeme sayfasına yönlendirilir
```

---

## 📝 Önemli Notlar

1. **SepetId alanı var mı kontrol et:** Sepet service'ine göre değişebilir
2. **Telefon numarası:** SMS göndermesi için gerekli, hatalı numaraya gönderim yapılmaz
3. **Kurumsal fatura:** `firmaAdi` ve `vergiNo` boş bırakılırsa sipariş yine oluşur ama fatura eksik kalır
4. **KartNotu:** Maksimum 500 karakter önerilir
5. **Teslimat adresi:** En ayrıntılı şekilde yazılmalı (İl, İlçe, Mahalle, Sokak, No)

---

## 🧪 Test Değerleri

```json
{
  "sepetId": "test-sepet-001",
  "musteriAd": "Test",
  "musteriSoyad": "Kullanıcı",
  "musteriEmail": "test@ecommerce.com",
  "musteriTelefon": "05551234567",
  "faturaAdresi": "Test Sokak No:1",
  "faturaTipi": "Bireysel",
  "aliciAdSoyad": "Test Alıcı",
  "aliciTelefon": "05551111111",
  "gonderimYeriAdi": "Test Yeri",
  "teslimatAdresi": "Elazığ, Test Caddesi",
  "kartNotu": "Test notu",
  "odemeYontemi": "KrediKarti"
}
```

Herhangi sorun varsa sor! 🌸
