using System.Reflection;
using Microsoft.EntityFrameworkCore;
using support.Domain;
using support.Infrastructure;

namespace support.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        
        public MessageService(
            ApplicationDbContext context
        )
        {
            _context = context;
        }
        public async Task<List<MessageResponse>> ConversationMessages(Guid conversationId)
        {
            var messages = await _context.Messages.Where(m=>m.ConversationId == conversationId).ToListAsync();
            var message = messages.Select(message=>new MessageResponse
            {
                Id = message.Id,
                MessageIssue = message.MessageIssue,
                CreatedAt = message.CreatedAt,
                ImageAttachment = message.ImageAttachment,
            }).ToList();
            return message;
        }

         public async Task<ApiResponse> SendMessage(SendMessage request)
        {
            string filePath =await FileUploadHandler(request.ImageAttachment,request.DocumentAttachment);
            var message = new Message
            {
                ConversationId = request.ConversationId,
                MessageIssue = request.Message,
                SentBy = request.SentBy,
                ImageAttachment = filePath
            };
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return new ApiResponse{
                 Success = true,
                 Message = $"Delivered {filePath}",
                 StatusCode = 201
            };
        }

        private async Task<string> FileUploadHandler(IFormFile imagefile,IFormFile documentfile)
        {
            string filePath = string.Empty;
          if(imagefile != null)
          {
            var imageFolderPath = Path.Combine("Upload","images");
            Directory.CreateDirectory(imageFolderPath);
            var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagefile.FileName);
            filePath = Path.Combine(imageFolderPath,imageFileName);
            using (var stream = new FileStream(filePath,FileMode.Create))
            {
              await  imagefile.CopyToAsync(stream);
            }
          }

          if(documentfile != null)
          {
            var documentFolderPath = Path.Combine("Upload","documents");
            Directory.CreateDirectory(documentFolderPath);
            var documentFileName = Guid.NewGuid().ToString() + Path.GetExtension(documentfile.FileName);
            filePath = Path.Combine(documentFolderPath,documentFileName);
            using (var stream = new FileStream(filePath,FileMode.Create))
            {
               await documentfile.CopyToAsync(stream);
            }
          }
          return filePath;
        }
    }
}