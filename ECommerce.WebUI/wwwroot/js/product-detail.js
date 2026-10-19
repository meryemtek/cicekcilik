/* ========================================================
   GALERİ & SLIDER YÖNETİMİ
   ======================================================== */
let galeriGorselleri = [];
let aktifGorselIndex = 0;
let galeriOtoGecisZamanlayici = null;

document.addEventListener("DOMContentLoaded", function () {
    galeriyiBaslat();
    doldurIl();
    saatleriKontrolEt();
    saatDegisti();
    fiyatGuncelle();
});

function galeriyiBaslat() {
    const thumbElements = document.querySelectorAll('.fl-thumb-item');
    galeriGorselleri = [];

    thumbElements.forEach((thumb, idx) => {
        const img = thumb.querySelector('img');
        const tag = thumb.querySelector('.fl-thumb-tag');
        const src = img ? (img.getAttribute('src') || '') : '';
        const baslik = tag ? tag.innerText.trim() : `Fotoğraf ${idx + 1}`;

        galeriGorselleri.push({
            index: idx,
            url: src,
            baslik: baslik
        });
    });

    if (galeriGorselleri.length === 0) {
        const anaImg = document.getElementById('anaUrunGorseli');
        if (anaImg) {
            galeriGorselleri.push({
                index: 0,
                url: anaImg.src,
                baslik: 'Buket Görünümü'
            });
        }
    }

    // Otomatik ilerlemeyi başlat
    otoGecisBaslat();

    // Fare ana galeri kutusunun üzerindeyken otomatik ilerlemeyi duraklat
    const galleryBox = document.querySelector('.fl-gallery-box');
    if (galleryBox) {
        galleryBox.addEventListener('mouseenter', () => clearInterval(galeriOtoGecisZamanlayici));
        galleryBox.addEventListener('mouseleave', () => otoGecisBaslat());
    }

    // Klavye yön tuşları ile lightbox içinde gezinti
    document.addEventListener('keydown', function (e) {
        const lightboxModal = document.getElementById('gorselLightboxModal');
        if (lightboxModal && lightboxModal.classList.contains('show')) {
            if (e.key === 'ArrowRight') {
                sonrakiGorsel(e);
            } else if (e.key === 'ArrowLeft') {
                oncekiGorsel(e);
            }
        }
    });
}

function otoGecisBaslat() {
    clearInterval(galeriOtoGecisZamanlayici);
    galeriOtoGecisZamanlayici = setInterval(() => {
        if (galeriGorselleri.length > 1) {
            // Lightbox açık değilse otomatik ilerlet
            const lightbox = document.getElementById('gorselLightboxModal');
            if (!lightbox || !lightbox.classList.contains('show')) {
                gorselGit(aktifGorselIndex + 1);
            }
        }
    }, 4500);
}

function gorselGit(hedefIndex) {
    if (!galeriGorselleri || galeriGorselleri.length === 0) return;

    aktifGorselIndex = (hedefIndex + galeriGorselleri.length) % galeriGorselleri.length;
    const secilen = galeriGorselleri[aktifGorselIndex];

    const anaGorsel = document.getElementById('anaUrunGorseli');
    const lightboxGorsel = document.getElementById('lightboxGorsel');
    const lightboxBaslik = document.getElementById('lightboxBaslik');

    // Yumuşak fade animasyonu
    if (anaGorsel) {
        anaGorsel.style.opacity = '0.3';
        setTimeout(() => {
            anaGorsel.src = secilen.url;
            anaGorsel.style.opacity = '1';
        }, 150);
    }

    if (lightboxGorsel) {
        lightboxGorsel.style.opacity = '0.3';
        setTimeout(() => {
            lightboxGorsel.src = secilen.url;
            lightboxGorsel.style.opacity = '1';
        }, 120);
    }

    if (lightboxBaslik) {
        lightboxBaslik.innerText = `${secilen.baslik} (${aktifGorselIndex + 1} / ${galeriGorselleri.length})`;
    }

    // Alt thumbnail aktifliğini güncelle
    const thumbElements = document.querySelectorAll('.fl-thumb-item');
    thumbElements.forEach((thumb, idx) => {
        if (idx === aktifGorselIndex) {
            thumb.classList.add('active');
        } else {
            thumb.classList.remove('active');
        }
    });

    // Alttaki modern nokta göstergesini (capsule dots) güncelle
    const dots = document.querySelectorAll('.fl-indicator-dot');
    dots.forEach((dot, idx) => {
        if (idx === aktifGorselIndex) {
            dot.classList.add('active');
        } else {
            dot.classList.remove('active');
        }
    });
}

