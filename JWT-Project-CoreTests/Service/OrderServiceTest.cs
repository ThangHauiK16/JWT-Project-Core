using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Model;
using JWT_Project_CoreTests.Setups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Service
{
    [TestClass]
    public class OrderServiceTest
    {
        private OrderServiceTestSetup _setup = null!;

        [TestInitialize]
        public void Init()
        {
            _setup = new OrderServiceTestSetup(Guid.NewGuid().ToString());
        }

      
        [TestMethod]
        public async Task GetByIdAsync_WhenExists_ShouldReturnOrder()
        {
            var id = Guid.NewGuid();

            _setup.Context.Orders.Add(new Order
            {
                MaHoaDon = id,
                Username = "thang",
                TrangThai = EnumStatus.pending
            });
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.MaHoaDon);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            var result = await _setup.Service.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

      
        [TestMethod]
        public async Task GetByUserAsync_ShouldReturnOrdersOfUser()
        {
            _setup.Context.Orders.AddRange(
                new Order { Username = "thang" },
                new Order { Username = "thang" },
                new Order { Username = "other" }
            );
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetByUserAsync("thang");

            Assert.AreEqual(2, result.Count);
        }

      
        [TestMethod]
        public async Task AddAsync_WhenValid_ShouldCreateOrder()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                TenSach = "Clean Code",
                SoLuong = 10,
                GiaBan = 100000
            });
            await _setup.Context.SaveChangesAsync();

            var dto = new OrderDTO
            {
                Username = "thang",
                Order_Books = new List<OrderBookDTO>
                {
                    new OrderBookDTO { MaSach = "B1", SoLuong = 2 }
                }
            };

            var result = await _setup.Service.AddAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual("thang", result.Username);
            Assert.AreEqual(1, result.Order_Books.Count);
        }

       
        [TestMethod]
        public async Task UpdateAsync_WhenExists_ShouldUpdateOrderBooks()
        {
            var id = Guid.NewGuid();

            _setup.Context.Orders.Add(new Order
            {
                MaHoaDon = id,
                OrderBooks = new List<OrderBook>
                {
                    new OrderBook { MaSach = "OLD", SoLuong = 1 }
                }
            });
            await _setup.Context.SaveChangesAsync();

            var dto = new OrderDTO
            {
                Order_Books = new List<OrderBookDTO>
                {
                    new OrderBookDTO { MaSach = "NEW", SoLuong = 3 }
                }
            };

            var result = await _setup.Service.UpdateAsync(dto, id);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Order_Books.Count);
            Assert.AreEqual("NEW", result.Order_Books[0].MaSach);
        }

       
        [TestMethod]
        public async Task DeleteAsync_WhenExists_ShouldReturnTrue()
        {
            var id = Guid.NewGuid();

            _setup.Context.Orders.Add(new Order { MaHoaDon = id });
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.DeleteAsync(id);

            Assert.IsTrue(result);
        }

      
        [TestMethod]
        public async Task ApproveAsync_ShouldSetStatusSuccess()
        {
            var id = Guid.NewGuid();

            _setup.Context.Orders.Add(new Order
            {
                MaHoaDon = id,
                TrangThai = EnumStatus.pending
            });
            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.ApproveAsync(id);

            var order = await _setup.Context.Orders.FindAsync(id);

            Assert.IsTrue(result);
            Assert.AreEqual(EnumStatus.success, order!.TrangThai);
        }

      
        [TestMethod]
        public async Task CancelAsync_ShouldCancelOrder_AndRestoreStock()
        {
            var id = Guid.NewGuid();

            var book = new Book
            {
                MaSach = "B1",
                SoLuong = 5
            };

            _setup.Context.Books.Add(book);

            // Giả lập đã checkout → trừ kho
            book.SoLuong -= 2;


            _setup.Context.Orders.Add(new Order
            {
                MaHoaDon = id,
                TrangThai = EnumStatus.pending,
                OrderBooks = new List<OrderBook>
                {
                    new OrderBook { MaSach = "B1", SoLuong = 2 }
                }
            });

            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.CancelAsync(id);

            Assert.IsTrue(result);
            Assert.AreEqual(5, book!.SoLuong);
        }
    }
}
