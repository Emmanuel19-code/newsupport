using Microsoft.AspNetCore.Mvc;
using support.Domain;
using support.Services;

namespace support.Controllers
{
    [Route("/api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send_message")]
        public async Task<ApiResponse> RespondToSupport(SendMessage request)
        {
            var response = await  _messageService.SendMessage(request);
            return response;
        }

        [HttpPost("messages/{conversationId}")]
        public async Task<List<MessageResponse>> Messages(Guid conversationId)
        {
            var response = await  _messageService.ConversationMessages(conversationId);
            return response;
        }
    }
}