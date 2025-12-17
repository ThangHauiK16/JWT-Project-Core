using JWT_Project_Core.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Project_CoreTests.Infrastructure
{
    public static class TestMockFactory
    {
        public static Mock<IFileService> CreateFileService()
        {
            return new Mock<IFileService>();
        }

        public static Mock<IEmailService> CreateEmailService()
        {
            return new Mock<IEmailService>();
        }
    }
}
