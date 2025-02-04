namespace support.Domain
{
    public class Notification
    {
        public Guid Id {get;set;}
        public required string Title {get;set;}
        public required string Message {get;set;}
        public string? Attachement {get;set;}
        public string Status {get;set;} = "Unread";
        public DateTime CreatedAt {get;set;}
    }
}