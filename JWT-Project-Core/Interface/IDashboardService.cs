using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
    // city of love
}
