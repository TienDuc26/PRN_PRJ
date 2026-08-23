// ============================================================
// TourViet - Admin JS
// ============================================================
document.addEventListener('DOMContentLoaded', function() {

    // Sidebar toggle (mobile)
    const toggle = document.getElementById('sidebarToggle');
    const sidebar = document.querySelector('.admin-sidebar');
    if (toggle && sidebar) {
        toggle.addEventListener('click', () => {
            sidebar.classList.toggle('show');
        });
        // close on outside click
        document.addEventListener('click', (e) => {
            if (window.innerWidth < 768
                && sidebar.classList.contains('show')
                && !sidebar.contains(e.target)
                && !toggle.contains(e.target)) {
                sidebar.classList.remove('show');
            }
        });
    }

    // Auto-dismiss alerts after 5s
    setTimeout(() => {
        document.querySelectorAll('.alert-success-custom, .alert.alert-info').forEach(el => {
            try { bootstrap.Alert.getOrCreateInstance(el).close(); } catch(e) {}
        });
    }, 5000);

    // Confirm dialog using SweetAlert2
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', function(e) {
            e.preventDefault();
            const msg = this.dataset.confirm;
            const href = this.dataset.href || this.href;
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

    // Form loading state
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

    // Set active sidebar link based on current URL
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar-nav .nav-link').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && currentPath.startsWith(href) && href !== '/admin/dashboard') {
            link.classList.add('active');
        } else if (href === '/admin/dashboard' && currentPath === '/admin') {
            link.classList.add('active');
        }
    });
});

// Toast helper
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