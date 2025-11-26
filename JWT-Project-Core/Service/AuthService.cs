using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Model.Human;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Project_Core.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<string?> LoginAsync(LoginDTO login)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == login.Username && u.Password == login.Password);

            if (user == null)
            {
                Log.Warning("Sai ten dang nhap hoac mat khau !");
                return null;
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            Log.Information("Login success , {Username} , {role} , {token}", login.Username, user.Role, tokenHandler.WriteToken(token));
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> RegisterAsync(RegisterDTO newUser)
        {
            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
            {
                Log.Warning("Username already exists");
                return "Username already exists";
            }


            var user = _mapper.Map<User>(newUser);
            user.Role = EnumRole.Customer;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            Log.Information("User created successfully!");
            return "User created successfully";
        }
    }
}
