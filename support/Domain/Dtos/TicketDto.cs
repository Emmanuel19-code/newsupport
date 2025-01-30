namespace support.Domain
{
  public class CreateTicket
  {
    public string TicketTitle {get;set;}
    public Guid CreatedBy {get;set;}
  }
  
  public class Ticket
  {
        public Guid Id { get; set; }
        public required string ConversationTitle { get; set; }
        public string ConversationStatus { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
  

  public class CreateConversationRequest
  {
    public Guid CreatorId {get;set;}
    public Guid SystemAdminId {get;set;}
  }

}