function gorselSegmentTikla(index, e) {
    if (e) {
        e.preventDefault();
        e.stopPropagation();
    }
    otoGecisBaslat();
    gorselGit(index);
}

function sonrakiGorsel(e) {
    if (e) {
        e.preventDefault();
        e.stopPropagation();
    }
    otoGecisBaslat(); // Zamanlayıcıyı sıfırla
    gorselGit(aktifGorselIndex + 1);
}

function oncekiGorsel(e) {
    if (e) {
        e.preventDefault();
        e.stopPropagation();
    }
    otoGecisBaslat(); // Zamanlayıcıyı sıfırla
    gorselGit(aktifGorselIndex - 1);
}

function anaResmiDegistir(el, url) {
    const thumbElements = Array.from(document.querySelectorAll('.fl-thumb-item'));
    const clickedIndex = thumbElements.indexOf(el);
    if (clickedIndex !== -1) {
        otoGecisBaslat();
        gorselGit(clickedIndex);
    }
}

/* Fotoğrafa tıklandığında büyütme (Lightbox Modal) */
function resmiBuyut() {
    const modalEl = document.getElementById('gorselLightboxModal');
    if (modalEl) {
        const secilen = galeriGorselleri[aktifGorselIndex];
        const lightboxGorsel = document.getElementById('lightboxGorsel');
        const lightboxBaslik = document.getElementById('lightboxBaslik');

        if (lightboxGorsel && secilen) {
            lightboxGorsel.src = secilen.url;
        }
        if (lightboxBaslik && secilen) {
            lightboxBaslik.innerText = `${secilen.baslik} (${aktifGorselIndex + 1} / ${galeriGorselleri.length})`;
        }

        const modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
        modal.show();
    }
}

function urunPaylas() {
    if (navigator.share) {
        navigator.share({
            title: baseUrunAd,
            url: window.location.href
        }).catch(() => {});
    } else {
        navigator.clipboard.writeText(window.location.href).then(() => {
            alert("Ürün bağlantısı panoya kopyalandı!");
        });
    }
}

function boyutRehberiGoster() {
    var modalEl = document.getElementById('boyutRehberiModal');
    if (modalEl) {
        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
        modal.show();
    }
}

/* ========================================================
   1. BOYUT TERCİHİ SEÇİMİ
   ======================================================== */
function boyutSec(el, varyantId, fark, boyutAdi) {
    document.querySelectorAll('.fl-size-card').forEach(c => c.classList.remove('active'));
    el.classList.add('active');

    if (document.getElementById('gizliVaryantId')) {
        document.getElementById('gizliVaryantId').value = varyantId || '';
    }
    if (document.getElementById('gizliBoyutAdi')) {
        document.getElementById('gizliBoyutAdi').value = boyutAdi || 'Klasik';
    }
    if (document.getElementById('gizliBoyutFark')) {
        document.getElementById('gizliBoyutFark').value = fark || 0;
    }

    fiyatGuncelle();
}

function fiyatGuncelle() {
    const fiyatDisplay = document.getElementById('urunFiyatDisplay');
    if (!fiyatDisplay) return;
    const tabanFiyat = parseFloat(fiyatDisplay.getAttribute('data-base-price')) || 0;
    const boyutFark = parseFloat(document.getElementById('gizliBoyutFark')?.value) || 0;
    const mahalleFark = parseFloat(document.getElementById('gizliMahalleFark')?.value) || 0;
    const vipFark = parseFloat(document.getElementById('gizliVipFark')?.value) || 0;
    const toplam = tabanFiyat + boyutFark + mahalleFark + vipFark;
    fiyatDisplay.innerText = Math.round(toplam).toLocaleString('tr-TR');
}

/* ========================================================
   MAHALLE VE TESLİMAT BÖLGESİ SEÇİMİ
   ======================================================== */
