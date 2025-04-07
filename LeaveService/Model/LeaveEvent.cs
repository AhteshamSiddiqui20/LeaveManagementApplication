namespace LeaveService.Model
{
    public class LeaveEvent
    {
        public string FromUser { get; set; } = string.Empty;
        public string ToUser { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
