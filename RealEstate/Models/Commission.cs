using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Models;

public class Commission
{
    [Key]
    public int CommissionId { get; set; }

    public int TransactionId { get; set; }
    [ForeignKey("TransactionId")]
    public Transaction Transaction { get; set; } = null!;

    public int EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;

    public decimal Amount { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}