function mahalleSecimiDegisti(selectEl, kapatListe = false) {
    const mahalleAdi = selectEl?.value || '';
    const fark = parseFloat(selectEl?.dataset.fark) || 0;
    const il = selectEl?.dataset.il || 'Elazığ';
    const ilce = selectEl?.dataset.ilce || 'Merkez';

    if (document.getElementById('gizliIl')) {
        document.getElementById('gizliIl').value = il;
    }
    if (document.getElementById('gizliIlce')) {
        document.getElementById('gizliIlce').value = ilce;
    }
    if (document.getElementById('gizliMahalle')) {
        document.getElementById('gizliMahalle').value = mahalleAdi;
    }
    if (document.getElementById('gizliMahalleFark')) {
        document.getElementById('gizliMahalleFark').value = fark;
    }

    document.querySelectorAll('.fl-neighborhood-option').forEach(option => {
        option.classList.toggle('active', option.querySelector('input') === selectEl);
    });

    const siparisVerBtn = document.getElementById('siparisVerBtn');
    if (siparisVerBtn) siparisVerBtn.disabled = !mahalleAdi;

    const whatsappSiparisBtn = document.getElementById('whatsappSiparisBtn');
    if (whatsappSiparisBtn) {
        whatsappSiparisBtn.setAttribute('aria-disabled', String(!mahalleAdi));
        whatsappSiparisBtn.classList.toggle('is-disabled', !mahalleAdi);
    }

    const secilenYeri = document.getElementById('secilenTeslimatYeri');
    if (secilenYeri) {
        secilenYeri.textContent = mahalleAdi
            ? `${il} / ${ilce} · ${mahalleAdi.toLocaleUpperCase('tr-TR')}`
            : 'Mahalle seçin';
    }

    if (kapatListe) teslimatYeriListesiniGoster(false);

    fiyatGuncelle();
}

function teslimatYeriListesiniGoster(acik) {
    const toggle = document.getElementById('teslimatYeriToggle');
    const liste = document.getElementById('mahalleSecimi');
    if (!toggle || !liste) return;

    toggle.setAttribute('aria-expanded', String(acik));
    liste.hidden = !acik;
}

function teslimatBolgeleriniGetir() {
    const kayitliBolgeler = typeof teslimatBolgeleri !== 'undefined' && Array.isArray(teslimatBolgeleri)
        ? teslimatBolgeleri.filter(bolge => bolge && bolge.aktifMi !== false && bolge.mahalleAdi)
        : [];

    return kayitliBolgeler;
}

