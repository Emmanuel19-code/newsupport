namespace support.Domain
{
  public class Message
{
    public Guid Id { get; set; }
    public string MessageIssue { get; set; }
    public string? ImageAttachment { get; set; } // Nullable if no image is attached
    public DateTime CreatedAt { get; set; }
    public Guid ConversationId { get; set; }
}

}