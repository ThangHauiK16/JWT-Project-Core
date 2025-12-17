using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using JWT_Project_CoreTests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;

namespace JWT_Project_CoreTests.Setups
{
    public class AuthServiceTestSetup
    {
        public ApplicationDbContext Context { get; }
        public AuthService Service { get; }
        public Mock<IEmailService> EmailServiceMock { get; }
        public IConfiguration Configuration { get; }
        public IMapper Mapper { get; }

        public AuthServiceTestSetup(string dbName)
        {
            Context = DbContextFactory.Create(dbName);
            Mapper = AutoMapperFactory.Create();
            EmailServiceMock = TestMockFactory.CreateEmailService();

            var jwtSettings = new Dictionary<string, string>
            {
              { "Jwt:Key", "THIS_IS_A_VERY_LONG_TEST_SECRET_KEY_123456789!!!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpireMinutes", "60" }
            };

            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(jwtSettings!)
                .Build();

            Service = new AuthService(
                Context,
                Configuration,
                Mapper,
                EmailServiceMock.Object
            );
        }
    }
}