function teslimatAdresSeciminiBaslat() {
    const mahalleListesi = document.getElementById('mahalleSecimi');
    if (!mahalleListesi) return;

    const toggle = document.getElementById('teslimatYeriToggle');
    if (toggle) {
        toggle.addEventListener('click', () => {
            teslimatYeriListesiniGoster(toggle.getAttribute('aria-expanded') !== 'true');
        });
    }

    const bolgeler = teslimatBolgeleriniGetir();
    mahalleListesi.replaceChildren();
    if (bolgeler.length === 0) {
        const bosMesaji = document.createElement('p');
        bosMesaji.className = 'fl-neighborhood-empty';
        bosMesaji.textContent = 'Kayıtlı teslimat mahallesi bulunmuyor.';
        mahalleListesi.appendChild(bosMesaji);
        mahalleSecimiDegisti(null);
        return;
    }

    const bolgeGruplari = new Map();
    bolgeler
        .sort((a, b) => (a.ilAdi || 'Elazığ').localeCompare(b.ilAdi || 'Elazığ', 'tr')
            || (a.ilceAdi || 'Merkez').localeCompare(b.ilceAdi || 'Merkez', 'tr')
            || a.mahalleAdi.localeCompare(b.mahalleAdi, 'tr'))
        .forEach(bolge => {
            const il = bolge.ilAdi || 'Elazığ';
            const ilce = bolge.ilceAdi || 'Merkez';
            const grupAnahtari = `${il}|${ilce}`;
            if (!bolgeGruplari.has(grupAnahtari)) {
                bolgeGruplari.set(grupAnahtari, { il, ilce, mahalleler: [] });
            }
            bolgeGruplari.get(grupAnahtari).mahalleler.push(bolge);
        });

    bolgeGruplari.forEach(grup => {
        const bolgeBasligi = document.createElement('div');
        bolgeBasligi.className = 'fl-neighborhood-region';
        bolgeBasligi.textContent = `${grup.il} / ${grup.ilce}`;
        mahalleListesi.appendChild(bolgeBasligi);

        grup.mahalleler.forEach(bolge => {
            const secenek = document.createElement('label');
            secenek.className = 'fl-neighborhood-option';

            const input = document.createElement('input');
            input.type = 'radio';
            input.name = 'mahalleSecim';
            input.value = bolge.mahalleAdi;
            input.dataset.il = grup.il;
            input.dataset.ilce = grup.ilce;
            input.dataset.fark = bolge.ekTeslimatUcreti || 0;
            input.checked = false;
            input.addEventListener('change', () => mahalleSecimiDegisti(input, true));

            const ad = document.createElement('span');
            ad.className = 'fl-neighborhood-name';
            ad.textContent = bolge.mahalleAdi.toLocaleUpperCase('tr-TR');

            const ucret = document.createElement('span');
            ucret.className = 'fl-neighborhood-price';
            ucret.textContent = bolge.ekTeslimatUcreti > 0
                ? `+₺${Number(bolge.ekTeslimatUcreti).toLocaleString('tr-TR', { minimumFractionDigits: 2 })}`
                : 'Ücretsiz';

            secenek.append(input, ad, ucret);
            mahalleListesi.appendChild(secenek);
        });
    });

    mahalleSecimiDegisti(null);
}

/* ========================================================
   TESLİMAT GÜNÜ & SAAT ARALIĞI
   ======================================================== */
function tarihTuruSec(el, tur) {
    document.querySelectorAll('.date-type-card').forEach(c => c.classList.remove('active'));
    el.classList.add('active');
    const input = el.querySelector('input');
    if (input) input.checked = true;

    const ozelTarihBox = document.getElementById('ozelTarihBox');
    if (tur === 'TarihSec') {
        if (ozelTarihBox) ozelTarihBox.style.display = 'block';
        const ozelTarihInput = document.getElementById('ozelTarihInput');
        if (document.getElementById('gizliTeslimatGunu')) {
            document.getElementById('gizliTeslimatGunu').value = ozelTarihInput ? ozelTarihInput.value : tur;
        }
    } else {
        if (ozelTarihBox) ozelTarihBox.style.display = 'none';
        if (document.getElementById('gizliTeslimatGunu')) {
            document.getElementById('gizliTeslimatGunu').value = tur;
        }
    }
    saatleriKontrolEt();
}

function ozelTarihDegisti() {
    const val = document.getElementById('ozelTarihInput')?.value;
    if (val && document.getElementById('gizliTeslimatGunu')) {
        document.getElementById('gizliTeslimatGunu').value = val;
    }
}

function saatleriKontrolEt() {
    const gunRadyo = document.querySelector('input[name="GorselTeslimatGunu"]:checked');
    const gunVal = gunRadyo ? gunRadyo.value : (document.getElementById('gizliTeslimatGunu')?.value || '');
    const bugunSeciliMi = (gunVal === 'Bugun');
    const simdi = new Date();
    const suAnkiToplamDakika = (simdi.getHours() * 60) + simdi.getMinutes();

    document.querySelectorAll('.time-select option').forEach(opt => {
        if (!opt.value) return;
        const baslangic = opt.getAttribute('data-baslangic');
        if (!baslangic) return;
        const [saat, dakika] = baslangic.split(':').map(Number);
        const baslangicToplamDakika = (saat * 60) + dakika;

        if (bugunSeciliMi && suAnkiToplamDakika > baslangicToplamDakika) {
            opt.disabled = true;
            if (!opt.text.includes('(Geçti)')) opt.text += ' (Geçti)';
        } else {
            opt.disabled = false;
            opt.text = opt.text.replace(' (Geçti)', '');
        }
    });
    saatDegisti();
}

