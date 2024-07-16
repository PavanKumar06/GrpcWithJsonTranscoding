namespace GrpcService.Models
{
    public class NotificationItem
    {
        public int Id { get; set; }

        public string? ToDoTitle { get; set; }

        public string? Notification { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
