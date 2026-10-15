document.addEventListener('DOMContentLoaded', async function () {
    const select = document.getElementById('LinkUrl');
    const status = document.getElementById('kategoriLoadStatus');
    if (!select || !select.dataset.kategoriApi || !status) return;

    const initialLink = select.dataset.currentLink || select.value;
    try {
        const response = await fetch(select.dataset.kategoriApi, { cache: 'no-store' });
        if (!response.ok) throw new Error(`Kategori API: ${response.status}`);
        const kategoriler = await response.json();
        if (!Array.isArray(kategoriler)) throw new Error('Kategori listesi beklenen biçimde değil.');

        const options = [new Option('Kategori seçin', '')];
        kategoriler
            .sort((a, b) => (a.ad || a.Ad || '').localeCompare(b.ad || b.Ad || '', 'tr'))
            .forEach(kategori => {
                const ad = kategori.ad || kategori.Ad || 'Adsız kategori';
                const ustKategoriAdi = kategori.ustKategoriAdi || kategori.UstKategoriAdi;
                const seoUrl = (kategori.seoUrl || kategori.SeoUrl || '').trim();
                const aktifMi = kategori.aktifMi ?? kategori.AktifMi;
                const etiket = `${ustKategoriAdi ? `${ustKategoriAdi} / ` : ''}${ad}${aktifMi ? '' : ' (pasif)'}${seoUrl ? '' : ' — SEO adresi yok'}`;
                const option = new Option(etiket, seoUrl ? `/kategori/${seoUrl}` : '');
                option.disabled = !seoUrl;
                options.push(option);
            });

        const selectedLink = select.value || initialLink;
        if (selectedLink && !options.some(option => option.value === selectedLink)) {
            options.push(new Option(`Mevcut bağlantı: ${selectedLink}`, selectedLink));
        }
        select.replaceChildren(...options);
        select.value = selectedLink || '';

        if (kategoriler.length === 0) {
            status.textContent = 'Admin/Kategori sayfasında kayıtlı kategori bulunamadı.';
        } else if (!options.some(option => option.value.startsWith('/kategori/'))) {
            status.textContent = 'Kategorilerde SEO adresi bulunmuyor. Kategori yönetiminden SEO adreslerini doldurun.';
        } else {
            status.textContent = '';
        }
    } catch (error) {
        if (select.options.length <= 1) {
            status.textContent = 'Kategoriler yüklenemedi. API bağlantısını kontrol edip sayfayı yenileyin.';
        }
        console.error('Slider kategorileri yüklenemedi:', error);
    }
});