function saatDegisti() {
    const isVip = document.getElementById('vipCheckbox')?.checked;
    const normalSaat = document.getElementById('normalSaatSelect')?.value || '';
    const vipSaat = document.getElementById('vipSaatSelect')?.value || '';
    const gizli = document.getElementById('gizliTeslimatSaati');
    if (gizli) {
        gizli.value = isVip ? vipSaat : normalSaat;
    }
}

function vipDurumuDegisti() {
    const checkbox = document.getElementById('vipCheckbox');
    const normalSaatSelect = document.getElementById('normalSaatSelect');
    const vipSaatKutusu = document.getElementById('vipSaatKutusu');
    const vipFarkEl = document.getElementById('gizliVipFark');

    if (checkbox && checkbox.checked) {
        if (normalSaatSelect) {
            normalSaatSelect.disabled = true;
            normalSaatSelect.style.opacity = '0.5';
        }
        if (vipSaatKutusu) vipSaatKutusu.style.display = 'block';
        if (vipFarkEl) vipFarkEl.value = vipFarkEl.getAttribute('data-ucret') || '0';
    } else {
        if (normalSaatSelect) {
            normalSaatSelect.disabled = false;
            normalSaatSelect.style.opacity = '1';
        }
        if (vipSaatKutusu) {
            vipSaatKutusu.style.display = 'none';
            const vipSelect = document.getElementById('vipSaatSelect');
            if (vipSelect) vipSelect.value = '';
        }
        if (vipFarkEl) vipFarkEl.value = '0';
    }
    saatDegisti();
    fiyatGuncelle();
}

/* ========================================================
   3. MÜHÜRLÜ EL YAZISI TEBRİK KARTI
   ======================================================== */
function sablonDoldur(kategori, metin) {
    document.querySelectorAll('.fl-template-chip').forEach(c => c.classList.remove('active'));
    event.currentTarget.classList.add('active');

    const txt = document.getElementById('kartNotuInput');
    if (txt) {
        txt.value = metin;
        txt.focus();
    }
}

/* ========================================================
   TESLİMAT KONUMU MODALI VE ELAZIĞ İLÇE/MAHALLE İŞLEMLERİ
   ======================================================== */
function doldurIl() {
    const ilSelect = document.getElementById('modalIl');
    if (!ilSelect) return;
    ilSelect.innerHTML = '<option value="Elazığ" selected>Elazığ</option>';

    const ilceSelect = document.getElementById('modalIlce');
    if (ilceSelect) {
        ilceSelect.innerHTML = '<option value="Merkez" selected>Merkez</option>';
    }

    ilceDegisti();
}

function ilDegisti() {
    ilceDegisti();
}

function ilceDegisti() {
    const mahalleSelect = document.getElementById('modalMahalle');
    if (!mahalleSelect) return;

    mahalleSelect.innerHTML = '<option value="" data-fark="0">Mahalle / Semt Seçiniz...</option>';

    let elazigMahalleler = [];
    if (typeof teslimatBolgeleri !== 'undefined' && Array.isArray(teslimatBolgeleri) && teslimatBolgeleri.length > 0) {
        elazigMahalleler = teslimatBolgeleri.filter(x => x.ilAdi === 'Elazığ');
    }

    if (elazigMahalleler.length > 0) {
        elazigMahalleler.forEach(m => {
            const ucretText = m.ekTeslimatUcreti > 0 ? ` (+₺${m.ekTeslimatUcreti.toLocaleString('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 0 })})` : ' (Ücretsiz Teslimat)';
            mahalleSelect.innerHTML += `<option value="${m.mahalleAdi}" data-fark="${m.ekTeslimatUcreti}">${m.mahalleAdi}${ucretText}</option>`;
        });
    }
}

