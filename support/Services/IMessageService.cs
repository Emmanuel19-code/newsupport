using support.Domain;

namespace support.Services
{
    public interface IMessageService 
    {
        Task<ApiResponse> SendMessage(SendMessage request);
        Task<List<MessageResponse>> ConversationMessages(Guid conversationId);
    }
}