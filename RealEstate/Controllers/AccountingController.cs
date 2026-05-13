using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models;
using RealEstate.Models.Seller;
using RealEstate.Services;
using System.Linq;
using System.Text.Json;

namespace RealEstate.Controllers;

[Route("accounting")]
[AuthorizeRole("Accounting")]
public class AccountingController : Controller
{
    private readonly ApplicationDBContext _context;

    public AccountingController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;

        ViewBag.TotalRevenue = _context.PaymentTransactions
            .Where(t => t.Status == "succeeded")
            .Sum(t => (decimal?)t.Amount) ?? 0;

        ViewBag.TotalPayouts = _context.CommissionDeals
            .Where(d => d.Status == CommissionDealStatus.Completed)
            .Sum(d => (decimal?)d.AgentPayoutAmount) ?? 0;

        ViewBag.PendingPayouts = _context.CommissionDeals
            .Where(d => d.Status == CommissionDealStatus.ApprovedForPayroll || d.Status == CommissionDealStatus.Locked)
            .Sum(d => (decimal?)d.AgentPayoutAmount) ?? 0;

        ViewBag.PaidCount = _context.CommissionDeals
            .Count(d => d.Status == CommissionDealStatus.Completed);

        ViewBag.PendingCount = _context.CommissionDeals
            .Count(d => d.Status == CommissionDealStatus.ApprovedForPayroll || d.Status == CommissionDealStatus.Locked);