function konumBilgileriniKaydet() {
    const il = "Elazığ";
    const ilce = document.getElementById('modalIlce')?.value || "Merkez";
    const mahalleSel = document.getElementById('modalMahalle');
    const mahalle = mahalleSel ? mahalleSel.value : '';
    const mahalleFark = parseFloat(mahalleSel?.options[mahalleSel.selectedIndex]?.getAttribute('data-fark') || 0);

    if (!mahalle) {
        teslimatUyarisiGoster('Lütfen teslimat için bir mahalle / semt seçin.');
        return;
    }

    document.getElementById('gizliIl').value = il;
    document.getElementById('gizliIlce').value = ilce;
    document.getElementById('gizliMahalle').value = mahalle;
    document.getElementById('gizliMahalleFark').value = mahalleFark;

    const triggerCard = document.getElementById('deliveryTriggerBox');
    if (triggerCard) {
        triggerCard.classList.add('is-selected');
    }

    const pinIcon = document.getElementById('deliveryPinIcon');
    if (pinIcon) {
        pinIcon.className = 'bi bi-check-circle-fill';
    }

    const eyebrow = document.getElementById('deliveryEyebrow');
    if (eyebrow) {
        eyebrow.innerText = 'TESLİMAT BÖLGESİ SEÇİLDİ';
    }

    const ozetEl = document.getElementById('secilenAdresOzet');
    if (ozetEl) {
        ozetEl.innerText = `${il} / ${ilce} · ${mahalle}`;
    }

    const zamanOzet = document.getElementById('secilenZamanOzet');
    if (zamanOzet) {
        zamanOzet.innerText = mahalleFark > 0
            ? `+₺${mahalleFark.toLocaleString('tr-TR')} ek teslimat ücreti`
            : 'Ücretsiz teslimat bölgesi';
    }

    const actionText = document.getElementById('deliveryActionText');
    if (actionText) {
        actionText.innerText = 'Değiştir';
    }

    const durumPill = document.getElementById('teslimatDurumPill');
    if (durumPill) {
        durumPill.innerText = mahalleFark > 0
            ? `+₺${mahalleFark.toLocaleString('tr-TR')} Teslimat`
            : 'Ücretsiz Teslimat';
    }

    fiyatGuncelle();

    var modalEl = document.getElementById('teslimatModal');
    if (modalEl) {
        var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
        modal.hide();
    }
}

/* ========================================================
   ADET VE FİYAT HESAPLAMA
   ======================================================== */
function adetDegistir(deger) {
    let gorselInput = document.getElementById('gorselAdet');
    let gercekInput = document.getElementById('gercekAdetInput');
    if (!gorselInput) return;

    let maxStok = parseInt(gorselInput.getAttribute('data-max-stok') || '999');
    if (isNaN(maxStok) || maxStok <= 0) return;

    let yeniAdet = (parseInt(gorselInput.value) || 1) + deger;
    if (yeniAdet < 1) yeniAdet = 1;
    if (yeniAdet > maxStok) {
        yeniAdet = maxStok;
    }

    gorselInput.value = yeniAdet;
    if (gercekInput) gercekInput.value = yeniAdet;
    fiyatGuncelle();
}

