UPDATE Urunler 
SET Ad = N'Boutique Rosa & Şakayık Senfonisi', 
    Aciklama = N'Fransız bahçe gülleri, pudra pembe şakayıklar, taze okaliptüs yaprakları ve atölyemize özel ipek ambalaj tanzimi ile zarafetin somut hali.', 
    AiAciklama = N'Bu aranjman, pastel pudra tonlarının dinginliğini okaliptüs yapraklarının taze yeşiliyle buluşturarak romantik ve sofistike bir atmosfer yaratmak üzere tasarlanmıştır.', 
    Etiket = N'Pembe Bahçe Gülleri & Şakayık Buketi'
WHERE SeoUrl = 'boutique-rosa-sakayik-senfonisi';

UPDATE UrunVaryantlari
SET VaryantAdi = N'Klasik (15 Dal)'
WHERE VaryantAdi = 'Klasik';

UPDATE UrunVaryantlari
SET VaryantAdi = N'Deluxe (25 Dal)'
WHERE VaryantAdi = 'Deluxe';

UPDATE UrunVaryantlari
SET VaryantAdi = N'Grandiflora (40 Dal)'
WHERE VaryantAdi = 'Grandiflora';
