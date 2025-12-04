using JWT_Project_Core.Interface;
using System.Security.Claims;

namespace JWT_Project_Core.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUsername()
        {
          
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity!.IsAuthenticated)
            {
               
                return null;
            }

            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
    }