function fiyatGuncelle() {
    const priceDisplay = document.getElementById('urunFiyatDisplay');
    if (!priceDisplay) return;

    const basePrice = parseFloat(priceDisplay.getAttribute('data-base-price')) || 0;
    const boyutFark = parseFloat(document.getElementById('gizliBoyutFark')?.value || 0);
    const adet = parseInt(document.getElementById('gercekAdetInput')?.value) || 1;
    const mahalleFark = parseFloat(document.getElementById('gizliMahalleFark')?.value || 0);
    const vipFark = parseFloat(document.getElementById('gizliVipFark')?.value || 0);

    let toplamFiyat = ((basePrice + boyutFark) * adet) + mahalleFark + vipFark;

    priceDisplay.innerText = toplamFiyat.toLocaleString('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
}

/* ========================================================
   SİPARİŞ DOĞRULAMA VE WHATSAPP ENTEGRASYONU
   ======================================================== */
document.addEventListener("DOMContentLoaded", function () {
    const siparisFormu = document.getElementById('siparisFormu');
    if (siparisFormu) {
        siparisFormu.addEventListener('submit', function (e) {
            if (!document.getElementById('gizliMahalle')?.value) {
                e.preventDefault();
                teslimatUyarisiGoster("Lütfen teslimat adresinizi seçin.", true);
                return false;
            }
            if (!teslimatSecimleriGecerliMi()) {
                e.preventDefault();
                return false;
            }
        });
    }
});

function teslimatSecimleriGecerliMi() {
    const gun = document.getElementById('gizliTeslimatGunu')?.value?.trim() || '';
    const saat = document.getElementById('gizliTeslimatSaati')?.value?.trim() || '';

    if (!gun) {
        teslimatUyarisiGoster('Devam etmek için teslimat gününü seçin.');
        return false;
    }

    if (gun === 'TarihSec' || /^\d{4}-\d{2}-\d{2}$/.test(gun)) {
        const tarih = document.getElementById('ozelTarihInput')?.value?.trim() || '';
        if (!tarih) {
            teslimatUyarisiGoster('Devam etmek için teslimat tarihini seçin.');
            return false;
        }
    }

    if (!saat) {
        teslimatUyarisiGoster('Devam etmek için teslimat saatini seçin.');
        return false;
    }

    return true;
}

function teslimatUyarisiGoster(mesaj, adresSecimiAc = false) {
    let bildirim = document.getElementById('teslimatUyarisi');
    if (!bildirim) {
        bildirim = document.createElement('div');
        bildirim.id = 'teslimatUyarisi';
        bildirim.className = 'delivery-toast';
        bildirim.setAttribute('role', 'alert');
        bildirim.setAttribute('aria-live', 'assertive');
        bildirim.innerHTML = '<span class="delivery-toast-icon"><i class="bi bi-geo-alt-fill"></i></span><span class="delivery-toast-message"></span><button type="button" class="delivery-toast-close" aria-label="Bildirimi kapat"><i class="bi bi-x-lg"></i></button>';
        document.body.appendChild(bildirim);
        bildirim.querySelector('.delivery-toast-close').addEventListener('click', () => bildirim.classList.remove('is-visible'));
    }

    bildirim.querySelector('.delivery-toast-message').textContent = mesaj;
    bildirim.classList.add('is-visible');
    clearTimeout(bildirim.dismissTimer);
    bildirim.dismissTimer = setTimeout(() => bildirim.classList.remove('is-visible'), 4200);

    if (adresSecimiAc) {
        setTimeout(() => {
            const modalEl = document.getElementById('teslimatModal');
            if (modalEl && window.bootstrap?.Modal) {
                (bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl)).show();
            }
        }, 500);
    }
}

function whatsappSiparis() {
    if (!document.getElementById('gizliMahalle')?.value) {
        teslimatUyarisiGoster("Siparişe devam etmek için teslimat adresinizi seçin.", true);
        return;
    }
    if (!teslimatSecimleriGecerliMi()) return;

    const urunAdi = baseUrunAd || "Çiçek Aranjmanı";
    const boyut = document.getElementById('gizliBoyutAdi')?.value || "Klasik";
    const gun = document.getElementById('gizliTeslimatGunu')?.value || "";
    const saat = document.getElementById('gizliTeslimatSaati')?.value || "";
    const ilce = document.getElementById('gizliIlce')?.value || "Merkez";
    const mahalle = document.getElementById('gizliMahalle')?.value || "Belirtilmedi";
    const alici = document.getElementById('aliciAdiInput')?.value || "";
    const kartNotu = document.getElementById('kartNotuInput')?.value || "";
    const adet = document.getElementById('gercekAdetInput')?.value || "1";
    const tutar = document.getElementById('urunFiyatDisplay')?.innerText || "";

    let konumMetni = `Elazığ / ${ilce} / ${mahalle}`;

    let msg = `*🌸 LİDYA ÇİÇEKÇİLİK SİPARİŞİ 🌸*%0A%0A` +
              `*Ürün:* ${urunAdi} (${baseUrunKod})%0A` +
              `*Boyut:* ${boyut}%0A` +
              `*Adet:* ${adet}%0A` +
              `*Teslimat:* ${gun} | ${saat}%0A` +
              `*Teslim Yeri:* ${konumMetni}%0A`;

    if (alici) {
        msg += `*Alıcı İsmi:* ${alici}%0A`;
    }
    if (kartNotu) {
        msg += `*Mühürlü Tebrik Notu:* ${kartNotu}%0A`;
    }

    msg += `%0A*Toplam Tutar:* ₺${tutar}%0A%0ASiparişimi onaylamak istiyorum.`;

    window.open(`https://api.whatsapp.com/send?phone=${rawWhatsappNo}&text=${msg}`, '_blank');
}
