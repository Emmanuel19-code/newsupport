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
                Role = request.Role == "User"?"User":"SuperAdmin",
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
            var ticket = await _context.Conversations.FindAsync(TicketId);
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
                UserId = request.CreatorId,
                AdminId = request.SystemAdminId,
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

        public Task<ApiResponse> AssignToAdmin(string TicketId)
        {
            throw new NotImplementedException();
        }
    }
}

