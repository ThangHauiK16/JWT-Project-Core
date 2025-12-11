using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace JWT_Project_Core.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _db;

        public DashboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var dto = new DashboardDto();

            // Lọc đơn đã duyệt
            var approvedOrders = _db.Orders
                    .Where(o => o.TrangThai == EnumStatus.success);

            // Tổng đơn hàng đã duyệt
            dto.TotalOrders = await approvedOrders.CountAsync();

            // Tổng sản phẩm đã bán (thuộc đơn đã duyệt)
            dto.TotalProductsSold = await _db.OrderBooks
                .Include(ob => ob.HoaDon)
                .Where(ob => ob.HoaDon!.TrangThai == EnumStatus.success)
                .SumAsync(ob => ob.SoLuong);

            // Tổng doanh thu (chỉ tính đơn đã duyệt)
            dto.TotalRevenue = await _db.OrderBooks
                .Include(ob => ob.HoaDon)
                .Include(ob => ob.Sach)
                .Where(ob => ob.HoaDon!.TrangThai == EnumStatus.success)
                .SumAsync(ob => ob.SoLuong * ob.Sach!.GiaBan);

            // Theo tháng (12 tháng)
            for (int month = 1; month <= 12; month++)
            {
                // Doanh thu theo tháng
                double monthlyRevenue = await _db.OrderBooks
                    .Include(ob => ob.HoaDon)
                    .Include(ob => ob.Sach)
                    .Where(ob =>
                        ob.HoaDon!.TrangThai == EnumStatus.success &&
                        ob.HoaDon.NgayTao.Month == month)
                    .SumAsync(ob => ob.SoLuong * ob.Sach!.GiaBan);

                dto.RevenueByMonth.Add(monthlyRevenue / 1_000_000); 

                // Sản phẩm bán theo tháng
                int monthlyProducts = await _db.OrderBooks
                    .Include(ob => ob.HoaDon)
                    .Where(ob =>
                        ob.HoaDon!.TrangThai == EnumStatus.success &&
                        ob.HoaDon.NgayTao.Month == month)
                    .SumAsync(ob => ob.SoLuong);

                dto.ProductsByMonth.Add(monthlyProducts);

                // Số đơn theo tháng (đơn đã duyệt)
                int monthlyOrders = await approvedOrders
                    .Where(o => o.NgayTao.Month == month)
                    .CountAsync();

                dto.OrdersByMonth.Add(monthlyOrders);
            }

            return dto;
        }
    }
}
