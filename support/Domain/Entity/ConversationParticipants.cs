using System.ComponentModel.DataAnnotations;

namespace support.Domain{
    public class ConversationParticipants
{
    public Guid Id { get; set; }
    public string TicketCreator { get; set; }
    public required string AdminUserName  { get; set; }
    public Guid ConversationId { get; set; }
}

}