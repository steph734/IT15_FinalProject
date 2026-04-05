using RealEstate.Models;
namespace RealEstate.Services;

public class InquiryService
{
    private readonly List<PropertyInquiry> _inquiries;

    public InquiryService()
    {
        _inquiries = new List<PropertyInquiry>
        {
            new PropertyInquiry { Id = 1, CustomerName = "Juan Dela Cruz", CustomerEmail = "juan@example.ph", PropertyId = 1, Message = "Is this still available?", Status = InquiryStatus.New, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new PropertyInquiry { Id = 2, CustomerName = "Anna Smith", CustomerEmail = "anna@example.com", PropertyId = 3, Message = "Can I schedule a viewing?", Status = InquiryStatus.InProgress, CreatedAt = DateTime.UtcNow.AddDays(-1) }
        };
    }

    public IReadOnlyList<PropertyInquiry> GetAll() => _inquiries.AsReadOnly();

    public IReadOnlyList<PropertyInquiry> GetForProperty(int propertyId) => _inquiries.Where(i => i.PropertyId == propertyId).OrderByDescending(i => i.CreatedAt).ToList();

    public PropertyInquiry? GetById(int id) => _inquiries.FirstOrDefault(i => i.Id == id);

    public PropertyInquiry Add(PropertyInquiry inq)
    {
        inq.Id = _inquiries.Count == 0 ? 1 : _inquiries.Max(i => i.Id) + 1;
        inq.CreatedAt = DateTime.UtcNow;
        _inquiries.Insert(0, inq);
        return inq;
    }

    public bool UpdateStatus(int id, InquiryStatus status)
    {
        var idx = _inquiries.FindIndex(i => i.Id == id);
        if (idx < 0) return false;
        var q = _inquiries[idx];
        q.Status = status;
        _inquiries[idx] = q;
        return true;
    }

    public bool AddReply(int id, string reply)
    {
        var idx = _inquiries.FindIndex(i => i.Id == id);
        if (idx < 0) return false;
        var q = _inquiries[idx];
        q.Reply = reply;
        _inquiries[idx] = q;
        return true;
    }
}
