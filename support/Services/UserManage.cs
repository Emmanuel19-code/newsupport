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
    public class UserManageService : IUserManageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserManageService(ApplicationDbContext context, IConfiguration configuration,IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<ApiResponse> AddUser(AddUserDto request)
        {
            if (request == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Please Provide the appropriate Information",
                    StatusCode = 400
                };
            }
            var company = await _context.Users.FirstOrDefaultAsync(c => c.CompanyName == request.CompanyName);
            if (company != null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Already Supporting",
                    StatusCode = 400
                };
            }
            var password = GeneratePassword();
            var hashpass = new PasswordHasher<Users>().HashPassword(null, password);
            var username = GenerateUserName(request.CompanyName);
            var existingUser = await _context.Users.FirstOrDefaultAsync(e => e.UserName == username);

            if (existingUser != null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Username is in use",
                    StatusCode = 400
                };
            }

            var expiryDate = CalculateExpiryData(request.SupportMonth);

            var user = new Users
            {
                CompanyName = request.CompanyName,
                Password = hashpass,
                UserName = username,
                SupportExpiry = expiryDate,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                Success = true,
                StatusCode = 200,
                Message = $"Successfully Added {password}"
            };
        }

        public async Task<ApiDataResponse<string>> AccessSystem(AccessDetails request)
        {
            if (request == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Please Provide Missing Information",
                    StatusCode = 400
                };
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "User not available",
                    StatusCode = 404
                };
            }
            if (new PasswordHasher<Users>().VerifyHashedPassword(null, user.Password, request.Password) == PasswordVerificationResult.Failed)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Authentication Failded",
                    StatusCode = 400
                };
            }
            string token = CreateToken(user);
            await GenerateRefreshTokenAsync(user.Id);
            if (token == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Authentication Failded Try again",
                    StatusCode = 400

                };
            }

            return new ApiDataResponse<string>
            {
                Data = token,
                Success = true,
                Message = "Authentication Successful",
                StatusCode = 200
            };

        }
        public async Task<ApiDataResponse<string>> RefreshAccessToken(Guid Id)
        {
            if (Id == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Missing Information",
                    StatusCode = 400,

                };
            }
            var usertoken = await _context.RefreshTokenDb.FirstOrDefaultAsync(u => u.UserId == Id);
            if (usertoken == null)
            {
                return new ApiDataResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Sign In",
                    StatusCode = 400
                };
            }
            var refreshToken = GenerateRefreshToken();
            usertoken.RefreshToken = refreshToken;
            usertoken.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return new ApiDataResponse<string>
            {
                Data = refreshToken,
                Success = true,
                Message = "Refresh Token Updated",
                StatusCode = 200
            };
        }
        public Task<List<Ticket>> GetAllSupportTickets(Guid userId)
        {
            throw new NotImplementedException();
        }
        public Task UpdateSupportTerms(Guid Id)
        {
            throw new NotImplementedException();
        }

        private string GeneratePassword()
        {
            char[] PasswordCharacters =
       "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+[]{}|;:,.<>?".ToCharArray();
            var random = new Random();
            var randomPassword = new string(Enumerable.Range(1, 12)
                                                       .Select(_ => PasswordCharacters[random.Next(PasswordCharacters.Length)])
                                                       .ToArray());
            return randomPassword;
        }

        private string GenerateUserName(string companyName)
        {
            var random = new Random();
            var randomNumber = random.Next(1000, 9999);
            return $"{companyName}{randomNumber}";
        }

        private DateTime CalculateExpiryData(int period)
        {
            return DateTime.Now.AddMonths(period);
        }
        private string CreateToken(Users user)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
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

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }
        private async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var refreshToken = GenerateRefreshToken();
            var usertoken = await _context.RefreshTokenDb.FirstOrDefaultAsync(rt => rt.UserId == userId);

            if (usertoken == null)
            {

                usertoken = new RefreshTokenDb
                {
                    //Id = Guid.NewGuid(),
                    UserId = userId,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
                };

                await _context.RefreshTokenDb.AddAsync(usertoken);
            }
            else
            {

                usertoken.RefreshToken = refreshToken;
                usertoken.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            }

            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<ApiResponse> CreateTicket(CreateTicket request)
        {
            if(request == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Please Provide Details",
                    StatusCode = 400

                };
            }
            var ticket = new Tickets
             {
                ConversationTitle = request.TicketTitle,
                CreatedBy = request.CreatedBy,
                CompanyName = request.CompanyName
             };
             await _context.Tickets.AddAsync(ticket);
             await _context.SaveChangesAsync();
             return new ApiResponse
             {
                Success = true,
                Message = "Created",
                StatusCode = 201
             };
        }
    }
}