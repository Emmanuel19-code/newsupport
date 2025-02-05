namespace support.Domain
{
  public class CreateTicket
  {
    public string TicketTitle {get;set;}
    public string CreatedBy {get;set;}
    public string CompanyName {get;set;}
  }
  
  public class Ticket
  {
        public Guid Id { get; set; }
        public required string ConversationTitle { get; set; }
        public required string ConversationStatus { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
  

  public class CreateConversationRequest
  {
   public required string TicketCreator { get; set; }
  public required string AdminUserName  { get; set; }
  }

 public class SendMessage 
 {
   public Guid ConversationId {get;set;}
   public string? Message {get;set;}
   public required string SentBy {get;set;}
   public IFormFile? ImageAttachment {get;set;}
   public IFormFile? DocumentAttachment {get;set;}
 }
 public class MessageResponse
 {
   public Guid Id {get;set;}
   public string MessageIssue {get;set;}
   public string SentBy {get;set;}
   public string ImageAttachment {get;set;}
   public DateTime CreatedAt {get;set;}
 }
}