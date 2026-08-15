namespace TuckBoxApp.Models;

public class TimeSlot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DisplayText { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}
