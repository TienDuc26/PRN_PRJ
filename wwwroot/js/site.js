// ============================================================
// TourViet - Customer Site JS
// ============================================================

document.addEventListener('DOMContentLoaded', function() {

    // Auto-dismiss success/info alerts after 5s
    setTimeout(() => {
        document.querySelectorAll('.alert-success-custom, .alert.alert-info').forEach(el => {
            try { bootstrap.Alert.getOrCreateInstance(el).close(); } catch(e) {}
        });
    }, 5000);

    // Back to top button
    const backBtn = document.getElementById('backToTop');
    if (backBtn) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 300) backBtn.classList.add('show');
            else backBtn.classList.remove('show');
        });
        backBtn.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));
    }

    // Header shrink on scroll
    const header = document.querySelector('.main-header');
    if (header) {
        let last = 0;
        window.addEventListener('scroll', () => {
            const y = window.scrollY;
            if (y > 60) header.classList.add('shrink');
            else header.classList.remove('shrink');
            last = y;
        });
    }

    // Form submission loading state (chống double-click trên form quan trọng)
    document.querySelectorAll('form[data-loading]').forEach(form => {
        form.addEventListener('submit', function() {
            const btn = form.querySelector('button[type="submit"]');
            if (btn) {
                btn.disabled = true;
                const original = btn.innerHTML;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';
                setTimeout(() => { btn.disabled = false; btn.innerHTML = original; }, 8000);
            }
        });
    });

    // Confirm dialog using SweetAlert2 (thay cho confirm() mặc định)
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', function(e) {
            e.preventDefault();
            const msg = this.dataset.confirm;
            const href = this.dataset.href || this.href || this.getAttribute('formaction');
            const form = this.closest('form');
            Swal.fire({
                title: 'Xác nhận',
                text: msg,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Hủy',
                confirmButtonColor: '#dc3545',
                reverseButtons: true
            }).then(result => {
                if (result.isConfirmed) {
                    if (form) form.submit();
                    else if (href) window.location.href = href;
                }
            });
        });
    });

    // Star rating widget
    document.querySelectorAll('.star-rating').forEach(container => {
        const stars = container.querySelectorAll('.star');
        const input = container.querySelector('input[type="hidden"]');
        const setRating = (n) => {
            stars.forEach((s, i) => {
                s.classList.toggle('bi-star-fill', i < n);
                s.classList.toggle('bi-star', i >= n);
            });
            if (input) input.value = n;
        };
        stars.forEach((s, i) => {
            s.addEventListener('click', () => setRating(i + 1));
            s.addEventListener('mouseenter', () => setRating(i + 1));
        });
        container.addEventListener('mouseleave', () => setRating(parseInt(input?.value || '0')));
    });
});

// Utility: format VND
function formatVND(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

// Utility: show toast
function showToast(icon, title) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
    Toast.fire({ icon, title });
}