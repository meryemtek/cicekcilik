document.addEventListener("DOMContentLoaded", function () {
    const list = document.getElementById("marqueeList");
    if (list && list.children.length > 1) {
        setInterval(() => {
            const firstItem = list.children[0];
            list.style.transform = 'translateY(-34px)';
            setTimeout(() => {
                list.style.transition = 'none';
                list.style.transform = 'translateY(0)';
                list.appendChild(firstItem);
                setTimeout(() => {
                    list.style.transition = 'transform 0.5s ease-in-out';
                }, 50);
            }, 500);
        }, 3500);
    }
    doldurIl();
    saatleriKontrolEt();
    setInterval(saatleriKontrolEt, 60000);
});

function doldurIl() {
    const ilSelect = document.getElementById('modalIl');
    if (!ilSelect || typeof teslimatBolgeleri === 'undefined') return;
    const iller = [...new Set(teslimatBolgeleri.map(x => x.ilAdi))];
    ilSelect.innerHTML = '<option value="">İl Seçiniz</option>';
    iller.forEach(il => { ilSelect.innerHTML += `<option value="${il}">${il}</option>`; });
    if (iller.length === 1) { ilSelect.value = iller[0]; ilDegisti(); }
}

function ilDegisti() {
    const secilenIl = document.getElementById('modalIl').value;
    const ilceSelect = document.getElementById('modalIlce');
    const mahalleSelect = document.getElementById('modalMahalle');
    if (!ilceSelect || !mahalleSelect) return;
    mahalleSelect.innerHTML = '<option value="" data-fark="0">Mahalle Seçiniz</option>';
    if (!secilenIl) { ilceSelect.innerHTML = '<option value="">Önce İl Seçiniz</option>'; return; }
    const ilceler = [...new Set(teslimatBolgeleri.filter(x => x.ilAdi === secilenIl).map(x => x.ilceAdi))];
    ilceSelect.innerHTML = '<option value="">İlçe Seçiniz</option>';
    ilceler.forEach(ilce => { ilceSelect.innerHTML += `<option value="${ilce}">${ilce}</option>`; });
    if (ilceler.length === 1) { ilceSelect.value = ilceler[0]; ilceDegisti(); }
}

function ilceDegisti() {
    const secilenIl = document.getElementById('modalIl').value;
    const secilenIlce = document.getElementById('modalIlce').value;
    const mahalleSelect = document.getElementById('modalMahalle');
    if (!mahalleSelect) return;
    if (!secilenIlce) { mahalleSelect.innerHTML = '<option value="" data-fark="0">Önce İlçe Seçiniz</option>'; return; }
    const mahalleler = teslimatBolgeleri.filter(x => x.ilAdi === secilenIl && x.ilceAdi === secilenIlce);
    mahalleSelect.innerHTML = '<option value="" data-fark="0">Mahalle Seçiniz...</option>';
    mahalleler.forEach(m => {
        const ucretText = m.ekTeslimatUcreti > 0 ? ` (+₺${m.ekTeslimatUcreti.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })})` : ' (Ücretsiz)';
        mahalleSelect.innerHTML += `<option value="${m.mahalleAdi}" data-fark="${m.ekTeslimatUcreti}">${m.mahalleAdi}${ucretText}</option>`;
    });
}

function konumBilgileriniKaydet() {
    const il = document.getElementById('modalIl').value;
    const ilce = document.getElementById('modalIlce').value;
    const mahalleSel = document.getElementById('modalMahalle');
    const mahalle = mahalleSel.value;
    const mahalleFark = parseFloat(mahalleSel.options[mahalleSel.selectedIndex]?.getAttribute('data-fark') || 0);

    if (!mahalle) { alert('Lütfen geçerli bir mahalle seçiniz.'); return; }

    document.getElementById('gizliIl').value = il;
    document.getElementById('gizliIlce').value = ilce;
    document.getElementById('gizliMahalle').value = mahalle;
    document.getElementById('gizliMahalleFark').value = mahalleFark;
    document.getElementById('secilenAdresOzet').innerText = `${il} / ${ilce} / ${mahalle}`;
    fiyatGuncelle();

    var modalEl = document.getElementById('teslimatModal');
    var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
    modal.hide();
}

function tarihTuruSec(el, tur) {
    document.querySelectorAll('.date-type-card').forEach(c => c.classList.remove('active'));
    el.classList.add('active');
    el.querySelector('input').checked = true;
    document.getElementById('ozelTarihBox').style.display = (tur === 'TarihSec') ? 'block' : 'none';
    document.getElementById('gizliTeslimatGunu').value = (tur === 'TarihSec') ? (document.getElementById('ozelTarihInput').value || tur) : tur;
    saatleriKontrolEt();
}

