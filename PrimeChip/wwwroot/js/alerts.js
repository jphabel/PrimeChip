function openPurchaseModal(id, name, reorderLevel, currentStock) {
    document.getElementById('purchaseInventoryId').value = id;
    document.getElementById('purchaseProductName').textContent = name;
    document.getElementById('purchaseCurrentStock').textContent = currentStock;
    document.getElementById('purchaseReorderLevel').textContent = reorderLevel;
    new bootstrap.Modal(document.getElementById('purchaseModal')).show();
}
