using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Model;
using JWT_Project_Core.Model.Human;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWT_Project_Core.Service
{
   

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper , IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<object?> LoginAsync(LoginDTO login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password)) return null;

            var accessToken = GenerateAccessToken(user);

            var refreshToken = GenerateRefreshToken();
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                Username = user.Username,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            await _context.SaveChangesAsync();

            return new
            {
                accessToken,
                refreshToken,
                username = user.Username,
                role = user.Role.ToString()
            };
        }

        private string GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<string?> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.Expires > DateTime.UtcNow);

            if (tokenEntity == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == tokenEntity.Username);
            if (user == null) return null;

            return GenerateAccessToken(user);
        }

        public async Task<bool> RemoveRefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (tokenEntity == null) return false;

            _context.RefreshTokens.Remove(tokenEntity);
            await _context.SaveChangesAsync();
            return true;
        }

       
        public async Task<string> RegisterAsync(RegisterDTO newUser)
        {
            try
            {
                string username = newUser.Username.Trim();
                string email = newUser.Email.Trim();

                var user = await _context.Users
                                    .IgnoreQueryFilters()
                                    .FirstOrDefaultAsync(u => u.Username == newUser.Username);
                
                if (user != null && !user.IsDeleted)
                    return "Username already exists";

              
                if (await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted))
                    return "Email already exists";

                
                if (user != null && user.IsDeleted)
                {
                    user.Email = email;
                    user.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                    user.Role = EnumRole.Customer;
                    user.IsDeleted = false;
                    user.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return "User restored successfully";
                }

               
                var newEntity = new User
                {
                    Username = username,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                    Role = EnumRole.Customer,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.Users.Add(newEntity);
                await _context.SaveChangesAsync();

                return "User created successfully";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RegisterAsync error");
                return "An error occurred while creating user";
            }
        }
        public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

            if (user == null)
                return "Email not found";

           
            string newPassword = GenerateRandomPassword();

            
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            
            await _emailService.SendEmailAsync(
                email,
                "Your New Password",
                $"<h3>Your password has been reset</h3><p>New Password: <b>{newPassword}</b></p>"
            );

            return "New password has been sent to your email";
        }

        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public async Task<string> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

            if (user == null)
                return "User not found";

          
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return "Old password is incorrect";

           
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return "Password updated successfully";
        }


    }

}
