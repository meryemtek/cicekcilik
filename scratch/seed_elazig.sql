-- Eski kayıt varsa temizle
DELETE FROM SemtTeslimatBolgeleri WHERE IlAdi <> 'Elazığ';

-- Elazığ mahallelerini ekle
IF NOT EXISTS (SELECT 1 FROM SemtTeslimatBolgeleri WHERE IlAdi = 'Elazığ')
BEGIN
    INSERT INTO SemtTeslimatBolgeleri (Id, IlAdi, IlceAdi, MahalleAdi, EkTeslimatUcreti, AktifMi, OlusturulmaTarihi) VALUES
    (NEWID(), N'Elazığ', N'Merkez', N'Ataşehir', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Çaydaçıra', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Sürsürü', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Abdullahpaşa', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Cumhuriyet', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Doğukent', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Bahçelievler', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Üniversite', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'İzzetpaşa', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Fırat', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Rızaiye', 0.00, 1, GETUTCDATE()),
    (NEWID(), N'Elazığ', N'Merkez', N'Yenimahalle', 0.00, 1, GETUTCDATE());
END
