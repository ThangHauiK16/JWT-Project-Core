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
    //public class AuthService : IAuthService
    //{
    //    private readonly ApplicationDbContext _context;
    //    private readonly IConfiguration _configuration;
    //    private readonly IMapper _mapper;

    //    public AuthService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
    //    {
    //        _context = context;
    //        _configuration = configuration;
    //        _mapper = mapper;
    //    }
    //    public async Task<object?> LoginAsync(LoginDTO login)
    //    {
    //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
    //        if (user == null)
    //        {
    //            Log.Warning("Sai ten dang nhap hoac mat khau !");
    //            return null;
    //        }

    //        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
    //        if (!isPasswordValid)
    //        {
    //            Log.Warning("Sai mat khau!");
    //            return null;
    //        }

    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var jwtSettings = _configuration.GetSection("Jwt");
    //        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(new Claim[]
    //            {
    //        new Claim(ClaimTypes.Name, user.Username),
    //        new Claim(ClaimTypes.Role, user.Role.ToString())
    //            }),
    //            Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"]!)),
    //            Issuer = jwtSettings["Issuer"],
    //            Audience = jwtSettings["Audience"],
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        var accessToken = tokenHandler.WriteToken(token);

    //        // tạo refresh token
    //        var refreshToken = GenerateRefreshToken();
    //        _context.RefreshTokens.Add(new RefreshToken
    //        {
    //            Token = refreshToken,
    //            Username = user.Username,
    //            Expires = DateTime.UtcNow.AddDays(7) // refresh token 7 ngày
    //        });
    //        await _context.SaveChangesAsync();

    //        Log.Information("Login success , {Username} , {role} , {token}", login.Username, user.Role, accessToken);

    //        return new
    //        {
    //            accessToken,
    //            refreshToken,
    //            username = user.Username,
    //            role = user.Role.ToString()
    //        };
    //    }
    //    //public async Task<string?> LoginAsync(LoginDTO login)
    //    //{
    //    //    var user = await _context.Users
    //    //        .FirstOrDefaultAsync(u => u.Username == login.Username );

    //    //    if (user == null)
    //    //    {
    //    //        Log.Warning("Sai ten dang nhap hoac mat khau !");
    //    //        return null;
    //    //    }
    //    //    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

    //    //    if (!isPasswordValid)
    //    //    {
    //    //        Log.Warning("Sai mat khau!");
    //    //        return null;
    //    //    }

    //    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    //    var jwtSettings = _configuration.GetSection("Jwt");
    //    //    var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

    //    //    var tokenDescriptor = new SecurityTokenDescriptor
    //    //    {
    //    //        Subject = new ClaimsIdentity(new Claim[]
    //    //        {
    //    //            new Claim(ClaimTypes.Name, user.Username),
    //    //            new Claim(ClaimTypes.Role, user.Role.ToString())
    //    //        }),
    //    //        Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"]!)),
    //    //        Issuer = jwtSettings["Issuer"],
    //    //        Audience = jwtSettings["Audience"],
    //    //        SigningCredentials = new SigningCredentials(
    //    //            new SymmetricSecurityKey(key),
    //    //            SecurityAlgorithms.HmacSha256Signature)
    //    //    };

    //    //    var token = tokenHandler.CreateToken(tokenDescriptor);
    //    //    Log.Information("Login success , {Username} , {role} , {token}", login.Username, user.Role, tokenHandler.WriteToken(token));
    //    //    return tokenHandler.WriteToken(token);
    //    //}
    //    private string GenerateRefreshToken()
    //    {
    //        var randomBytes = new byte[64];
    //        RandomNumberGenerator.Fill(randomBytes); 
    //        return Convert.ToBase64String(randomBytes);
    //    }

    //    public async Task<string?> RefreshTokenAsync(string refreshToken)
    //    {
    //        var tokenEntity = await _context.RefreshTokens
    //            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.Expires > DateTime.UtcNow);

    //        if (tokenEntity == null)
    //            return null;

    //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == tokenEntity.Username);
    //        if (user == null) return null;

    //        // tạo access token mới
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var jwtSettings = _configuration.GetSection("Jwt");
    //        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(new Claim[]
    //            {
    //        new Claim(ClaimTypes.Name, user.Username),
    //        new Claim(ClaimTypes.Role, user.Role.ToString())
    //            }),
    //            Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"]!)),
    //            Issuer = jwtSettings["Issuer"],
    //            Audience = jwtSettings["Audience"],
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        return tokenHandler.WriteToken(token);
    //    }

    //    public async Task<bool> RemoveRefreshTokenAsync(string refreshToken)
    //    {
    //        var tokenEntity = await _context.RefreshTokens
    //            .FirstOrDefaultAsync(t => t.Token == refreshToken);

    //        if (tokenEntity == null) return false;

    //        _context.RefreshTokens.Remove(tokenEntity);
    //        await _context.SaveChangesAsync();

    //        return true;
    //    }


    //    public async Task<string> RegisterAsync(RegisterDTO newUser)
    //    {
    //        if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
    //        {
    //            Log.Warning("Username already exists");
    //            return "Username already exists";
    //        }


    //        var user = _mapper.Map<User>(newUser);

    //        user.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
    //        user.Role = EnumRole.Customer;

    //        _context.Users.Add(user);
    //        await _context.SaveChangesAsync();
    //        Log.Information("User created successfully!");
    //        return "User created successfully";
    //    }
    //}

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
            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
                return "Username already exists";

            var user = _mapper.Map<User>(newUser);
            user.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            user.Role = EnumRole.Customer;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return "User created successfully";
        }
    }

}