function ozelTarihDegisti() {
    document.getElementById('gizliTeslimatGunu').value = document.getElementById('ozelTarihInput').value;
}

function saatleriKontrolEt() {
    const gunRadyo = document.querySelector('input[name="GorselTeslimatGunu"]:checked');
    if (!gunRadyo) return;
    const bugunSeciliMi = (gunRadyo.value === 'Bugun');
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
    const isVip = document.getElementById('vipCheckbox').checked;
    document.getElementById('gizliTeslimatSaati').value = isVip ? document.getElementById('vipSaatSelect').value : document.getElementById('normalSaatSelect').value;
}

function vipDurumuDegisti() {
    const checkbox = document.getElementById('vipCheckbox');
    const normalSaatSelect = document.getElementById('normalSaatSelect');
    const vipSaatKutusu = document.getElementById('vipSaatKutusu');
    if (checkbox.checked) {
        normalSaatSelect.disabled = true; normalSaatSelect.style.opacity = '0.5';
        vipSaatKutusu.style.display = 'block';
        document.getElementById('gizliVipFark').value = document.getElementById('gizliVipFark').getAttribute('data-ucret');
    } else {
        normalSaatSelect.disabled = false; normalSaatSelect.style.opacity = '1';
        vipSaatKutusu.style.display = 'none'; document.getElementById('vipSaatSelect').value = "";
        document.getElementById('gizliVipFark').value = "0";
    }
    saatDegisti(); fiyatGuncelle();
}

function adetDegistir(deger) {
    let gorselInput = document.getElementById('gorselAdet');
    let gercekInput = document.getElementById('gercekAdetInput');
    let yeniAdet = parseInt(gorselInput.value) + deger;
    if (yeniAdet < 1) yeniAdet = 1;
    gorselInput.value = yeniAdet;
    gercekInput.value = yeniAdet;
    fiyatGuncelle();
}

function fiyatGuncelle() {
    const priceDisplay = document.getElementById('urunFiyatDisplay');
    const basePrice = parseFloat(priceDisplay.getAttribute('data-base-price')) || 0;

    const varyantSelect = document.querySelector('select[name="VaryantId"]');
    let varyantFark = varyantSelect && varyantSelect.selectedIndex >= 0
        ? parseFloat(varyantSelect.options[varyantSelect.selectedIndex].getAttribute('data-fark') || 0)
        : 0;

    let adet = parseInt(document.getElementById('gercekAdetInput').value) || 1;
    const mahalleFark = parseFloat(document.getElementById('gizliMahalleFark').value || 0);
    const vipFark = parseFloat(document.getElementById('gizliVipFark').value || 0);

    let toplamFiyat = ((basePrice + varyantFark) * adet) + mahalleFark + vipFark;

    priceDisplay.innerText = toplamFiyat.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

document.addEventListener("DOMContentLoaded", function () {
    const siparisFormu = document.getElementById('siparisFormu');
    if (siparisFormu) {
        siparisFormu.addEventListener('submit', function (e) {
            if (!document.getElementById('gizliMahalle').value) {
                e.preventDefault(); alert("Lütfen bir teslimat adresi seçiniz.");
                var modalEl = document.getElementById('teslimatModal');
                (bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl)).show();
                return false;
            }
            if (!document.getElementById('gizliTeslimatSaati').value) {
                e.preventDefault(); alert("Lütfen teslimat saati seçiniz."); return false;
            }
        });
    }
});

function whatsappSiparis() {
    if (!document.getElementById('gizliMahalle').value || !document.getElementById('gizliTeslimatSaati').value) {
        alert("Lütfen önce teslimat konumu ve saatini seçiniz."); return;
    }
    const msg = `Merhaba, sipariş vermek istiyorum.%0A%0A*Ürün:* ${baseUrunKod} - ${baseUrunAd}%0A*Tarih & Saat:* ${document.getElementById('gizliTeslimatGunu').value} / ${document.getElementById('gizliTeslimatSaati').value}%0A*Konum:* ${document.getElementById('gizliIl').value} - ${document.getElementById('gizliIlce').value} - ${document.getElementById('gizliMahalle').value}%0A*Tutar:* ${document.getElementById('urunFiyatDisplay').innerText} ₺`;
    window.open(`https://api.whatsapp.com/send?phone=${rawWhatsappNo}&text=${msg}`, '_blank');
}