

using support.Domain;

namespace support.Service
{
    public interface IUserManageService
    {
        Task<ApiDataResponse<string>> AccessSystem(AccessDetails request);
        Task<ApiDataResponse<string>> RefreshAccessToken(Guid Id);
        Task<List<Ticket>> GetAllSupportTickets(Guid userId);
        Task<ApiResponse> CreateTicket(CreateTicket request);
        Task UpdateSupportTerms(Guid Id);
    }
}