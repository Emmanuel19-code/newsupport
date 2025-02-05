namespace support.Domain
{
  public class Message
{
    public Guid Id { get; set; }
     public Guid ConversationId { get; set; }
    public string? MessageIssue { get; set; }
    public string? ImageAttachment { get; set; } 
    public required string SentBy {get;set;}
    public DateTime CreatedAt { get; set; }
   
}

}