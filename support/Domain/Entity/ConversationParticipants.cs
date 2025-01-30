using System.ComponentModel.DataAnnotations;

namespace support.Domain{
    public class ConversationParticipants
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
    public Guid ConversationId { get; set; }
}

}