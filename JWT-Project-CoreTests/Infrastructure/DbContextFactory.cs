using JWT_Project_Core.Data;
using JWT_Project_Core.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Infrastructure
{
    public static class DbContextFactory
    {
        public static ApplicationDbContext Create(string dbName)
        {
            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock
                .Setup(x => x.GetUsername())
                .Returns("unit-test-user");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(
                options,
                currentUserMock.Object
            );
        }
    }
}

