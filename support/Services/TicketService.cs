using support.Domain;

namespace support.Service
{
    public class TicketService : ITicketService
    {
        public Task AllTickets(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> CreateTicket(CreateTicket request, Guid TicketId)
        {
            throw new NotImplementedException();
        }
    }
}