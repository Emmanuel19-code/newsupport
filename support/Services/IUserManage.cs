

using support.Domain;

namespace support.Service
{
    public interface IUserManageService
    {
        Task<ApiResponse> AddUser(AddUserDto request);
        Task<ApiDataResponse<string>> AccessSystem(AccessDetails request);
        Task<ApiDataResponse<string>> RefreshAccessToken(Guid Id);
        Task<ApiDataResponse<List<UserProfile>>> GetSupportingCompanies();
        Task<List<Ticket>> GetAllSupportTickets(Guid userId);
        Task<ApiResponse> CreateTicket(CreateTicket request);
        Task UpdateSupportTerms(Guid Id);
    }
}