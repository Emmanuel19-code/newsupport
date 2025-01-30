using support.Domain;

namespace support.Service
{
    public interface ITicketService
    {
        Task<ApiResponse> CreateTicket(CreateTicket request,Guid TicketId);
        Task AllTickets(Guid Id);
    }
}