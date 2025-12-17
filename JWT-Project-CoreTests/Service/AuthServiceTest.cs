using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Model;
using JWT_Project_Core.Model.Human;
using JWT_Project_CoreTests.Setups;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Service
{
    [TestClass]
    public class AuthServiceTest
    {
        private AuthServiceTestSetup _setup = null!;

        [TestInitialize]
        public void Init()
        {
            _setup = new AuthServiceTestSetup(Guid.NewGuid().ToString());
        }
        [TestMethod]
        public async Task LoginAsync_ValidUser_ReturnToken()
        {
         

            var user = new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = EnumRole.Customer,
                IsDeleted = false
            };

            _setup.Context.Users.Add(user);
            await _setup.Context.SaveChangesAsync();

            var loginDto = new LoginDTO
            {
                Username = "testuser",
                Password = "123456"
            };

            var result = await _setup.Service.LoginAsync(loginDto);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task LoginAsync_WrongPassword_ReturnNull()
        {


            _setup.Context.Users.Add(new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = EnumRole.Customer
            });

            await _setup.Context.SaveChangesAsync();

            var loginDto = new LoginDTO
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var result = await _setup.Service.LoginAsync(loginDto);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task RegisterAsync_NewUser_ReturnSuccess()
        {
          

            var dto = new RegisterDTO
            {
                Username = "newuser",
                Password = "Abc@1234",
                Email = "newuser@gmail.com"
            };

            var result = await _setup.Service.RegisterAsync(dto);

            Assert.AreEqual("User created successfully", result);
            Assert.AreEqual(1, _setup.Context.Users.Count());
        }

        [TestMethod]
        public async Task RegisterAsync_DuplicateUsername_ReturnError()
        {


            _setup.Context.Users.Add(new User
            {
                Username = "testuser",
                Email = "a@gmail.com",
                Password = "123",
                IsDeleted = false
            });

            await _setup.Context.SaveChangesAsync();

            var dto = new RegisterDTO
            {
                Username = "testuser",
                Password = "Abc@1234",
                Email = "b@gmail.com"
            };

            var result = await _setup.Service.RegisterAsync(dto);

            Assert.AreEqual("Username already exists", result);
        }

        [TestMethod]
        public async Task RefreshTokenAsync_ValidToken_ReturnNewAccessToken()
        {

            var user = new User
            {
                Username = "testuser",
                Password = "123",
                Role = EnumRole.Customer
            };

            _setup.Context.Users.Add(user);

            _setup.Context.RefreshTokens.Add(new RefreshToken
            {
                Token = "valid-refresh",
                Username = "testuser",
                Expires = DateTime.UtcNow.AddDays(1)
            });

            await _setup.Context.SaveChangesAsync();

            var token = await _setup.Service.RefreshTokenAsync("valid-refresh");

            Assert.IsNotNull(token);
        }

        [TestMethod]
        public async Task ForgotPasswordAsync_ValidEmail_SendEmail()
        {

            _setup.Context.Users.Add(new User
            {
                Username = "testuser",
                Email = "test@gmail.com",
                Password = "123",
                IsDeleted = false
            });

            await _setup.Context.SaveChangesAsync();

            var result = await _setup.Service.ForgotPasswordAsync("test@gmail.com");

            Assert.AreEqual("New password has been sent to your email", result);

            _setup.EmailServiceMock.Verify(
                x => x.SendEmailAsync(
                    "test@gmail.com",
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

    }
}
