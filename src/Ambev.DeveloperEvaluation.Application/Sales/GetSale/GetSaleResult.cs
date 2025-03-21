using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleResult
    {
        public Guid Id { get; set; }
        public int SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public string Branch { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public List<GetSaleItemDto> Items { get; set; }
    }

    public class GetSaleItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalItemAmount { get; set; }
        public bool IsCancelled { get; set; }
    }
}