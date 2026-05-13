// inventory.js

document.addEventListener('DOMContentLoaded', function () {

    document.getElementById('searchInput').addEventListener('keyup', filterTable);
    document.getElementById('filterCategory').addEventListener('change', filterTable);
    document.getElementById('filterStatus').addEventListener('change', filterTable);

    function filterTable() {
        const search = document.getElementById('searchInput').value.toLowerCase().trim();
        const category = document.getElementById('filterCategory').value.toLowerCase().trim();
        const status = document.getElementById('filterStatus').value.toLowerCase().trim();

        const rows = document.querySelectorAll('tbody tr');

        rows.forEach(row => {
            const cells = row.querySelectorAll('td');

            const name = cells[0]?.innerText.toLowerCase().trim() || '';
            const sku = cells[1]?.innerText.toLowerCase().trim() || '';
            const rowCategory = cells[2]?.innerText.toLowerCase().trim() || '';
            const rowStatus = cells[7]?.querySelector('.badge')?.innerText.toLowerCase().trim() || '';

            const matchesSearch = name.includes(search) || sku.includes(search);
            const matchesCategory = category === '' || rowCategory === category;
            const matchesStatus = status === '' || rowStatus === status;

            row.style.display = matchesSearch && matchesCategory && matchesStatus ? '' : 'none';
        });
    }

    window.clearFilters = function () {
        document.getElementById('searchInput').value = '';
        document.getElementById('filterCategory').value = '';
        document.getElementById('filterStatus').value = '';
        filterTable();
    };

    window.confirmDelete = function (id) {
        document.getElementById('deleteForm').action = '/Inventory/Delete/' + id;
        document.getElementById('deleteId').value = id;
        const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
        deleteModal.show();
    }

});