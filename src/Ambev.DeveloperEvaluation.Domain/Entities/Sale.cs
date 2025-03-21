using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public int SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Branch { get; set; }
    public bool IsCancelled { get; set; }

    public List<SaleItem> Items { get; set; } = new();
}

public class SaleItem
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TotalItemAmount { get; set; }
    public bool IsCancelled { get; set; }
}