        ViewBag.RecentTransactions = _context.PaymentTransactions
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedDate)
            .Take(5)
            .ToList();

        ViewBag.RecentPayouts = _context.CommissionDeals
            .AsNoTracking()
            .Where(d => d.Status == CommissionDealStatus.Completed)
            .OrderByDescending(d => d.CompletedAtUtc)
            .Take(5)
            .ToList();

        return View("~/Views/Accounting/Dashboard.cshtml");
    }

    // ── Payment Processing ────────────────────────────────────────────────────

    [HttpGet("payment/approved-payouts")]
    public IActionResult ApprovedPayouts()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var deals = _context.CommissionDeals
            .AsNoTracking()
            .Where(d => d.Status == CommissionDealStatus.ApprovedForPayroll || d.Status == CommissionDealStatus.Locked)
            .OrderByDescending(d => d.ManagerApprovedAtUtc)
            .ToList();
        return View("~/Views/Accounting/ApprovedPayouts.cshtml", deals);
    }

    [HttpGet("payment/process-payments")]
    public IActionResult ProcessPayments(int? id)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        if (id.HasValue)
        {
            var deal = _context.CommissionDeals
                .AsNoTracking()
                .FirstOrDefault(d => d.Id == id.Value && (d.Status == CommissionDealStatus.ApprovedForPayroll || d.Status == CommissionDealStatus.Locked));
            ViewBag.SelectedDeal = deal;
        }

        var deals = _context.CommissionDeals
            .AsNoTracking()
            .Where(d => d.Status == CommissionDealStatus.ApprovedForPayroll || d.Status == CommissionDealStatus.Locked)
            .OrderByDescending(d => d.ManagerApprovedAtUtc)
            .ToList();

        return View("~/Views/Accounting/ProcessPayments.cshtml", deals);
    }

    [HttpPost("payment/process-payments")]
    public IActionResult ProcessPayment([FromForm] int dealId, [FromForm] string paymentMethod, [FromForm] string referenceNumber, [FromForm] DateTime paymentDate)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var accountingId = HttpContext.Session.GetInt32("UserId") ?? 0;

        var deal = _context.CommissionDeals.FirstOrDefault(d => d.Id == dealId);
        if (deal == null) return NotFound();

        deal.Status = CommissionDealStatus.Completed;
        deal.CompletedAtUtc = DateTime.UtcNow;
        deal.AccountingReleasedAtUtc = paymentDate;
        deal.AccountingReleasedByUserId = accountingId;
        deal.PayStubUrl = deal.PayStubUrl ?? $"/accounting/payout-stubs/{deal.BatchId ?? "SINGLE"}/{deal.Id}";

        var audit = new Dictionary<string, object?>();
        try { audit = JsonSerializer.Deserialize<Dictionary<string, object?>>(deal.AuditJson) ?? new(); } catch { }
        audit["processedAtUtc"] = DateTime.UtcNow;
        audit["processedByUserId"] = accountingId;
        audit["paymentMethod"] = paymentMethod;
        audit["referenceNumber"] = referenceNumber;
        audit["paymentDate"] = paymentDate;
        deal.AuditJson = JsonSerializer.Serialize(audit);

        _context.SaveChanges();

        TempData["SuccessMessage"] = "Payment processed successfully.";
        return RedirectToAction(nameof(ProcessPayments));
    }

    [HttpGet("payment/payment-records")]
    public IActionResult PaymentRecords()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var deals = _context.CommissionDeals
            .AsNoTracking()
            .Where(d => d.Status == CommissionDealStatus.Completed)
            .OrderByDescending(d => d.CompletedAtUtc)
            .Take(200)
            .ToList();
        return View("~/Views/Accounting/PaymentRecords.cshtml", deals);
    }

    // ── Commission Management ─────────────────────────────────────────────────

    [HttpGet("commission/records")]
    public IActionResult CommissionRecords()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var deals = _context.CommissionDeals
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAtUtc)
            .Take(200)
            .ToList();
        return View("~/Views/Accounting/CommissionRecords.cshtml", deals);
    }

    [HttpGet("commission/breakdown")]
    public IActionResult CommissionBreakdown()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var deals = _context.CommissionDeals
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAtUtc)
            .Take(200)
            .ToList();

        ViewBag.TotalGross = deals.Sum(d => d.GrossCommission);
        ViewBag.TotalAgentShare = deals.Sum(d => d.GrossCommission * (d.AgentSplitPercent / 100m));
        ViewBag.TotalCompanyShare = deals.Sum(d => d.GrossCommission * (d.CompanySplitPercent / 100m));
        ViewBag.TotalAddOns = deals.Sum(d => d.AddOnsTotal);
        ViewBag.TotalAgentPayout = deals.Sum(d => d.AgentPayoutAmount);

        return View("~/Views/Accounting/CommissionBreakdown.cshtml", deals);
    }

    // ── Billing & Invoices ────────────────────────────────────────────────────

    [HttpGet("billing/invoices")]
    public IActionResult Invoices()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var invoices = _context.PaymentTransactions
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedDate)
            .Take(200)
            .ToList();
        return View("~/Views/Accounting/Invoices.cshtml", invoices);
    }

    [HttpGet("billing/invoice-tracking")]
    public IActionResult InvoiceTracking()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var invoices = _context.PaymentTransactions
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedDate)
            .Take(200)
            .ToList();
        return View("~/Views/Accounting/InvoiceTracking.cshtml", invoices);
    }

    // ── Financial Reports ─────────────────────────────────────────────────────

    [HttpGet("reports/dashboard")]
    public IActionResult ReportsDashboard()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");

        ViewBag.TotalRevenue = _context.PaymentTransactions
            .Where(t => t.Status == "succeeded")
            .Sum(t => (decimal?)t.Amount) ?? 0;

        ViewBag.TotalPayouts = _context.CommissionDeals
            .Where(d => d.Status == CommissionDealStatus.Completed)
            .Sum(d => (decimal?)d.AgentPayoutAmount) ?? 0;

        ViewBag.TotalCommissions = _context.CommissionDeals
            .Sum(d => (decimal?)d.GrossCommission) ?? 0;

        ViewBag.PendingPayouts = _context.CommissionDeals
            .Where(d => d.Status == CommissionDealStatus.ApprovedForPayroll || d.Status == CommissionDealStatus.Locked)
            .Sum(d => (decimal?)d.AgentPayoutAmount) ?? 0;

        ViewBag.MonthlyRevenue = _context.PaymentTransactions
            .Where(t => t.Status == "succeeded" && t.CreatedDate.Month == DateTime.UtcNow.Month && t.CreatedDate.Year == DateTime.UtcNow.Year)
            .Sum(t => (decimal?)t.Amount) ?? 0;

        ViewBag.DealCount = _context.CommissionDeals.Count();
        ViewBag.TransactionCount = _context.PaymentTransactions.Count();
        ViewBag.CompletedDealCount = _context.CommissionDeals.Count(d => d.Status == CommissionDealStatus.Completed);

        return View("~/Views/Accounting/ReportsDashboard.cshtml");
    }

    [HttpGet("reports/export-csv")]
    public IActionResult ExportCsv(string reportType)
    {
        var csv = "Type,Date,Amount,Status,Reference\n";

        if (reportType == "revenue" || reportType == "all")
        {
            var transactions = _context.PaymentTransactions.AsNoTracking().OrderByDescending(t => t.CreatedDate).Take(500).ToList();
            foreach (var t in transactions)
            {
                csv += $"Revenue,{t.CreatedDate:yyyy-MM-dd},{t.Amount},{t.Status},{t.PayMongoPaymentIntentId}\n";
            }
        }

        if (reportType == "payouts" || reportType == "all")
        {
            var deals = _context.CommissionDeals.AsNoTracking().Where(d => d.Status == CommissionDealStatus.Completed).OrderByDescending(d => d.CompletedAtUtc).Take(500).ToList();
            foreach (var d in deals)
            {
                csv += $"Payout,{d.CompletedAtUtc:yyyy-MM-dd},{d.AgentPayoutAmount},Completed,Deal #{d.Id}\n";
            }
        }

        if (reportType == "commissions" || reportType == "all")
        {
            var deals = _context.CommissionDeals.AsNoTracking().OrderByDescending(d => d.CreatedAtUtc).Take(500).ToList();
            foreach (var d in deals)
            {
                csv += $"Commission,{d.CreatedAtUtc:yyyy-MM-dd},{d.GrossCommission},{d.Status},Deal #{d.Id}\n";
            }
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
        return File(bytes, "text/csv", $"report_{reportType}_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    // ── Transactions ──────────────────────────────────────────────────────────

    [HttpGet("transactions/history")]
    public IActionResult TransactionHistory()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var transactions = _context.PaymentTransactions
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedDate)
            .Take(200)
            .ToList();
        return View("~/Views/Accounting/TransactionHistory.cshtml", transactions);
    }

    // ── Payment Validation ────────────────────────────────────────────────────

    [HttpGet("payment/validate")]
    public IActionResult PaymentValidation(string? method = null, string? status = null)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var query = _context.PaymentTransactions.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(method))
            query = query.Where(t => t.PaymentMethod == method);

        if (status == "succeeded")
            query = query.Where(t => t.Status == "succeeded");
        else if (status == "pending")
            query = query.Where(t => t.Status != "succeeded" && t.Status != "failed");
        else if (status == "failed")
            query = query.Where(t => t.Status == "failed");

        var transactions = query.OrderByDescending(t => t.CreatedDate).Take(200).ToList();

        ViewBag.MethodFilter = method;
        ViewBag.StatusFilter = status;
        ViewBag.TotalGCash = _context.PaymentTransactions.Where(t => t.PaymentMethod == "gcash" && t.Status == "succeeded").Sum(t => (decimal?)t.Amount) ?? 0;
        ViewBag.TotalBank  = _context.PaymentTransactions.Where(t => t.PaymentMethod == "bank_transfer" && t.Status == "succeeded").Sum(t => (decimal?)t.Amount) ?? 0;
        ViewBag.TotalCash  = _context.PaymentTransactions.Where(t => t.PaymentMethod == "cash" && t.Status == "succeeded").Sum(t => (decimal?)t.Amount) ?? 0;
        ViewBag.PendingCount = _context.PaymentTransactions.Count(t => t.Status != "succeeded" && t.Status != "failed");

        return View("~/Views/Accounting/PaymentValidation.cshtml", transactions);
    }

    [HttpPost("payment/validate/{id:int}/approve")]
    public IActionResult ApprovePayment(int id)
    {
        var transaction = _context.PaymentTransactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null) return NotFound();

        transaction.Status = "succeeded";
        transaction.IsProcessed = true;
        transaction.UpdatedDate = DateTime.UtcNow;
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Payment validated and approved successfully.";
        return RedirectToAction(nameof(PaymentValidation));
    }

    [HttpPost("payment/validate/{id:int}/reject")]
    public IActionResult RejectPayment(int id, [FromForm] string? reason)
    {
        var transaction = _context.PaymentTransactions.FirstOrDefault(t => t.Id == id);
        if (transaction == null) return NotFound();

        transaction.Status = "failed";
        transaction.IsProcessed = false;
        transaction.ErrorMessage = reason ?? "Rejected by accounting";
        transaction.UpdatedDate = DateTime.UtcNow;
        _context.SaveChanges();

        TempData["ErrorMessage"] = "Payment rejected.";
        return RedirectToAction(nameof(PaymentValidation));
    }

    // ── Receipts ──────────────────────────────────────────────────────────────

    [HttpGet("receipts")]
    public IActionResult Receipts(string? search = null)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var query = _context.PaymentTransactions
            .AsNoTracking()
            .Where(t => t.Status == "succeeded");

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Description != null && t.Description.Contains(search) ||
                                     t.PayMongoPaymentIntentId != null && t.PayMongoPaymentIntentId.Contains(search));

        var transactions = query.OrderByDescending(t => t.CreatedDate).Take(200).ToList();
        ViewBag.SearchFilter = search;
        ViewBag.TotalIssued  = transactions.Count;
        return View("~/Views/Accounting/Receipts.cshtml", transactions);
    }

    [HttpGet("receipts/{id:int}/print")]
    public IActionResult PrintReceipt(int id)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var transaction = _context.PaymentTransactions.AsNoTracking().FirstOrDefault(t => t.Id == id);
        if (transaction == null) return NotFound();
        return View("~/Views/Accounting/PrintReceipt.cshtml", transaction);
    }

    // ── Property Revenue Tracking ─────────────────────────────────────

    [HttpGet("property-revenue")]
    public IActionResult PropertyRevenue()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var soldProperties = _context.Properties
            .AsNoTracking()
            .Where(p => p.Status == "Sold" && p.FinalPrice.HasValue)
            .OrderByDescending(p => p.DecisionAt)
            .Take(200)
            .ToList();

        ViewBag.TotalSoldRevenue = soldProperties.Sum(p => p.FinalPrice ?? 0);
        ViewBag.SoldCount        = soldProperties.Count;
        return View("~/Views/Accounting/PropertyRevenue.cshtml", soldProperties);
    }


    [HttpGet("payout-batches")]
    public IActionResult PayoutBatches()
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var batches = _context.CommissionBatches
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAtUtc)
            .Take(200)
            .ToList();
        return View("~/Views/Accounting/PayoutBatches.cshtml", batches);
    }

    [HttpGet("payout-batches/{batchId}")]
    public IActionResult BatchDetails(string batchId)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        batchId = (batchId ?? "").Trim();
        var batch = _context.CommissionBatches.AsNoTracking().FirstOrDefault(b => b.BatchId == batchId);
        if (batch == null) return NotFound();

        var deals = _context.CommissionDeals
            .AsNoTracking()
            .Where(d => d.BatchId == batchId)
            .OrderByDescending(d => d.TransferredToAccountingAtUtc)
            .ToList();

        ViewData["Batch"] = batch;
        return View("~/Views/Accounting/BatchDetails.cshtml", deals);
    }

    [HttpPost("payout-batches/{batchId}/set-status")]
    public IActionResult SetBatchStatus(string batchId, [FromForm] CommissionBatchStatus status)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var batch = _context.CommissionBatches.FirstOrDefault(b => b.BatchId == batchId);
        if (batch == null) return NotFound();

        batch.Status = status;

        var deals = _context.CommissionDeals.Where(d => d.BatchId == batchId).ToList();
        var payout = status switch
        {
            CommissionBatchStatus.AwaitingReview => AccountingPayoutStatus.AwaitingReview,
            CommissionBatchStatus.Processing => AccountingPayoutStatus.Processing,
            CommissionBatchStatus.Released => AccountingPayoutStatus.Released,
            CommissionBatchStatus.Completed => AccountingPayoutStatus.Released,
            _ => AccountingPayoutStatus.AwaitingReview
        };
        foreach (var d in deals)
            d.AccountingPayoutStatus = payout;

        _context.SaveChanges();
        return RedirectToAction(nameof(BatchDetails), new { batchId });
    }

    [HttpPost("payout-deals/{id:int}/set-status")]
    public IActionResult SetDealStatus(int id, [FromForm] AccountingPayoutStatus status)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var deal = _context.CommissionDeals.FirstOrDefault(d => d.Id == id);
        if (deal == null) return NotFound();

        deal.AccountingPayoutStatus = status;
        _context.SaveChanges();
        return RedirectToAction(nameof(BatchDetails), new { batchId = deal.BatchId });
    }

    [HttpPost("payout-deals/{id:int}/reject")]
    public IActionResult RejectDeal(int id, [FromForm] string reason)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var accountingId = HttpContext.Session.GetInt32("UserId");

        var deal = _context.CommissionDeals.FirstOrDefault(d => d.Id == id);
        if (deal == null) return NotFound();

        deal.Status = CommissionDealStatus.RejectedByAccounting;
        deal.AccountingPayoutStatus = AccountingPayoutStatus.AwaitingReview;
        deal.BatchId = null;

        var audit = new Dictionary<string, object?>();
        try { audit = JsonSerializer.Deserialize<Dictionary<string, object?>>(deal.AuditJson) ?? new(); } catch { }
        audit["rejectedAtUtc"] = DateTime.UtcNow;
        audit["rejectedByUserId"] = accountingId;
        audit["rejectReason"] = reason;
        deal.AuditJson = JsonSerializer.Serialize(audit);

        _context.SaveChanges();
        TempData["SuccessMessage"] = "Deal rejected and returned to Manager.";
        return RedirectToAction(nameof(PayoutBatches));
    }

    [HttpPost("payout-batches/{batchId}/complete")]
    public IActionResult CompleteBatch(string batchId)
    {
        ViewData["UserName"] = HttpContext.Session.GetString("UserName");
        var accountingId = HttpContext.Session.GetInt32("UserId") ?? 0;

        var batch = _context.CommissionBatches.FirstOrDefault(b => b.BatchId == batchId);
        if (batch == null) return NotFound();

        var deals = _context.CommissionDeals.Where(d => d.BatchId == batchId).ToList();
        foreach (var d in deals)
        {
            d.Status = CommissionDealStatus.Completed;
            d.CompletedAtUtc = DateTime.UtcNow;
            d.AccountingReleasedAtUtc = d.AccountingReleasedAtUtc ?? DateTime.UtcNow;
            d.AccountingReleasedByUserId = accountingId;
            d.PayStubUrl = d.PayStubUrl ?? $"/accounting/payout-stubs/{batchId}/{d.Id}";

            var audit = new Dictionary<string, object?>();
            try { audit = JsonSerializer.Deserialize<Dictionary<string, object?>>(d.AuditJson) ?? new(); } catch { }
            audit["completedAtUtc"] = d.CompletedAtUtc;
            audit["completedByUserId"] = accountingId;
            audit["batchId"] = batchId;
            d.AuditJson = JsonSerializer.Serialize(audit);
        }

        batch.Status = CommissionBatchStatus.Completed;
        batch.CompletedAtUtc = DateTime.UtcNow;
        batch.CompletedByUserId = accountingId;

        _context.SaveChanges();
        TempData["SuccessMessage"] = $"Batch {batchId} marked completed.";
        return RedirectToAction(nameof(PayoutBatches));
    }

    // ── Settings / Profile ────────────────────────────────────────────────────

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Settings.cshtml");
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var userName = HttpContext.Session.GetString("UserName");
        ViewData["UserName"] = userName;
        return View("~/Views/Accounting/Profile.cshtml");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Admin");
    }
}
