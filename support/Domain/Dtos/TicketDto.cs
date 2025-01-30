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

}