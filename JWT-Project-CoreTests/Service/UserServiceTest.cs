using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Model.Human;
using JWT_Project_CoreTests.Setups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Service
{
    [TestClass]
    public class UserServiceTest
    {
        private UserServiceTestSetup _setup = null!;

        [TestInitialize]
        public void Init()
        {
            _setup = new UserServiceTestSetup(Guid.NewGuid().ToString());
        }


        [TestMethod]
        public async Task GetUsersAsync_ShouldReturnPagedUsers()
        {
            _setup.Context.Users.AddRange(
                new User { Username = "admin", Email = "admin@test.com", Role = EnumRole.Admin },
                new User { Username = "user1", Email = "user1@test.com", Role = EnumRole.Customer },
                new User { Username = "user2", Email = "user2@test.com", Role = EnumRole.Customer }
            );

            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetUsersAsync(1, 2);

            Assert.AreEqual(2, result.Items.Count());
            Assert.AreEqual(3, result.TotalItems);
        }

        [TestMethod]
        public async Task GetUsersAsync_WithKeyword_ShouldFilterCorrectly()
        {
            _setup.Context.Users.AddRange(
                new User { Username = "admin", Email = "admin@test.com", Role = EnumRole.Admin },
                new User { Username = "user1", Email = "user1@test.com", Role = EnumRole.Customer }
            );

            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetUsersAsync(1, 10, "admin");

            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual("admin", result.Items.First().Username);
        }


        [TestMethod]
        public async Task GetUserAsync_ShouldReturnUser_WhenExists()
        {
            _setup.Context.Users.Add(new User
            {
                Username = "user1",
                Email = "user1@test.com",
                Role = EnumRole.Customer
            });

            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.GetUserAsync("user1");

            Assert.IsNotNull(result);
            Assert.AreEqual("user1", result!.Username);
        }

        [TestMethod]
        public async Task GetUserAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _setup.Service.GetUserAsync("notfound");

            Assert.IsNull(result);
        }


        [TestMethod]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenExists()
        {
            _setup.Context.Users.Add(new User
            {
                Username = "user1",
                Email = "old@test.com",
                Role = EnumRole.Customer
            });

            await _setup.Context.SaveChangesAsync();

            var dto = new UserDTO
            {
                Email = "new@test.com",
                Role = EnumRole.Admin
            };

            var result = await _setup.Service.UpdateUserAsync("user1", dto);

            var user = await _setup.Context.Users.FindAsync("user1");

            Assert.IsTrue(result);
            Assert.AreEqual("new@test.com", user!.Email);
            Assert.AreEqual(EnumRole.Admin, user.Role);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldReturnFalse_WhenUserNotExists()
        {
            var dto = new UserDTO
            {
                Email = "test@test.com",
                Role = EnumRole.Customer
            };

            var result = await _setup.Service.UpdateUserAsync("notfound", dto);

            Assert.IsFalse(result);
        }

      

        [TestMethod]
        public async Task DeleteUserAsync_ShouldSoftDeleteUser()
        {
            _setup.Context.Users.Add(new User
            {
                Username = "user1",
                Email = "user1@test.com",
                Role = EnumRole.Customer,
                IsDeleted = false
            });

            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.DeleteUserAsync("user1");

            var user = await _setup.Context.Users.FindAsync("user1");

            Assert.IsTrue(result);
            Assert.IsTrue(user!.IsDeleted);
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotExists()
        {
            var result = await _setup.Service.DeleteUserAsync("notfound");

            Assert.IsFalse(result);
        }
    }
}
