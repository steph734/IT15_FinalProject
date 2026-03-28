namespace RealEstate.Models;

public class PropertyDetailsViewModel
{
    public Property Property { get; set; } = null!;
    public Agent? Agent { get; set; }
    public ScheduleViewingViewModel Schedule { get; set; } = new();
}
