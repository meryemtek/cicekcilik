document.addEventListener('DOMContentLoaded', function () {
    const fileInput = document.getElementById('sliderImageFile');
    const form = fileInput?.closest('form');
    const modalElement = document.getElementById('sliderCropModal');
    if (!fileInput || !form || !modalElement) return;

    // Admin layout uses a stacking context; Bootstrap modals must be under body.
    document.body.appendChild(modalElement);
    const modal = new bootstrap.Modal(modalElement);
    const image = document.getElementById('sliderCropImage');
    const preview = document.getElementById('sliderCropPreview');
    const widthInput = document.getElementById('sliderCropWidth');
    const heightInput = document.getElementById('sliderCropHeight');
    const error = document.getElementById('sliderCropError');
    const applyButton = document.getElementById('sliderCropApply');
    const selectedPreview = document.getElementById('sliderSelectedPreview');
    let cropper = null;
    let sourceUrl = null;
    let previewUrl = null;
    let acceptedFile = null;
    let applied = false;

    function setFile(file) {
        const transfer = new DataTransfer();
        if (file) transfer.items.add(file);
        fileInput.files = transfer.files;
    }

    function dimensions() {
        if (!widthInput.reportValidity() || !heightInput.reportValidity()) return null;
        const width = Number(widthInput.value);
        const height = Number(heightInput.value);
        if (!Number.isInteger(width) || !Number.isInteger(height) || width < 320 || width > 3840 || height < 160 || height > 2160) return null;
        return { width, height };
    }

    fileInput.addEventListener('change', function () {
        const file = fileInput.files[0];
        if (!file) return;
        if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type) || file.size > 15 * 1024 * 1024) {
            error.textContent = 'JPG, PNG veya WEBP biçiminde, en fazla 15 MB bir görsel seçin.';
            setFile(acceptedFile);
            modal.show();
            return;
        }
        error.textContent = '';
        applied = false;
        if (sourceUrl) URL.revokeObjectURL(sourceUrl);
        sourceUrl = URL.createObjectURL(file);
        image.src = sourceUrl;
        modal.show();
    });

    modalElement.addEventListener('shown.bs.modal', function () {
        if (!sourceUrl) return;
        cropper?.destroy();
        cropper = new Cropper(image, {
            aspectRatio: NaN,
            viewMode: 1,
            dragMode: 'crop',
            cropBoxMovable: true,
            cropBoxResizable: true,
            movable: true,
            autoCropArea: 0.9,
            preview: '#sliderCropPreview',
            responsive: true,
            background: false,
            crop(event) {
                if (event.detail.width > 0 && event.detail.height > 0) {
                    preview.style.aspectRatio = `${event.detail.width} / ${event.detail.height}`;
                }
            }
        });
    });

    modalElement.addEventListener('hidden.bs.modal', function () {
        cropper?.destroy();
        cropper = null;
        if (sourceUrl) URL.revokeObjectURL(sourceUrl);
        sourceUrl = null;
        image.removeAttribute('src');
        if (!applied) setFile(acceptedFile);
        error.textContent = '';
    });

    modalElement.querySelectorAll('[data-slider-crop-action]').forEach(button => {
        button.addEventListener('click', function () {
            if (!cropper) return;
            switch (button.dataset.sliderCropAction) {
                case 'crop':
                case 'move':
                    cropper.setDragMode(button.dataset.sliderCropAction);
                    modalElement.querySelectorAll('[data-slider-crop-action="crop"], [data-slider-crop-action="move"]').forEach(modeButton => {
                        modeButton.classList.toggle('btn-primary', modeButton === button);
                        modeButton.classList.toggle('btn-outline-secondary', modeButton !== button);
                    });
                    break;
                case 'zoom-in': cropper.zoom(0.1); break;
                case 'zoom-out': cropper.zoom(-0.1); break;
                case 'rotate': cropper.rotate(90); break;
                case 'reset': cropper.reset(); break;
            }
        });
    });

    applyButton.addEventListener('click', function () {
        const size = dimensions();
        if (!cropper || !size) {
            error.textContent = 'Geçerli genişlik ve yükseklik girin.';
            return;
        }
        const crop = cropper.getData(true);
        if (!crop.width || !crop.height) {
            error.textContent = 'Kırpılacak alanı seçin.';
            return;
        }
        const scale = Math.min(1, size.width / crop.width, size.height / crop.height);
        const canvas = cropper.getCroppedCanvas({
            width: Math.max(1, Math.round(crop.width * scale)),
            height: Math.max(1, Math.round(crop.height * scale)),
            imageSmoothingEnabled: true,
            imageSmoothingQuality: 'high',
            fillColor: '#ffffff'
        });
        if (!canvas) {
            error.textContent = 'Görsel işlenemedi. Başka bir dosya deneyin.';
            return;
        }
        applyButton.disabled = true;
        canvas.toBlob(function (blob) {
            applyButton.disabled = false;
            if (!blob) {
                error.textContent = 'Görsel işlenemedi. Başka bir dosya deneyin.';
                return;
            }
            if (blob.size > 15 * 1024 * 1024) {
                error.textContent = 'Düzenlenen görsel 15 MB sınırını aşıyor. Daha küçük boyut seçin.';
                return;
            }
            acceptedFile = new File([blob], 'slider_crop.webp', { type: 'image/webp' });
            setFile(acceptedFile);
            if (previewUrl) URL.revokeObjectURL(previewUrl);
            previewUrl = URL.createObjectURL(blob);
            selectedPreview.src = previewUrl;
            selectedPreview.hidden = false;
            applied = true;
            modal.hide();
        }, 'image/webp', 0.9);
    });

    form.addEventListener('submit', function (event) {
        if (fileInput.files.length && fileInput.files[0] !== acceptedFile) {
            event.preventDefault();
            error.textContent = 'Görseli kırpıp uygulayın.';
            modal.show();
        }
    });
});
