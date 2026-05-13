using System;
using System.Collections.Generic;

namespace RealEstate.Models;

/// <summary>
/// Transaction - core business entity for property sales
/// </summary>
public class Transaction
{
    public int TransactionId { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public int SellerId { get; set; }
    public PropertySeller Seller { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string Status { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
