namespace support.Domain
{
    public class Tickets
{
    public Guid Id { get; set; }
    public required string ConversationTitle { get; set; }
    public string ConversationStatus { get; set; } = "Open";
    public  string CreatedBy { get; set; }
    public required string CompanyName {get;set;}
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
}

}