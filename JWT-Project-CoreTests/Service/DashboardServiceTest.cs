using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Model;
using JWT_Project_CoreTests.Setups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Service
{
    [TestClass]
    public class DashboardServiceTest
    {
        private DashboardServiceTestSetup _setup = null!;

        [TestInitialize]
        public void Init()
        {
            _setup = new DashboardServiceTestSetup(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public async Task GetDashboardDataAsync_ShouldReturnCorrectDashboardData()
        {
            var book1 = new Book { MaSach = "S1", GiaBan = 100_000 };
            var book2 = new Book { MaSach = "S2", GiaBan = 200_000 };

            _setup.Context.Books.AddRange(book1, book2);

            var order1 = new Order
            {
                MaHoaDon = Guid.NewGuid(),
                TrangThai = EnumStatus.success,
                NgayTao = new DateTime(2025, 1, 10)
            };

            var order2 = new Order
            {
                MaHoaDon = Guid.NewGuid(),
                TrangThai = EnumStatus.success,
                NgayTao = new DateTime(2025, 2, 5)
            };

            var orderCancel = new Order
            {
                MaHoaDon = Guid.NewGuid(),
                TrangThai = EnumStatus.cancel,
                NgayTao = new DateTime(2025, 1, 20)
            };

            _setup.Context.Orders.AddRange(order1, order2, orderCancel);

            _setup.Context.OrderBooks.AddRange(
                new OrderBook
                {
                    HoaDon = order1,
                    Sach = book1,
                    SoLuong = 2
                },
                new OrderBook
                {
                    HoaDon = order2,
                    Sach = book2,
                    SoLuong = 3
                },
                new OrderBook
                {
                    HoaDon = orderCancel,
                    Sach = book1,
                    SoLuong = 10 
                }
            );

            await _setup.Context.SaveChangesAsync();

            DashboardDto result = await _setup.Service.GetDashboardDataAsync();


            Assert.AreEqual(2, result.TotalOrders);

            Assert.AreEqual(5, result.TotalProductsSold);

            Assert.AreEqual(800_000, result.TotalRevenue);

            Assert.AreEqual(12, result.RevenueByMonth.Count);
            Assert.AreEqual(12, result.ProductsByMonth.Count);
            Assert.AreEqual(12, result.OrdersByMonth.Count);

            Assert.AreEqual(0.2, result.RevenueByMonth[0]);
            Assert.AreEqual(2, result.ProductsByMonth[0]);
            Assert.AreEqual(1, result.OrdersByMonth[0]);

            Assert.AreEqual(0.6, result.RevenueByMonth[1]);
            Assert.AreEqual(3, result.ProductsByMonth[1]);
            Assert.AreEqual(1, result.OrdersByMonth[1]);
        }

        [TestMethod]
        public async Task GetDashboardDataAsync_WhenNoOrders_ShouldReturnZero()
        {
            var result = await _setup.Service.GetDashboardDataAsync();

            Assert.AreEqual(0, result.TotalOrders);
            Assert.AreEqual(0, result.TotalProductsSold);
            Assert.AreEqual(0, result.TotalRevenue);
        }
    }
}
