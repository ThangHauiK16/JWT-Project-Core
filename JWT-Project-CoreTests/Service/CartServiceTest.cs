using JWT_Project_Core.Model;
using JWT_Project_CoreTests.Setups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Service
{
    [TestClass]
    public class CartServiceTest
    {
        private CartServiceTestSetup _setup = null!;
        private const string USER = "test-user";

        [TestInitialize]
        public void Init()
        {
            _setup = new CartServiceTestSetup(Guid.NewGuid().ToString());
        }

       
        [TestMethod]
        public async Task GetCartAsync_WhenNotExists_ShouldCreateNewCart()
        {
            var cart = await _setup.Service.GetCartAsync(USER);

            Assert.IsNotNull(cart);
            Assert.AreEqual(USER, cart.Username);
            Assert.AreEqual(0, cart.Items?.Count);
        }

       
        [TestMethod]
        public async Task AddToCartAsync_WhenValid_ShouldAddItem()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                TenSach = "Test Book",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            var cart = await _setup.Service.AddToCartAsync(USER, "B1", 2);

            Assert.AreEqual(1, cart.Items?.Count);
            Assert.AreEqual(2, cart.Items?.First().SoLuong);
        }

        [TestMethod]
        public async Task AddToCartAsync_WhenAddSameBook_ShouldIncreaseQuantity()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddToCartAsync(USER, "B1", 2);
            var cart = await _setup.Service.AddToCartAsync(USER, "B1", 3);

            Assert.AreEqual(1, cart.Items?.Count);
            Assert.AreEqual(5, cart.Items?.First().SoLuong);
        }

       
        [TestMethod]
        public async Task UpdateQuantityAsync_WhenValid_ShouldUpdate()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddToCartAsync(USER, "B1", 2);
            var cart = await _setup.Service.UpdateQuantityAsync(USER, "B1", 5);

            Assert.AreEqual(5, cart.Items?.First().SoLuong);
        }

    
        [TestMethod]
        public async Task IncreaseQuantityAsync_ShouldIncreaseByOne()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddToCartAsync(USER, "B1", 1);
            var cart = await _setup.Service.IncreaseQuantityAsync(USER, "B1");

            Assert.AreEqual(2, cart.Items?.First().SoLuong);
        }

      
        [TestMethod]
        public async Task DecreaseQuantityAsync_WhenQuantityIsOne_ShouldRemoveItem()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddToCartAsync(USER, "B1", 1);
            var cart = await _setup.Service.DecreaseQuantityAsync(USER, "B1");

            Assert.AreEqual(0, cart.Items?.Count);
        }

        [TestMethod]
        public async Task RemoveItemAsync_WhenExists_ShouldReturnTrue()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddToCartAsync(USER, "B1", 2);

            var result = await _setup.Service.RemoveItemAsync(USER, "B1");

            Assert.IsTrue(result);
            var cart = await _setup.Service.GetCartAsync(USER);
            Assert.AreEqual(0, cart.Items?.Count);
        }

       
        [TestMethod]
        public async Task CheckoutAsync_WhenValid_ShouldCreateOrderAndDecreaseStock()
        {
            _setup.Context.Books.Add(new Book
            {
                MaSach = "B1",
                SoLuong = 10
            });
            await _setup.Context.SaveChangesAsync();

            await _setup.Service.AddToCartAsync(USER, "B1", 3);

            var order = await _setup.Service.CheckoutAsync(USER);

            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.Order_Books.Count);

            var book = await _setup.Context.Books.FindAsync("B1");
            Assert.AreEqual(7, book!.SoLuong);
        }
    }
}
