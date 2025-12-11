namespace JWT_Project_Core.DTO
{
    public class DashboardDto
    {
        public double TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public List<double> RevenueByMonth { get; set; } = new();
        public List<int> ProductsByMonth { get; set; } = new();
        public List<int> OrdersByMonth { get; set; } = new();
    }
}
