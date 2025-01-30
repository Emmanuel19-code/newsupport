using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using support.Domain;
using support.Infrastructure;

namespace support.Service
{
    public class SystemAdmin : ISystemAdmin
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public SystemAdmin(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        //Creating A New system Admin
        public async Task<ApiDataResponse<CreatedResponse>> AddSystemAdmin(NewSystemAdmin request)
        {
            if (request == null)
            {
                return new ApiDataResponse<CreatedResponse>
                {
                    Data = null,
                    Success = false,
                    StatusCode = 400,
                    Message = "Please Provide Missing Information"
                };
            }
            var emailExist = await _context.SystemAdmins.FirstOrDefaultAsync(e => e.AdminUserEmail == request.AdminUserEmail);
            if (emailExist != null)
            {
                return new ApiDataResponse<CreatedResponse>
                {
                    Data = null,
                    Success = false,
                    StatusCode = 400,
                    Message = "Your Account is Already Set"
                };
            }
            string userName;
            userName = GenerateUserName(request.AssignedTo);
            var userNameExist = await _context.SystemAdmins.FirstOrDefaultAsync(e => e.AdminUserName == userName);
            var AdminPass = GeneratePassword();
            var hashpass = new PasswordHasher<SystemAdmin>().HashPassword(null, AdminPass);
            while (userNameExist != null)
            {
                userName = GenerateUserName(request.AssignedTo);
                userNameExist = await _context.SystemAdmins.FirstOrDefaultAsync(e => e.AdminUserName == userName);
            }
            var addAdmin = new SystemAdminDb
            {
                AdminUserName = userName,
                AdminUserEmail = request.AdminUserEmail,
                AdminPassword = hashpass,
                AssignedTo = request.AssignedTo,
                Role = request.Role == "Admin" ? "Admin" : "SuperAdmin",
            };
            await _context.SystemAdmins.AddAsync(addAdmin);
            await _context.SaveChangesAsync();
            return new ApiDataResponse<CreatedResponse>
            {
                Data = new CreatedResponse
                {
                    AdminUserName = userName,
                    AdminPassword = AdminPass,
                },
                Success = true,
                StatusCode = 201,
                Message = "System Admin Created Successfully"
            };
        }
        //Accessing The System
        public async Task<ApiDataResponse<string>> AccessSystem(AccessRequest request)
        {
            if (request == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Please Provide Information",
                    StatusCode = 400
                };
            }
            var userExist = await _context.SystemAdmins.FirstOrDefaultAsync(e => e.AdminUserName == request.AdminUserName);
            if (userExist == null)
            {

                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "User Not Found",
                    StatusCode = 404
                };
            }
            if (new PasswordHasher<SystemAdminDb>().VerifyHashedPassword(null, userExist.AdminPassword, request.AdminPassword) == PasswordVerificationResult.Failed)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Wrong Credentials",
                    StatusCode = 401
                };
            }
            var token = CreateToken(userExist);
            await GenerateRefreshTokenAsync(userExist.Id);
            return new ApiDataResponse<string>
            {
                Data = token,
                Success = true,
                Message = "Login Successful",
                StatusCode = 200
            };
        }
        //Reseting Admin Password
        public async Task<ApiDataResponse<string>> ResetAdminPassword(string AdminUserName)
        {
            if (AdminUserName == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Provide AdminUserName",
                    StatusCode = 400
                };
            }
            var admin = await _context.SystemAdmins.FirstOrDefaultAsync(a => a.AdminUserName == AdminUserName);
            if (admin == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Not An Admin",
                    StatusCode = 400
                };
            }
            var newpassword = GeneratePassword();
            var hashpass = new PasswordHasher<SystemAdminDb>().HashPassword(null, newpassword);
            admin.AdminPassword = hashpass;
            await _context.SaveChangesAsync();
            return new ApiDataResponse<string>
            {
                Data = newpassword,
                Success = true,
                Message = "Password Has Been Reset",
                StatusCode = 201
            };
        }

        //Getting All Registered Admins
        public async Task<ApiDataResponse<List<SystemAdmins>>> AllAdmin()
        {
            var systemAdminsDb = await _context.SystemAdmins.ToListAsync();
            var systemAdmins = _mapper.Map<List<SystemAdmins>>(systemAdminsDb);
            return new ApiDataResponse<List<SystemAdmins>>
            {
                Success = true,
                Data = systemAdmins,
                StatusCode = 200
            };
        }

        //Start Conversation
        public async Task<ApiResponse> StartConversation(CreateConversationRequest request, Guid TicketId)
        {
            var ticket = await _context.Tickets.FindAsync(TicketId);
            if (ticket == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Could not find Ticket",
                    StatusCode = 400
                };
            }
            var ticket_conversation = await _context.Participants.FindAsync(TicketId);
            if (ticket_conversation != null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "PLease Check Your Inbox For this Conversation",
                    StatusCode = 400
                };
            }
            var conversationParticipants = new ConversationParticipants
            {
                TicketCreator = request.TicketCreator,
                AdminUserName = request.AdminUserName,
                ConversationId = TicketId
            };
            await _context.Participants.AddAsync(conversationParticipants);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Success = true,
                Message = "Conversation Created",
                StatusCode = 201
            };
        }
        public async Task<List<Ticket>> AllReceivedTickets()
        {
            var tickets = await _context.Tickets.ToListAsync();
            var ticketList = tickets.Select(ticket => new Ticket
            {
                Id = ticket.Id,
                ConversationStatus = ticket.ConversationStatus,
                ConversationTitle = ticket.ConversationTitle,
                CreatedBy = ticket.CreatedBy,
                CreatedAt = ticket.CreatedAt,
            }).ToList();
            return ticketList;
        }
        public async Task<ApiResponse> AssignToAdmin(AssignedToAdmin request)
        {
            if (request == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Please provide the right Information",
                    StatusCode = 400
                };
            }

            var ticket = await _context.Tickets.FindAsync(request.TicketId);
            if (ticket == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Ticket Not Available",
                    StatusCode = 404
                };
            }
            if (!string.IsNullOrEmpty(ticket.AssignedTo))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Ticket Has already been Assigned to {ticket.AssignedTo}",
                    StatusCode = 404
                };
            }
            ticket.AssignedTo = request.AdminUserName;
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Success = true,
                Message = "Ticket successfully assigned",
                StatusCode = 200
            };
        }
        public async Task<List<Ticket>> MyAssignedTicket(string AdminUserName)
        {
            var assignedTickets = await _context.Tickets
                                                .Where(ticket => ticket.AssignedTo == AdminUserName)
                                                .ToListAsync();
            var assigned = assignedTickets.Select(assigned=> new Ticket{
                Id = assigned.Id,
                ConversationTitle = assigned.ConversationTitle,
                ConversationStatus = assigned.ConversationStatus,
                CreatedBy = assigned.CreatedBy,
                CreatedAt = assigned.CreatedAt
            }).ToList();
            return assigned;
        }
         public async Task<ApiDataResponse<List<CompanyProfile>>>  GetSupportingCompanies()
        {
            var companies = await _context.Users.ToListAsync();
            var companyProfile = _mapper.Map<List<CompanyProfile>>(companies);
            return new ApiDataResponse<List<CompanyProfile>>
            {
                Data = companyProfile,
                Success = true,
            };
        }
        public async Task<ApiResponse> TerminateSupport(Guid companyId)
        {
            if(companyId == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Please provide details",
                    StatusCode = 400
                };
            }
            var company = await _context.Users.FindAsync(companyId);
            if(company == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "No Match Found",
                    StatusCode  = 404
                };
            }
            if(company.Supporting == false)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Support Already Terminated",
                    StatusCode = 400
                };
            }
            company.Supporting = false;
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Success = true,
                Message = "Support Terminated",
                StatusCode = 200
            };
        }
        public async Task<ApiResponse> RenewSupport(Guid companyId)
        {
            if(companyId == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Please provide details",
                    StatusCode = 400
                };
            }
            var company = await _context.Users.FindAsync(companyId);
            if(company == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "No Match Found",
                    StatusCode  = 404
                };
            }
            if(company.Supporting == true)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Already Renewed",
                    StatusCode = 400
                };
            }
            company.Supporting = true;
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Success = true,
                Message = "Support Terminated",
                StatusCode = 200
            };
        }
        //Function To Generate Password
        private string GeneratePassword()
        {
            char[] PasswordCharacters =
       "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[]{}|;:,<>?".ToCharArray();
            var random = new Random();
            var randomPassword = new string(Enumerable.Range(1, 12)
                                                       .Select(_ => PasswordCharacters[random.Next(PasswordCharacters.Length)])
                                                       .ToArray());
            return randomPassword;
        }
        //Function To generate UserName
        private string GenerateUserName(string AssignedTo)
        {
            var random = new Random();
            var randomNumber = random.Next(1000, 9999);
            var splitName = AssignedTo.Split(" ")[0];
            return $"{splitName}{randomNumber}";
        }

        //Function To Generate RefreshToken
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }
        //Function to Set the RefreshToken
        private async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var refreshToken = GenerateRefreshToken();
            var usertoken = await _context.SystemAdmins.FirstOrDefaultAsync(rt => rt.Id == userId);
            usertoken.RefreshToken = refreshToken;
            usertoken.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        //Creating the AccessToken
        private string CreateToken(SystemAdminDb systemAdmin)
        {
            Console.WriteLine($"this is details {systemAdmin}");
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name,systemAdmin.AdminUserName),
                new Claim(ClaimTypes.NameIdentifier,systemAdmin.Id.ToString()),
                new Claim(ClaimTypes.Role,systemAdmin.Role)
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
               issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
               audience: _configuration.GetValue<string>("AppSettings:Audience"),
               claims: claims,
               expires: DateTime.UtcNow.AddDays(1),
               signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        
    }
}

