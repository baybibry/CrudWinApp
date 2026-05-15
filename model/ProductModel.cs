using System;

namespace crudApp.model;

public class ProductModel
{
    public int Id { get; set; }

    public string ProductName { get; set; } = "";
    public string Description { get; set; } = "";
    
    public decimal Price { get; set; }
    
    public decimal ConstPrice { get; set; }
    public decimal Discount { get; set; }
    public long Quantity { get; set; }
    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}