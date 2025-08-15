namespace InventoryService.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProductId { get; set; } = null!;
    public int AvailableQuantity { get; private set; }

    private InventoryItem() { }

    public InventoryItem(string productId, int initialQuantity)
    {
        if (string.IsNullOrWhiteSpace(productId)) throw new ArgumentException("ProductId boş olamaz", nameof(productId));
        if (initialQuantity < 0) throw new ArgumentException("Miktar negatif olamaz", nameof(initialQuantity));

        ProductId = productId;
        AvailableQuantity = initialQuantity;
    }

    public bool TryDecrease(int quantity)
    {
        if (quantity <= 0) return false;
        if (AvailableQuantity < quantity) return false;
        AvailableQuantity -= quantity;
        return true;
    }

    public void Increase(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Artış pozitif olmalı", nameof(quantity));
        AvailableQuantity += quantity;
    }
}