namespace support.Domain
{
    public class Conversation
{
    public Guid Id { get; set; }
    public required string ConversationTitle { get; set; }
    public string ConversationStatus { get; set; } = "Open";
    public  Guid CreatedBy { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
}

}